using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control_Tests : MonoBehaviour {

	private Vector3 m_velocity;
	private float m_mass = 2;
	private float m_gravityScale = -9.81f;

	private bool m_isGrounded = false;

	private Vector3 m_currentGroundNormal;

	// Use this for initialization
	void Start () {
		m_velocity = Vector3.zero;
	}

	//Gravity and grounding
	void GroundPlayer(){
		if (!m_isGrounded) {
			m_velocity = new Vector3 (m_velocity.x, m_velocity.y + (m_gravityScale * m_mass * Time.deltaTime), m_velocity.z);

			Vector3 velocityCache = m_velocity * Time.deltaTime;

			RaycastHit hit;

			TryMove (velocityCache);

			if (Physics.SphereCast (transform.position + new Vector3 (0, 1f, 0), .5f, Vector3.down, out hit, 1.1f, MovementLibrary.m_ground_layerMask)) {
				// Move player to hit point ( perform landing )
				transform.position = hit.point + (hit.normal * .5f) + (Vector3.down * .5f);

				m_currentGroundNormal = hit.normal;

				velocityCache = Vector3.zero;

				m_isGrounded = true;
			} else {
				// Move by velocity amount vertically, no matter what, as I am airborne
				transform.Translate ( new Vector3(0, velocityCache.y, 0));
			}
		} else {
			//Player is gounded

			RaycastHit hit;

			// Is there ground beneath me?
			if (Physics.SphereCast (transform.position + new Vector3 (0, 1f, 0), .5f, Vector3.down, out hit, 1.1f, MovementLibrary.m_ground_layerMask)) {
				float normalSlope = Vector3.Dot (hit.normal, Vector3.up);

				// Slope is walkable
				if (normalSlope > .5f) {
					m_currentGroundNormal = hit.normal;
				} else {
					//Slope is wall, find ground normal

					// Cast a ray down from the wall hit (assumes there is a floor somewhere below us
					Vector3 rayDir = Vector3.Cross (Vector3.Cross (hit.normal, Vector3.down), hit.normal);

					if(Physics.Raycast(hit.point, rayDir, out hit, Mathf.Infinity, MovementLibrary.m_ground_layerMask)){
						m_currentGroundNormal = hit.normal;
					}
				}
			} else {
				m_isGrounded = false;

				// Was grounded, I am now not, zero out velocity initially ( extra safe )
				m_velocity = Vector3.zero;
			}
		}
	}

	// Try to move player
	void TryMove(Vector3 movement, int Depth = 0){

		RaycastHit hit;

		// Get local right vector of player movement
		Vector3 localRightAxis = Vector3.Cross (movement.normalized, -Vector3.up);

		// Cache ground normal for later dash use

		float magnitude = movement.magnitude;
		Vector3 groundedMovementPlayer = Vector3.Cross (localRightAxis, m_currentGroundNormal) * magnitude;

		// Check if I hit something (slightly smaller capsule cast to avoid hitting floor)
		if (Physics.CapsuleCast (transform.position + new Vector3 (0, 1.5f, 0), transform.position + new Vector3 (0, 0.5f, 0), 0.475f, movement, out hit, movement.magnitude, MovementLibrary.m_ground_layerMask)) {

			// I have hit something
			if (Depth < 1) {
				//RaycastHit floorHit;

				// Check wall
				// Work towards the local right vector
				Vector3 slideVector_Local = Vector3.Cross (hit.normal, m_currentGroundNormal);
				Vector3 slide_World = Vector3.Cross (hit.normal, Vector3.up);

				// How aligned to the slide world vector is my input, and therefore how much slide should be applied
				float slideColinearity = Vector3.Dot (groundedMovementPlayer, slide_World);

				// How 'slidey' should we be based on how we're trying to move along the wall (less so if we're trying to move directly into the wall, more so if parralel to)
				slideVector_Local *= slideColinearity;

				TryMove (slideVector_Local, Depth + 1);
			}
		} else {
			// I have not hit something
			transform.Translate (groundedMovementPlayer);
		}
	}
	
	// Update is called once per frame
	void Update () {
		GroundPlayer ();

		Vector3 moveVector = (transform.forward + transform.right) * Time.deltaTime * 1.5f;
		TryMove (moveVector);
	}
}
