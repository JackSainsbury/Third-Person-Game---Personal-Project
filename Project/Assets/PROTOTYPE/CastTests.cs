using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastTests : MonoBehaviour {

	public GameObject m_sphereVisStart;
	public GameObject m_sphereVisTarget;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;

		if(Physics.SphereCast(m_sphereVisStart.transform.position, .5f, Vector3.down, out hit, 5)){
			Debug.DrawLine (hit.point, hit.point + hit.normal, Color.red);

			m_sphereVisTarget.transform.position = hit.point + (hit.normal * .5f);
		}
	}
}
