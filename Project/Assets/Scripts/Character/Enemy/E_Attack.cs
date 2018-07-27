using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Attack : C_Attack {
	// Use this for initialization
	protected override void Start () {
		base.Start ();

		m_isPlayer = false;
	}

	public override bool TryAttack(){
		bool attacking = base.TryAttack ();

		return attacking;
	}
}
