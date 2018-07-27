using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinMove : MonoBehaviour {
	public GameObject m_skinTarget;

	private List<GameObject> m_skinnedMovers;
	private bool m_isSkinned = true;

	private int m_layerMask;

	// Use this for initialization
	void Start () {
		m_skinnedMovers = new List<GameObject> ();

		m_layerMask = LayerMask.GetMask("GroundCol");
	}

	// Update is called once per frame
	void Update () {
		if (m_isSkinned) {
			Vector3 skinVector = transform.up;

			// Get the directional skinning vector from all interacting bodies
			for (int i = 0; i < m_skinnedMovers.Count; ++i) {
				if (m_skinnedMovers [i] != null) {
					skinVector += (m_skinTarget.transform.position - m_skinnedMovers [i].transform.position);
				} else {
					m_skinnedMovers.Remove (m_skinnedMovers [i]);
				}
			}
				
			Quaternion targetRot = Quaternion.FromToRotation (m_skinTarget.transform.up, new Vector3 (skinVector.x, m_skinTarget.transform.up.y, skinVector.z));

			m_skinTarget.transform.rotation = Quaternion.Lerp (m_skinTarget.transform.rotation, targetRot, Time.deltaTime * 0.1f * Quaternion.Angle (m_skinTarget.transform.rotation, targetRot));
		} else {
			Destroy (GetComponent<Collider>());
			Destroy (this);
		}
	}

	// Skin object has collided with this root object, add to influence list
	void OnTriggerEnter(Collider other){
		if (m_isSkinned) {
			if ((1 << other.gameObject.layer) != m_layerMask && other.gameObject.tag != "Destructable") {
				m_skinnedMovers.Add (other.gameObject);
			}
		}
	}

	// Skin object has left this root object, remove influence
	void OnTriggerExit(Collider other){
		if (m_isSkinned) {
			// Remove other from the list
			m_skinnedMovers.Remove (other.gameObject);
		}
	}

	public bool IsSkinned{
		get 
		{ 
			return m_isSkinned;
		}

		set 
		{ 
			m_isSkinned = value;

			if (!m_isSkinned) {
				m_skinnedMovers.Clear ();
			}
		}
	}
}
