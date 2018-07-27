using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Env_Dest_JugPot : Env_Dest_LootTable {

	public ParticleSystem m_shardsParticles;

	// One shot the particles
	public override void Attacked(Vector3 attackVector){
		//The destructable object has been attacked
		base.Attacked(attackVector);

		// Aim the particles towards the attack vector (and up a bit)
		m_shardsParticles.gameObject.transform.LookAt (m_shardsParticles.gameObject.transform.position + attackVector.normalized + new Vector3(0, 2, 0));
		m_shardsParticles.Play ();

		Destroy (GetComponent<MeshRenderer> ());
		gameObject.tag = "Untagged";
	}
}
