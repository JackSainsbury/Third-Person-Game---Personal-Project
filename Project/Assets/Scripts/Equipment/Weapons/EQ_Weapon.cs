using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * Script to contain all components of a "currently" equipred weapon, which can be used for combat
 * 
*/ 

public class EQ_Weapon : MonoBehaviour {
	// Potentially store this information in a structure, for combos/triple swing style combat
	// Stores all swing stats and information
	public SO_SwordSwing m_swingObject;

	// Collider shape which will perform damage
	public GameObject m_colliderObject;

	// Animator which will control animation playback
	public Animator m_weaponAnimator;
}
