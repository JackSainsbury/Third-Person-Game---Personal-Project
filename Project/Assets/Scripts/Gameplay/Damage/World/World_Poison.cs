using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World_Poison : MonoBehaviour {

	public float m_poisonForSeconds = 2;

	public float m_knockBackSpeed = 30;

	private GameObject m_playerReference;

	// Use this for initialization
	void Start () {
		m_playerReference = GameObject.FindGameObjectWithTag ("Player");
	}

	// Trigger delivers damage to player
	void OnCollisionEnter(Collision other){
		if (other.gameObject.tag == "Player") {
			m_playerReference.GetComponent<C_Health> ().StartCoroutine("SetColours");

			//Contact point[0], just for safety, Should always have a contact point if a collision has occurred
			Vector3 launchVec = (m_playerReference.transform.position - other.contacts[0].point).normalized;

			m_playerReference.GetComponent<P_Move> ().Dash (new Vector2 (launchVec.x, launchVec.z), m_knockBackSpeed, .1f, 2.5f, CharacterSpace.World, true);
		}
	}
}
