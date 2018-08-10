using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(P_Inventory))]
public class P_ItemSwapping : MonoBehaviour {
    // Base inventory ui and functional component
    private P_Inventory m_InventoryBaseComponent;

    // Held by the cursor, current ID and COUNT
    private int m_heldID;
    private int m_heldCOUNT;

	// Use this for initialization
	void Start () {
        m_InventoryBaseComponent = GetComponent<P_Inventory>();
    }
	
    // Pick up the item in the currently hovered slot
	public void SelectSlot()
    {
        // Get reference to the current slot
        Slot curSlot = m_InventoryBaseComponent.GetCurrentSlot();

        // Store the new held values
        m_heldID = curSlot.ID;
        m_heldCOUNT = curSlot.COUNT;

        // Place down values on (now empty) slot
        curSlot.ID = 0;
        curSlot.COUNT = 0;
    }

    // Drop the item in the currently hovered slot
    public void DropSlot()
    {
        Debug.Log(m_InventoryBaseComponent.GetCurrentSlot().SLOTOBJ.name);
    }
}
