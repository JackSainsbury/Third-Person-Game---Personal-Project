using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Env_Dest_Bush : Env_Dest_LootTable {

	public SkinMove m_skinner;

	// Shrink the object
	private float m_shrinkTimer = 1f;

	private float m_shrinkAfter = 0;

	public override void Attacked(Vector3 attackVector){
		// Set a shrink time (base value + random offset)
		m_shrinkAfter = m_destroyAfterSeconds + Random.Range (.1f, .5f);
		// + 1 for full destroy after in base class
		m_destroyAfterSeconds = m_shrinkAfter + 1;

		//The destructable object has been attacked (now call coroutine and wait for seoconds in base)
		base.Attacked(attackVector);

		// Stop skinning now false
		m_skinner.IsSkinned = false;

		StartCoroutine (Shrink());

		GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
		GetComponent<Rigidbody> ().useGravity = true;
		GetComponent<Rigidbody> ().AddForce (attackVector * 250);
	}

	IEnumerator Shrink(){
		// Wait for shrink
		yield return new WaitForSeconds (m_shrinkAfter);
		// Lerp for 1 (shrink + 1 = full destroy time in base)
		while (m_shrinkTimer > 0f) {
			m_shrinkTimer -= Time.deltaTime;

			// Do the shrink
			gameObject.transform.localScale = new Vector3 (m_shrinkTimer, m_shrinkTimer, m_shrinkTimer);
			yield return null;
		}
			
		yield return null;
	}
}
