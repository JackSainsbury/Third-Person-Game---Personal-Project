using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sword_Swing", menuName = "Combat/Sword_Swing", order = 1)]
public class SO_SwordSwing : ScriptableObject {
	// Delay after pressing butt, before collider is instanced.
	public float m_swingDelay = 0;
	// Time after the delay, for which the collider remains active.
	public float m_swingTime = .2f;
	// Time after swing has been deactivated, before can attack will set back to true.
	public float m_swingRecovery = .25f;
	// Time after interrupted swing (at any point), before can attack will be set back to true.
	public float m_swingInterrupt = .25f;

	// How much damage to do
	public int m_damageTicks = 3;
}
 