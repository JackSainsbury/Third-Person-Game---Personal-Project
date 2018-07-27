using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Health : C_Health {

	public GameObject m_healthbarInstance;
	public GameObject m_healthBarBack;

	// Use this for initialization
	protected override void Start () {
		base.Start ();

		m_healthbarInstance.transform.localScale = new Vector3 (GetCurHealth () / (float)GetMaxHealth (), 1, 1);
	}

	public void Update(){
		m_healthBarBack.transform.LookAt (Camera.main.transform);
		m_healthBarBack.transform.Rotate (90, 0, 0);
	}

	public override void ModHealth (int value)
	{
		base.ModHealth (value);

		m_healthbarInstance.transform.localScale = new Vector3 (GetCurHealth () / (float)GetMaxHealth (), 1, 1);

		if (value < 0) {
			// Turn on the health bar when damaged
			m_healthBarBack.SetActive (true);
		}

		if (!isAlive) {
			StartCoroutine(killEnemy ());
		}
	}

	// Character has died, perform general character death logic
	IEnumerator killEnemy(){
		tag = "Untagged";
		GameObject.FindGameObjectWithTag ("Player").GetComponent<P_Target> ().ReShuffleEnemies ();
		yield return null;
	}
}
