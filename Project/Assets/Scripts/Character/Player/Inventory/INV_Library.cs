using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class INV_Library : MonoBehaviour {

   public static void AdjustGridCells(GameObject m_panel, GridLayoutGroup m_slotGridLayout, int m_horSlotsToDisplay)
    {
        // Set the cell sizes
        float cellDimension = (m_panel.GetComponent<RectTransform>().rect.width - (m_slotGridLayout.spacing.x * (m_horSlotsToDisplay + 5))) / m_horSlotsToDisplay;
        m_slotGridLayout.cellSize = new Vector2(cellDimension, cellDimension);
    }

    public static Slot[] LayoutSlots(int m_curBagSize, GameObject m_slotObject, GameObject m_slotSubPanel)
    {
        // Clean up old cells
        int cellCount = m_slotSubPanel.transform.childCount;
        for (int i = 0; i < cellCount; ++i)
        {
            Destroy(m_slotSubPanel.transform.GetChild((cellCount - i) - 1).gameObject);
        }

        Slot[] m_slotArray = new Slot[m_curBagSize];

        // Do full rows
        for (int i = 0; i < m_curBagSize; ++i)
        {
            // Parent to the container to display on UI Canvas
            GameObject newSlot = Instantiate(m_slotObject, Vector3.zero, Quaternion.identity);

            newSlot.transform.SetParent(m_slotSubPanel.transform);

            // Push new slot to next working index
            m_slotArray[i] = new Slot(0, 0, newSlot);
        }

        return m_slotArray;
    }
}
