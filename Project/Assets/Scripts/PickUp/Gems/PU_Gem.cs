using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PU_Gem : PickUp_Base {

	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}

	// Override the trigger event
	protected override void OnTriggerEnter (Collider other)
	{
		// Do something specific when player enters
		if (other.tag == "Player") {
			// Play sound / effects
			other.GetComponent<P_Gems>().ModGems(1);
		}

		// General cleanup etc (same for all pickups)
		base.OnTriggerEnter (other);
	}
}
