using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class P_Inventory : MonoBehaviour
{
    public GameObject m_inventoryBackground;

    // Main Panel on the canvas to hold inventory components
    public GameObject m_inventoryPanel;
    // Main Panel on the canvas to hold inventory components
    public GameObject m_inventorySlotSubPanel;
    // GameObject instantiated to display inventory items in
    public GameObject m_slotObject;

    // For state 1, load this left hand panel
    public GameObject m_containerPanel;
    // Main Panel on the canvas to hold inventory components
    public GameObject m_containerSlotSubPanel;
    // For state 0, load this left hand panel
    public GameObject m_equipmentPanel;

    // Slot highlight prefab, signifies the currently selected icon
    public GameObject m_slotHighlighter;



    // Array to hold what items are currently held
    private Slot[] m_slotArrayInventory;
    // Array to hold what items are currently held
    private Slot[] m_slotArrayContainer;

    // Size of the currently equiped bag
    private int m_curBagSize = 28;

    // Amount of items to display in a horizontal line before the line wraps
    private int m_horSlotsToDisplay = 6;

    // Internal inventory open check --- MOVE TO GAME CONTROLLER
    private bool m_inventoryOpen = false;

    // Grid layout UI component for laying out cell slot objects
    private GridLayoutGroup m_inventoryGridLayout;

    private GridLayoutGroup m_containerGridLayout;

    // List of containers which are in range of the player
    private List<WORLD_Container> m_containersInRange;

    // Check if screen width changed
    private float m_lastWidth;

    // Scroll target at 0,0 initially
    private Vector2 m_invScrollTarget = Vector2.zero;

    // Used to delay between auto scrolling
    private bool m_canScrollInv = true;
    // The delaying coroutine, cached here so that we can interrupt if left stick is released
    private IEnumerator m_scrollCoroutine;

    // If there are 1 or more containers in my inRange list, keep checking to see which is closest and cycle the visuals
    private IEnumerator m_closestContainterCheckCoroutine;


    // The left hand panel, what pallette am I displaying (character info / container)
    private int m_palletteState = 0;

    // (if open) the current open container, else null
    private WORLD_Container m_openContainer;

    // If the container is open, am I currently on my inventory menu or the container menu
    private bool m_invOrContainer = false;

    // Use this for initialization
    void Start()
    {
        // Create a new list for the containers which enter our range
        m_containersInRange = new List<WORLD_Container>();


        // Get reference to the grid layout
        m_inventoryGridLayout = m_inventorySlotSubPanel.GetComponent<GridLayoutGroup>();
        m_containerGridLayout = m_containerSlotSubPanel.GetComponent<GridLayoutGroup>();

        // Set the cell sizes initially
        LIB_Inventory.AdjustGridCells(m_inventoryPanel, m_inventoryGridLayout, m_horSlotsToDisplay);
        LIB_Inventory.AdjustGridCells(m_containerPanel, m_containerGridLayout, m_horSlotsToDisplay);

        // Layout the slots to begin with, initiallizes and populates array also
        m_slotArrayInventory = LIB_Inventory.LayoutSlots(m_curBagSize, m_slotObject, m_inventorySlotSubPanel);

        if (m_slotArrayInventory.Length > 0)
        {
            LIB_Inventory.ForceGridLayoutGroupRebuild(m_inventorySlotSubPanel.GetComponent<RectTransform>());

            m_slotHighlighter.transform.position = m_slotArrayInventory[0].SLOTOBJ.transform.position;
            m_slotHighlighter.GetComponent<RectTransform>().sizeDelta = new Vector2(LIB_Inventory.CELL_DIMENSION * 1.1f, LIB_Inventory.CELL_DIMENSION * 1.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {

        // Do container distance and icon display cycling (every 0.1 seconds the list is checked)
        if (m_containersInRange.Count > 0)
        {
            if (m_closestContainterCheckCoroutine == null)
            {
                m_closestContainterCheckCoroutine = containerIconCheck();
                StartCoroutine(m_closestContainterCheckCoroutine);
            }
        }

        // Accomodate resolution change in inventory
        if (m_lastWidth != Screen.width)
        {
            // Set the cell sizes on rescale
            LIB_Inventory.AdjustGridCells(m_inventoryPanel, m_inventoryGridLayout, m_horSlotsToDisplay);
            LIB_Inventory.AdjustGridCells(m_containerPanel, m_containerGridLayout, m_horSlotsToDisplay);

            // Rebuild the inventory grid on adjust
            LIB_Inventory.ForceGridLayoutGroupRebuild(m_inventorySlotSubPanel.GetComponent<RectTransform>());

            // Adjust the highlighter slot position on resize
            m_slotHighlighter.transform.position = m_slotArrayInventory[0].SLOTOBJ.transform.position;
            m_slotHighlighter.GetComponent<RectTransform>().sizeDelta = new Vector2(LIB_Inventory.CELL_DIMENSION * 1.1f, LIB_Inventory.CELL_DIMENSION * 1.1f);
        }

        m_lastWidth = Screen.width;
    }

    // Inventory is open and left stick has been moved
    public void ScrollInventory(Vector2 leftStick)
    {
        // Only 4 directional movement
        if (Mathf.Abs(leftStick.x) > Mathf.Abs(leftStick.y))

        {
            leftStick.y = 0;
        }
        else
        {
            leftStick.x = 0;
        }

        // Set the input vector to move discretely
        leftStick = new Vector2(
            leftStick.x > 0.15f ? 1 : leftStick.x < -0.15f ? -1 : 0,
            leftStick.y > 0.15f ? -1 : leftStick.y < -0.15f ? 1 : 0
            );


        if (leftStick.x != 0 || leftStick.y != 0)
        {
            if (m_canScrollInv)
            {
                // The current bag size
                int activeBagSize = m_curBagSize;
                Slot[] activeArray = m_slotArrayInventory;

                if (m_palletteState == 1)
                {
                    if (m_openContainer != null)
                    {
                        if (!m_invOrContainer)
                        {
                            activeBagSize = m_openContainer.CONTAINER_SIZE;
                            activeArray = m_slotArrayContainer;
                        }
                    }
                }

                Vector2 candidateScrollTarget = m_invScrollTarget + leftStick;
                int nextTarget = (int)candidateScrollTarget.y * m_horSlotsToDisplay + (int)candidateScrollTarget.x;

                // Get the length of the last row
                int lastRowLength = activeBagSize % m_horSlotsToDisplay;
                lastRowLength = lastRowLength == 0 ? m_horSlotsToDisplay : lastRowLength;

                int lastFullRow = activeBagSize / m_horSlotsToDisplay;

                // Scroll looping
                if (leftStick.y != 0)
                {
                    if (leftStick.y == 1)
                    {
                        // Moving up has taken me out of range
                        if (nextTarget >= activeArray.Length)
                        {
                            candidateScrollTarget.y = 0;
                        }
                    }
                    else
                    {
                        if(nextTarget < 0)
                        {
                            if (lastRowLength != m_horSlotsToDisplay)
                            {
                                if (candidateScrollTarget.x >= lastRowLength)
                                {
                                    candidateScrollTarget.y = lastFullRow - 1;
                                }
                                else
                                {
                                    candidateScrollTarget.y = lastFullRow;
                                }
                            }
                            else
                            {
                                candidateScrollTarget.y = lastFullRow - 1;
                            }
                        }
                    }
                }
                else
                {
                    if(leftStick.x == 1)
                    {
                        // For all rows this applies
                        if(candidateScrollTarget.x >= m_horSlotsToDisplay)
                        {
                            candidateScrollTarget.x = 0;
                        }
                        else
                        {
                            // Check if I'm on the final row
                            if (candidateScrollTarget.y == lastFullRow)
                            {
                                // If so check if the final row is short
                                if (lastRowLength != m_horSlotsToDisplay)
                                {
                                    // If so, check if I've gone off the end of the short row
                                    if (candidateScrollTarget.x >= lastRowLength)
                                    {
                                        candidateScrollTarget.x = 0;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (candidateScrollTarget.x < 0)
                        {
                            candidateScrollTarget.x = m_horSlotsToDisplay - 1;

                            // Check if I'm on the final row
                            if (candidateScrollTarget.y == lastFullRow)
                            {
                                // If so check if the final row is short
                                if (lastRowLength != m_horSlotsToDisplay)
                                {
                                    candidateScrollTarget.x = lastRowLength - 1;
                                }
                            }
                        }
                    }
                }
                

                nextTarget = (int)candidateScrollTarget.y * m_horSlotsToDisplay + (int)candidateScrollTarget.x;
                
                if (nextTarget >= 0 && nextTarget < activeArray.Length)
                {
                    m_invScrollTarget = candidateScrollTarget;

                    m_slotHighlighter.transform.position = activeArray[nextTarget].SLOTOBJ.transform.position;

                    m_canScrollInv = false;

                    // Start the delay coroutine
                    m_scrollCoroutine = scrollDelay();
                    StartCoroutine(m_scrollCoroutine);
                }
            }
        }
        else
        {
            if (m_scrollCoroutine != null)
            {
                StopCoroutine(m_scrollCoroutine);
                m_canScrollInv = true;
            }
        }
    }

    // Delaying coroutine for automatic scrolling of the cursor, when the user holds down the left stick in the inventory
    IEnumerator scrollDelay()
    {
        yield return new WaitForSecondsRealtime(.2f);

        m_canScrollInv = true;
    }
    
    // Open a container with the inventory (interface function)
    public void OpenContainer()
    {
        SetOnContainerMenu(false);

        WORLD_Container closestContainerInRange = SortContainers();

        if (closestContainerInRange != null)
        {
            // Layout the slots to begin with, initiallizes and populates array also
            m_slotArrayContainer = LIB_Inventory.LayoutSlots(closestContainerInRange.CONTAINER_SIZE, m_slotObject, m_containerSlotSubPanel);

            if (m_slotArrayContainer.Length > 0)
            {
                m_openContainer = closestContainerInRange;

                // Force the layout initially
                LIB_Inventory.ForceGridLayoutGroupRebuild(m_containerSlotSubPanel.GetComponent<RectTransform>());
            }

            ToggleInventory(1);
        }
        else
        {
            m_openContainer = null;
        }
    }

    // Gets the closest container from the list of containers in range, if none exist, returns null
    public WORLD_Container SortContainers()
    {
        int numberOfContainersInRange = m_containersInRange.Count;

        if (numberOfContainersInRange > 0)
        {
            // Sort the containers by distance
            WORLD_Container closestContainerInRange = m_containersInRange[0];
            
            float closestDistance = Vector3.Distance(transform.position, closestContainerInRange.transform.position);

            // Find closest
            for (int i = 0; i < numberOfContainersInRange; ++i)
            {
                float candidateDistance = Vector3.Distance(transform.position, m_containersInRange[i].transform.position);

                m_containersInRange[i].m_openIcon.SetActive(true);

                if (candidateDistance < closestDistance)
                {
                    closestDistance = candidateDistance;

                    // Disable old
                    closestContainerInRange.m_openIcon.SetActive(false);
                    closestContainerInRange = m_containersInRange[i];
                }
                else
                {
                    // If I was comparing myself against the shorted distance, don't disable
                    if (closestContainerInRange != m_containersInRange[i])
                    {
                        m_containersInRange[i].m_openIcon.SetActive(false);
                    }
                }
            }

            return closestContainerInRange;
        }

        return null;
    }

    IEnumerator containerIconCheck()
    {
        while (m_containersInRange.Count > 0)
        {
            yield return new WaitForSeconds(0.1f);
            SortContainers();

            yield return null;
        }

        m_closestContainterCheckCoroutine = null;
    }

    // Perform basic toggle
    public void ToggleInventory(int palletteState = 0)
    {
        m_inventoryOpen = !m_inventoryOpen;
        SetInventoryState(palletteState);
    }
    // Set to value
    public void ToggleInventory(bool openStatus, int palletteState = 0)
    {
        m_inventoryOpen = openStatus;
        SetInventoryState(palletteState);
    }

    // Inventory state has "changed"
    void SetInventoryState(int palletteState)
    {

        // Toggle all inventory
        m_inventoryPanel.SetActive(m_inventoryOpen);
        m_inventoryBackground.SetActive(m_inventoryOpen);

        m_slotHighlighter.GetComponent<RectTransform>().sizeDelta = new Vector2(LIB_Inventory.CELL_DIMENSION * 1.1f, LIB_Inventory.CELL_DIMENSION * 1.1f);

        // Enable the slot highlighter
        m_slotHighlighter.SetActive(m_inventoryOpen);

        if (m_inventoryOpen)
        {
            // Pause Game
            Time.timeScale = 0;

            m_palletteState = palletteState;

            // Enable and disable all option windows accordingly
            switch (palletteState)
            {
                case 0:
                    // Opened inventory in its default state (Left panel should display character info etc)
                    m_equipmentPanel.SetActive(true);

                    m_containerPanel.SetActive(false);

                    m_invScrollTarget = Vector2.zero;
                    m_slotHighlighter.transform.position = m_slotArrayInventory[0].SLOTOBJ.transform.position;

                    break;
                case 1:
                    // Opened inventory via a container (Left panel should display container contents panel)
                    m_equipmentPanel.SetActive(false);

                    m_containerPanel.SetActive(true);

                    m_invScrollTarget = Vector2.zero;
                    m_slotHighlighter.transform.position = m_slotArrayContainer[0].SLOTOBJ.transform.position;

                    break;
            }
        }
        else
        {
            // Resume Game
            Time.timeScale = 1;

            m_invScrollTarget = Vector2.zero;

            LIB_Inventory.CleanContainer(m_containerSlotSubPanel);

            m_slotHighlighter.transform.position = m_slotArrayInventory[0].SLOTOBJ.transform.position;

            // Disable all option windows
            m_equipmentPanel.SetActive(false);
            m_containerPanel.SetActive(false);
        }

        // Update the game controller, static vars
        LIB_GameController.IS_INVENTORY_OPEN = m_inventoryOpen;
    }

    public void AddContainter(WORLD_Container toAddContainer)
    {
        m_containersInRange.Add(toAddContainer);
    }

    public void RemoveContainer(WORLD_Container toRemoveContainer)
    {
        m_containersInRange.Remove(toRemoveContainer);
    }

    // Swap between on the container or on the inventory (when appropriate)
    public void SetOnContainerMenu(bool value)
    {
        // Is a container actually open?
        if (m_palletteState == 1)
        {
            m_invOrContainer = value;

            m_invScrollTarget = Vector2.zero;

            if (m_inventoryOpen)
            {
                if (m_invOrContainer)
                {
                    m_slotHighlighter.transform.position = m_slotArrayInventory[0].SLOTOBJ.transform.position;
                }
                else
                {
                    m_slotHighlighter.transform.position = m_slotArrayContainer[0].SLOTOBJ.transform.position;
                }
            }
        }
        else
        {
            m_invOrContainer = false;
        }
    }

    // returns the currently hoverred slot
    public Slot GetCurrentSlot()
    {
        int nextTarget = (int)m_invScrollTarget.y * m_horSlotsToDisplay + (int)m_invScrollTarget.x;

        if (m_palletteState == 1)
        {
            if (m_invOrContainer)
            {
                return m_slotArrayInventory[nextTarget];
            }
            else
            {
                return m_slotArrayContainer[nextTarget];
            }
        }
        else
        {
            return m_slotArrayInventory[nextTarget];
        }
    }
}
