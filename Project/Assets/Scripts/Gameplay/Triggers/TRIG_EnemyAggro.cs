using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRIG_EnemyAggro : MonoBehaviour {

	public GameObject[] m_enemiesToAggro;

	void OnTriggerEnter(Collider other){
		if (other.tag == "Player") {
			for (int i = 0; i < m_enemiesToAggro.Length; ++i) {
				m_enemiesToAggro [i].GetComponent<E_AI> ().Aggro ();
			}
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
