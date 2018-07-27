using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Env_Destructable : MonoBehaviour {
	// Should enemies be allowed to destroy this destructable, or just the player
	public bool m_canEnemyDestroy = true;

	[SerializeField]
	protected float m_destroyAfterSeconds = 3;

	// Trigger the destroy
	public virtual void Attacked(Vector3 attackVector){
		StartCoroutine (DestroyAfter());
	}

	// Destroy Destroy after attacked
	IEnumerator DestroyAfter(){
		// Do destroy
		yield return new WaitForSeconds (m_destroyAfterSeconds);

		Destroy (gameObject);

		yield return null;
	}
}
