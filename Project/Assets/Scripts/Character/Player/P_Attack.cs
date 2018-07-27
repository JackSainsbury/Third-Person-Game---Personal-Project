using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Attack : C_Attack {

	// Use this for initialization
	protected override void Start () {
		base.Start ();

		m_isPlayer = true;
	}

	// Update is called once per frame
	void Update () {
		// Do a sword attack
		if (Input.GetButtonDown("X_Button")) {
			TryAttack ();
		}
	}

	public override bool TryAttack(){
		bool attacking = base.TryAttack ();

		if (attacking) {
			// Any player specific attack logic
		}

		return attacking;
	}
}
