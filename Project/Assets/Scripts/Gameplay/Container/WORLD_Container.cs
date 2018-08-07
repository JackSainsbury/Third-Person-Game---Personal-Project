using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// INTERFACE COMPONENT OF CONTAINERS (INVENTORY EXTENSION)
/// 
/// </summary>
public class WORLD_Container : MonoBehaviour {
    // The world icon for activating the chest
    public GameObject m_XIcon;

    // Size of the container
    [SerializeField]
    private int m_containerSize = 5;
    [SerializeField]
    private SO_LootTable m_containerTable;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<P_Inventory>().OpenContainer(this);
            //m_XIcon.SetActive(true);
        }
    }

    // Property for the size in slots of this container
    public int CONTAINER_SIZE
    {
        get
        {
            return m_containerSize;
        }
    }

    // Property for the loot table relating to this container
    public SO_LootTable LOOT_TABLE
    {
        get
        {
            return m_containerTable;
        }
    }
}
