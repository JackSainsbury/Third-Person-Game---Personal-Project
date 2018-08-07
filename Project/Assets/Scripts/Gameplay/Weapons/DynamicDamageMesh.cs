using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicDamageMesh : MonoBehaviour {

	private C_Attack m_attackScript;

	private DamageCollider m_damagerColliderScript;

	private int m_damageTicksToDeal;

	public void InitDynaMesh(C_Attack attackScript, DamageCollider damageColliderScript, int damageToDeal){
		m_attackScript = attackScript;
		m_damagerColliderScript = damageColliderScript;
		m_damageTicksToDeal = damageToDeal;
	}

	void OnTriggerEnter(Collider other){
        if (other.tag == "SwingInterrupt") {
			// I hit a wall
			m_attackScript.InterruptAttack();
		}else if (other.tag == "Destructable") {
			// I hit a ground object
			Env_Destructable destructScript = other.GetComponent<Env_Destructable> ();

			// Both enemies and player can destroy destructables (if enemy is allowed to)
			if (m_damagerColliderScript.PlayerOwnership == true || (m_damagerColliderScript.PlayerOwnership == false && destructScript.m_canEnemyDestroy)) {
				destructScript.Attacked ((other.transform.position - m_damagerColliderScript.transform.position).normalized);
			}
		} else {
			// If owned by the player
			if (m_damagerColliderScript.PlayerOwnership == true) {
				// Owner is player

				if (other.tag == "Enemy") {
                    if (m_damagerColliderScript.CheckTarget (other)) {
						other.GetComponent<C_Health> ().ModHealth (-m_damageTicksToDeal);
					}
				}
			} else {
				// Owner is enemy

				if (other.tag == "Player") {
					if (m_damagerColliderScript.CheckTarget (other)) {
						other.GetComponent<P_Health> ().ModHealth (-m_damageTicksToDeal);
					}
				}
			}
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
