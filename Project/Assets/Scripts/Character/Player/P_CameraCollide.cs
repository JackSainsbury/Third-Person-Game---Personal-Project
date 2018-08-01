using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_CameraCollide : MonoBehaviour {

    public GameObject m_cameraRoot;

    private float m_maxZoom = 3f;

    private float m_zoomVal;

	// Use this for initialization
	void Start () {
        m_zoomVal = (transform.position - m_cameraRoot.transform.position).magnitude;
        m_maxZoom = m_zoomVal;
    }
	
	// Update is called once per frame
	void Update () {
        /*
        RaycastHit hit;

        Vector3 dir = transform.position - m_cameraRoot.transform.position;

        if (Physics.Raycast(m_cameraRoot.transform.position + (dir * m_maxZoom), dir, out hit, (dir.magnitude - m_maxZoom), MovementLibrary.m_ground_layerMask))
        {
            Debug.DrawLine(hit.point - new Vector3(0.3f, 0, 0), hit.point + new Vector3(0.3f, 0, 0));
            Debug.DrawLine(hit.point - new Vector3(0, 0.3f, 0), hit.point + new Vector3(0, 0.3f, 0));
            Debug.DrawLine(hit.point - new Vector3(0, 0, 0.3f), hit.point + new Vector3(0, 0, 0.3f));

            // Get the zoom value to move to
            m_zoomVal = (m_cameraRoot.transform.position - hit.point).magnitude;
            transform.position = dir * m_zoomVal;
        }
        */
	}
}
