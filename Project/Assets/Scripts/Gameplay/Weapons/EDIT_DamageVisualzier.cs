using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(DamageCollider))]
public class EDIT_DamageVisualzier : MonoBehaviour {
	public DamageCollider m_colliderScript;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (m_colliderScript.A != null && m_colliderScript.B != null && m_colliderScript != null) {
			Debug.DrawLine (
				m_colliderScript.A.transform.position + (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) - (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				m_colliderScript.B.transform.position + (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) - (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				Color.gray
			);
			Debug.DrawLine (
				m_colliderScript.A.transform.position - (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) + (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				m_colliderScript.B.transform.position - (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) + (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				Color.gray
			);
			Debug.DrawLine (
				m_colliderScript.A.transform.position + (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) + (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				m_colliderScript.B.transform.position + (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) + (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				Color.gray
			);
			Debug.DrawLine (
				m_colliderScript.A.transform.position - (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) - (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				m_colliderScript.B.transform.position - (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) - (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				Color.gray
			);


			Debug.DrawLine (
				m_colliderScript.A.transform.position - (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) - (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				m_colliderScript.A.transform.position + (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) - (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				Color.green
			);
			Debug.DrawLine (
				m_colliderScript.A.transform.position - (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) + (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				m_colliderScript.A.transform.position + (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) + (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				Color.green
			);

			Debug.DrawLine (
				m_colliderScript.A.transform.position + (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) + (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				m_colliderScript.A.transform.position + (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) - (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				Color.green
			);
			Debug.DrawLine (
				m_colliderScript.A.transform.position - (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) + (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				m_colliderScript.A.transform.position - (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) - (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				Color.green
			);


			Debug.DrawLine (
				m_colliderScript.B.transform.position - (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) - (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				m_colliderScript.B.transform.position + (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) - (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				Color.red
			);
			Debug.DrawLine (
				m_colliderScript.B.transform.position - (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) + (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				m_colliderScript.B.transform.position + (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) + (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				Color.red
			);

			Debug.DrawLine (
				m_colliderScript.B.transform.position + (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) + (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				m_colliderScript.B.transform.position + (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) - (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				Color.red
			);
			Debug.DrawLine (
				m_colliderScript.B.transform.position - (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) + (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				m_colliderScript.B.transform.position - (m_colliderScript.A.transform.up * (m_colliderScript.m_height / 2)) - (m_colliderScript.A.transform.right * (m_colliderScript.m_width / 2)), 
				Color.red
			);
		}
	}
}
