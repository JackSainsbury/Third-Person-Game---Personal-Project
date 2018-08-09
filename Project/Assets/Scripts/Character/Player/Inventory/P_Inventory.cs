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
    private int m_curBagSize = 25;

    // Amount of items to display in a horizontal line before the line wraps
    private int m_horSlotsToDisplay = 6;

    // Internal inventory open check --- MOVE TO GAME CONTROLLER
    private bool m_inventoryOpen = false;

    // Grid layout UI component for laying out cell slot objects
    private GridLayoutGroup m_inventoryGridLayout;

    private GridLayoutGroup m_containerGridLayout;

    // List of containers which are in range of the player
    public List<WORLD_Container> m_containersInRange;

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

        // Do container distance and icon display cycling (every 0.3 seconds the list is checked)
        if(m_containersInRange.Count > 0)
        {
            if (m_closestContainterCheckCoroutine == null)
            {
                m_closestContainterCheckCoroutine = containerIconCheck();
                StartCoroutine(m_closestContainterCheckCoroutine);
            }
        }
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

        if (m_canScrollInv)
        {
            if (leftStick.x != 0 || leftStick.y != 0)
            {
                Vector2 candidateScrollTarget = m_invScrollTarget + leftStick;

                int nextTarget = (int)candidateScrollTarget.y * m_horSlotsToDisplay + (int)candidateScrollTarget.x;

                if (nextTarget >= 0 && nextTarget < m_slotArrayInventory.Length)
                {
                    m_invScrollTarget = candidateScrollTarget;

                    m_slotHighlighter.transform.position = m_slotArrayInventory[nextTarget].SLOTOBJ.transform.position;

                    m_canScrollInv = false;

                    // Start the delay coroutine
                    m_scrollCoroutine = scrollDelay();
                    StartCoroutine(m_scrollCoroutine);
                }
            }
        }
        else
        {
            if (leftStick.x == 0 && leftStick.y == 0)
            {
                StopCoroutine(m_scrollCoroutine);
                m_canScrollInv = true;
            }
        }
    }

    // Delaying coroutine for automatic scrolling of the cursor, when the user holds down the left stick in the inventory
    IEnumerator scrollDelay()
    {
        yield return new WaitForSeconds(.4f);
        m_canScrollInv = true;
    }

    // Open a container with the inventory (interface function
    public void OpenContainer()
    {
        WORLD_Container closestContainerInRange = SortContainers();

        if (closestContainerInRange != null)
        {
            // Layout the slots to begin with, initiallizes and populates array also
            m_slotArrayContainer = LIB_Inventory.LayoutSlots(closestContainerInRange.CONTAINER_SIZE, m_slotObject, m_containerSlotSubPanel);

            if (m_slotArrayContainer.Length > 0)
            {
                m_slotHighlighter.transform.position = m_slotArrayContainer[0].SLOTOBJ.transform.position;
                m_slotHighlighter.GetComponent<RectTransform>().sizeDelta = new Vector2(LIB_Inventory.CELL_DIMENSION * 1.1f, LIB_Inventory.CELL_DIMENSION * 1.1f);
            }

            ToggleInventory(1);
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
            // Enable and disable all option windows accordingly
            switch (palletteState)
            {
                case 0:
                    // Opened inventory in its default state (Left panel should display character info etc)
                    m_equipmentPanel.SetActive(true);

                    m_containerPanel.SetActive(false);

                    break;
                case 1:
                    // Opened inventory via a container (Left panel should display container contents panel)
                    m_equipmentPanel.SetActive(false);

                    m_containerPanel.SetActive(true);

                    break;
            }
        }
        else
        {
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
}
