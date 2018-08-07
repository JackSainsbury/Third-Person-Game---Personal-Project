using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// This script is designed to hold the current state of the game as a whole,
/// mainly used to determine if the player can or cannot perform a particular
/// action.
/// 
/// Acting as a static collection of variables, which can be accessed from
/// anywhere, properties must be used in order to maintain healthy encapsulation
/// as the possibility for multiple scripts trying to modify/access values
/// is very high.
/// 
/// </summary>
public class LIB_GameController : MonoBehaviour {
    private static bool m_isInventoryOpen = false;
    private static bool m_isDashing = false;

    // Property for is inventory open
    public static bool IS_INVENTORY_OPEN
    {
        get
        {
            return m_isInventoryOpen;
        }

        set
        {
            m_isInventoryOpen = value;
        }
    }

    // Property for is dashing
    public static bool IS_DASHING
    {
        get
        {
            return m_isDashing;
        }

        set
        {
            m_isDashing = value;
        }
    }
}
