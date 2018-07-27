using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour {

	public GameObject m_headBone;
	public GameObject m_neckBone;

	public float m_lookAtPlayerSpeed = 25;
	public float m_returnLookToAnimSpeed = .5f;

	// How many units to look (Above) the target (basic offsetter)
	public float m_lookAboveTargetOffset = 1;

	[SerializeField]
	// The target I should be looking at
	private Transform m_target;

	private Quaternion m_neckRestRot;
	private Quaternion m_headRestRot;

	// Current rotations in LATE UPDATE
	private Quaternion m_neckRot;
	private Quaternion m_headRot;

	// Exit rotations
	private Quaternion m_neckExitRot;
	private Quaternion m_headExitRot;

	// Heavy float counter
	private float m_resetLookTimer = 0;
	private bool m_isResetting = false;

	// Store reference to the return view coroutine
	private IEnumerator m_lookBackCoroutine;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void LateUpdate () {
		// Rotations will initially come in, in their non animated state (animation rest)
		m_neckRestRot = m_neckBone.transform.rotation;
		m_headRestRot = m_headBone.transform.rotation;

		Vector3 targetPosMod = m_target.position + new Vector3(0, m_lookAboveTargetOffset, 0);

		// Now check if we should be looking at the player
		Vector3 lookVector = (targetPosMod - transform.position);

		float dist = Vector3.SqrMagnitude (lookVector);
		float howInfront = Vector3.Dot (transform.forward, lookVector.normalized);

		if (dist < 30 && howInfront > 0) {
			Quaternion neck_Look = Quaternion.LookRotation (targetPosMod - m_neckBone.transform.position) * Quaternion.Euler(0, 0, -90);
			Quaternion head_Look = Quaternion.LookRotation (targetPosMod - m_headBone.transform.position) * Quaternion.Euler(0, 0, -90);

			// Perform the lerp
			m_neckBone.transform.rotation = Quaternion.Lerp (m_neckRot, neck_Look, Time.deltaTime * m_lookAtPlayerSpeed);
			// Perform the lerp
			m_headBone.transform.rotation = Quaternion.Lerp (m_headRot, head_Look, Time.deltaTime * m_lookAtPlayerSpeed);

			m_neckExitRot = m_neckRot;
			m_headExitRot = m_headRot;

			// Needs resetting, possible re-entry?
			m_isResetting = true;
			m_resetLookTimer = 0;
		} else {
			if (m_isResetting) {
				if (m_resetLookTimer <= 1) {
					// Perform the lerp
					m_neckBone.transform.rotation = Quaternion.Lerp (m_neckExitRot, m_neckRestRot, m_resetLookTimer);
					// Perform the lerp
					m_headBone.transform.rotation = Quaternion.Lerp (m_headExitRot, m_headRestRot, m_resetLookTimer);

					// Increment our timer
					m_resetLookTimer += Time.deltaTime * m_returnLookToAnimSpeed;
				} else {
					// End by self
					m_isResetting = false;
					m_resetLookTimer = 0;
				}
			}
		}

		// Store the rotation for entering the lerp towards state
		m_neckRot = m_neckBone.transform.rotation;
		m_headRot = m_headBone.transform.rotation;
	}


	// Property for setting a target
	public Transform Target{
		get{
			return m_target;
		}
		set{
			m_target = value;
		}
	}
}
