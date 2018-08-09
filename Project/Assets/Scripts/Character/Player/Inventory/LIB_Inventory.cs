using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LIB_Inventory : MonoBehaviour {

    // Current Cell size
    private static float m_cellDimension;


    public static void AdjustGridCells(GameObject panel, GridLayoutGroup slotGridLayout, int horSlotsToDisplay)
    {
        // Set the cell sizes
        float cellDimension = (panel.GetComponent<RectTransform>().rect.width - (slotGridLayout.spacing.x * (horSlotsToDisplay + 5))) / horSlotsToDisplay;
        slotGridLayout.cellSize = new Vector2(cellDimension, cellDimension);

        m_cellDimension = cellDimension;
    }

    public static Slot[] LayoutSlots(int curBagSize, GameObject slotObject, GameObject slotSubPanel)
    {
        // Clean up old cells
        int cellCount = slotSubPanel.transform.childCount;
        for (int i = 0; i < cellCount; ++i)
        {
            Destroy(slotSubPanel.transform.GetChild((cellCount - i) - 1).gameObject);
        }

        Slot[] m_slotArray = new Slot[curBagSize];

        // Do full rows
        for (int i = 0; i < curBagSize; ++i)
        {
            // Parent to the container to display on UI Canvas
            GameObject newSlot = Instantiate(slotObject, Vector3.zero, Quaternion.identity);

            newSlot.transform.SetParent(slotSubPanel.transform);

            // Push new slot to next working index
            m_slotArray[i] = new Slot(0, 0, newSlot);
        }

        return m_slotArray;
    }


    public static float CELL_DIMENSION
    {
        get
        {
            return m_cellDimension;
        }
    }
}

