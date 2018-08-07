using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot
{
    // ID 0 is an empty slot
    private int m_currentID = 0;
    // Number of the object in the slot
    private int m_count = 0;

    // GameObject in the inventoryPanel->SlotContainer which will describe the contents of this slot
    private GameObject m_slotObject;

    public Slot(int id, int count, GameObject slotObj)
    {
        m_currentID = id;
        m_count = count;
        m_slotObject = slotObj;
    }

    // ID property
    public int ID
    {
        get
        {
            return m_currentID;
        }

        set
        {
            m_currentID = value;
        }
    }

    // Count property
    public int COUNT
    {
        get
        {
            return m_count;
        }

        set
        {
            m_count = value;
        }
    }

    // Slot object property, readonly
    public GameObject SLOTOBJ
    {
        get
        {
            return m_slotObject;
        }
    }
}