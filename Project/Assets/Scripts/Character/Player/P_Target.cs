using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(P_Move))]
public class P_Target : MonoBehaviour {

	public GameObject m_tempTarget;

	private P_Move m_playerMoveScript;

	private GameObject[] m_enemies;

	private bool singleHold = false;

	// Use this for initialization
	void Start () {
		m_playerMoveScript = GetComponent<P_Move> ();

		ReShuffleEnemies ();
	}
	
	// Update is called once per frame
	void Update () {
		// Input triggers are 0 -> 1 for left and 0 -> -1 for right
		if (Input.GetAxis ("InputTriggers") > 0.05f) {
			if (!singleHold) {
				if (m_playerMoveScript.GetCameraLocked ()) {
					m_playerMoveScript.SetCameraLocked ();
				} else {
					if (m_enemies.Length > 0) {
						int shortestIndex = 0;

						float shortestLength = Vector3.Distance (transform.position, m_enemies [shortestIndex].transform.position);

						for (int i = 0; i < m_enemies.Length; ++i) {
							float candidatelength = Vector3.Distance (transform.position, m_enemies [i].transform.position);

							if (candidatelength < shortestLength) {
								shortestIndex = i;
								shortestLength = candidatelength;
							}
						}

						m_playerMoveScript.SetCameraLocked (m_enemies [shortestIndex]);
					}
				}

				singleHold = true;
			}
		} else {
			//Released the trigger
			singleHold = false;
		}
	}

	public void ReShuffleEnemies(){
		m_enemies = GameObject.FindGameObjectsWithTag ("Enemy");
	}
}
