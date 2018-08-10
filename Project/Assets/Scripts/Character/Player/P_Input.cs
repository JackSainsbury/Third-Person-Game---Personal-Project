using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Input : MonoBehaviour {

    public P_Inventory m_inventoryScript;
    public P_ItemSwapping m_itemSwapScript;
    public P_Move m_moveScript;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        
        // "OPENING STANDARD INVENTORY"
        if (Input.GetButtonDown("B_Button"))
        {
            m_inventoryScript.ToggleInventory();
        }
        
        // "OPENING A CONTAINER"
        if (Input.GetButtonDown("A_Button"))
        {
            if (!LIB_GameController.IS_INVENTORY_OPEN)
            {
                m_inventoryScript.OpenContainer();
            }
            else
            {
                m_itemSwapScript.SelectSlot();
            }
        }

        // General Dash
        if (Input.GetButtonDown("X_Button"))
        {
            if (!LIB_GameController.IS_INVENTORY_OPEN)
            {
                m_moveScript.Dash();
            }
            else
            {
                m_itemSwapScript.DropSlot();
            }
        }

        if (Input.GetButtonDown("Y_Button"))
        {
            if (!LIB_GameController.IS_INVENTORY_OPEN)
            {
                m_moveScript.Jump();
            }
        }

        if (Input.GetButtonDown("R_Bumper"))
        {
            m_inventoryScript.SetOnContainerMenu(true);
        }
        else if (Input.GetButtonDown("L_Bumper"))
        {
            m_inventoryScript.SetOnContainerMenu(false);
        }

        // Left joystick
        float verL = Input.GetAxis("VerticalL");
        float horL = Input.GetAxis("HorizontalL");

        // Right joystick
        float verR = Input.GetAxis("VerticalR");
        float horR = Input.GetAxis("HorizontalR");

        if (!LIB_GameController.IS_INVENTORY_OPEN)
        {
            m_moveScript.DoMove(horL, verL, horR, verR);
        }
        else
        {
            m_inventoryScript.ScrollInventory(new Vector2(horL, verL));
        }
    }
}
