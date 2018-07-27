using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PU_Heart : PickUp_Base {
	// How much health should the hearts heal
	public int m_ticksToHeal = 5;

	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}


	// Override the trigger event
	protected override void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player") {
			// Reference to the health script
			C_Health playerHealth_scriptRef = m_playerReference.GetComponent<C_Health> ();

			// Player is alive AND damaged
			if (playerHealth_scriptRef.isAlive && playerHealth_scriptRef.GetCurHealth () != playerHealth_scriptRef.GetMaxHealth ()) {
				// Heart Object specific logic (healing)
				playerHealth_scriptRef.ModHealth (m_ticksToHeal);

				// General cleanup etc (same for all pickups)
				base.OnTriggerEnter (other);
			}
		}
	}
}
