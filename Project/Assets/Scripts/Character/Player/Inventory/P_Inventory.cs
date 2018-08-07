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



    // Array to hold what items are currently held
    private Slot[] m_slotArray;

    // Size of the currently equiped bag
    private int m_curBagSize = 25;

    // Amount of items to display in a horizontal line before the line wraps
    private int m_horSlotsToDisplay = 6;

    // Internal inventory open check --- MOVE TO GAME CONTROLLER
    private bool m_inventoryOpen = false;

    // Grid layout UI component for laying out cell slot objects
    private GridLayoutGroup m_inventoryGridLayout;

    private GridLayoutGroup m_containerGridLayout;

    // Check if screen width changed
    private float m_lastWidth;

    // Use this for initialization
    void Start()
    {
        // Get reference to the grid layout
        m_inventoryGridLayout = m_inventorySlotSubPanel.GetComponent<GridLayoutGroup>();

        m_containerGridLayout = m_containerSlotSubPanel.GetComponent<GridLayoutGroup>();

        // Set the cell sizes initially
        INV_Library.AdjustGridCells(m_inventoryPanel, m_inventoryGridLayout, m_horSlotsToDisplay);
        INV_Library.AdjustGridCells(m_containerPanel, m_containerGridLayout, m_horSlotsToDisplay);

        // Layout the slots to begin with, initiallizes and populates array also
        m_slotArray = INV_Library.LayoutSlots(m_curBagSize, m_slotObject, m_inventorySlotSubPanel);
    }

    // Update is called once per frame
    void Update()
    {
        // "OPENING STANDARD INVENTORY"
        if (Input.GetButtonDown("Y_Button"))
        {
            ToggleInventory();
        }

        // "OPENING A CONTAINER"
        if (Input.GetButtonDown("X_Button"))
        {
            //OpenContainer();
        }


        // Accomodate resolution change in inventory
        if (m_lastWidth != Screen.width)
        {
            // Set the cell sizes on rescale
            INV_Library.AdjustGridCells(m_inventoryPanel, m_inventoryGridLayout, m_horSlotsToDisplay);
            INV_Library.AdjustGridCells(m_containerPanel, m_containerGridLayout, m_horSlotsToDisplay);
        }

        m_lastWidth = Screen.width;
    }

    // Open a container with the inventory (interface function
    public void OpenContainer(WORLD_Container containerReference)
    {
        // Layout the slots to begin with, initiallizes and populates array also
        m_slotArray = INV_Library.LayoutSlots(containerReference.CONTAINER_SIZE, m_slotObject, m_containerSlotSubPanel);
        ToggleInventory(1);
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
}
