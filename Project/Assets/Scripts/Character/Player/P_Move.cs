using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum for space relative movement
public enum CharacterSpace{
	Player,
	Camera,
	World
}

[RequireComponent(typeof(CapsuleCollider))]
public class P_Move : MonoBehaviour {


	// Speed when moving normally
	public float m_moveSpeed = 5;

	// Force forwards whilst jumping
	public float m_jumpForwardForce = 2;
	// Force upwards whilst jumping 
	public float m_jumpVeritcalForce = 2;
	// When falling off a ledge, how much should we boost the raw input velocity by
	public float m_maintainVelocityOnFallMultiplier = 20;

	// Time in which to perform the dash
	public float m_dashTime = 1;

	// Speed when stepping up or down
	public float m_stepSpeed = 5;
	public float m_maxStepHeight = .5f;

	// Base rotation of player with left stick speed
	public float m_rotSpeed = 0.05f;
	// Rotation of player to locked look at enemy speed;
	public float m_rotLockSpeed = 0.005f;

	// Rotate with right stick speed
	public float m_cameraRotSpeed = 5;
	// Move to player (catch up) speed
	public float m_cameraMoveSpeed = 5;

	// Zoom to locked mode speed
	public float m_cameraLockZoomSpeed = 5;

	public float m_orthoBaseScale = 20;
	public float m_orthoLockMod = -5;

	// Radius for capsule collider and ground check movement
	public float m_playerColliderRadius = 0.5f;
	public float m_playerHeight = 2;

	// Anchor for the camera to attach to
	public GameObject m_cameraAnchor;



	// Currently dashing, is able to dash, the timer for controlling their resets and the dash vector, during a dash
	private bool m_canDash = true;
	// Can the player currently walk?
	private bool m_canWalk = true;
	// Is the player currently on the ground?
	private bool m_isGrounded = true;

	// Current acceleration of the player due to gravity
	private Vector3 m_fallVelocity = Vector3.zero;

	// Private but serializable field for character mass
	[SerializeField]
	private float m_characterMass = 3;

	private float m_dashResetTimer = 0;

	// Camera lock state
	private bool m_cameraLocked = false;



	// The target transform to look at when locked on
	private Transform m_targetTransform;

	// Store reference to the currently running coroutine
	private IEnumerator m_dashCoRoutine;

	private Vector3 m_currentGroundNormal;
	private float m_currentYStepTaretDist;

	private Vector3 OLD_POS_TEST = Vector3.zero;
	private Vector3 NEW_POS_TEST = Vector3.zero;



	// Use this for initialization
	void Start () {
		GetComponent<CapsuleCollider> ().radius = m_playerColliderRadius;

		m_cameraAnchor.transform.SetParent (null);
	}

	// Update is called once per frame
	void Update () {
		// Left joystick
		float verL = Input.GetAxis ("VerticalL");
		float horL = Input.GetAxis ("HorizontalL");

		// Right joystick
		float verR = Input.GetAxis ("VerticalR");
		float horR = Input.GetAxis ("HorizontalR");

		// Input axis as unit vector
		Vector3 movementRawL = new Vector3 (horL, 0, verL);
		// Input right
		Vector3 movementRawR = new Vector3 (horR, 0, verR);

		MovementLibrary.PlayerDropGravity (this);

		// General Dash
		if (Input.GetButtonDown("B_Button")) {
			Dash (new Vector2 (0, 1), m_dashTime, .1f, 2.5f, CharacterSpace.Player, false);
		}

		if (m_canWalk) {
			if (verL != 0 || horL != 0) {
				// Try and move the player
				MovementLibrary.MovePlayer (this, movementRawL, m_moveSpeed);
			}
		}

		if (m_cameraAnchor.transform.position != transform.position) {
			// Move camera to player position
			PositionCamera ();
		}

		if(Input.GetButtonDown("A_Button") && IsGrounded){
			Jump (new Vector3(transform.forward.x * m_jumpForwardForce, m_jumpVeritcalForce, transform.forward.z * m_jumpForwardForce));
		}

		// Set player and camera rot
		OrientPlayerAndCamera (movementRawL, movementRawR);

		Debug.DrawLine (OLD_POS_TEST, NEW_POS_TEST);
	}
		

	// Methods
	public void Jump(Vector3 initialFallVelocity){
		// Prevent walk during jump
		m_canWalk = false;
		m_isGrounded = false;
		CanDash = false;

		// Set the velocity to up initially
		CharVelocity = initialFallVelocity;
	}

	void PositionCamera(){
		// Camera Follow by setting to player position
		m_cameraAnchor.transform.position = Vector3.Lerp(m_cameraAnchor.transform.position, transform.position, Time.deltaTime * m_cameraMoveSpeed * Vector3.Distance(m_cameraAnchor.transform.position, transform.position));
	}

	//-------------------------------------

	// External/Internal dashing function, in a normalized (x,z) direction
	public void Dash(Vector2 movementRawR, float timeToDash, float resetTime, float dashLength, CharacterSpace space = CharacterSpace.Camera, bool overrideCurDash = false){
		// Can I dash or am I forcing a new dash
		if(m_canDash || overrideCurDash) {
			// Hard reset the current dash
			if (overrideCurDash) {
				// We have a running coroutine
				if (m_dashCoRoutine != null) {
					// Make sure we're not instancing a new dash coroutine
					StopCoroutine (m_dashCoRoutine);
				}
			}

			// Otherwise we can assume that the player "can" dash and therefore the dash coroutine is not running, because
			// "m_canDash" should ONLY be set to true from inside the end case of the coroutine

			// Player is dashing, prepare
			m_dashResetTimer = 0;
			m_dashCoRoutine = PerformDash (new Vector3 (movementRawR.x, movementRawR.y), timeToDash, resetTime, dashLength, space);

			StartCoroutine(m_dashCoRoutine);
		}
	}

	IEnumerator PerformDash(Vector2 dashVectorNormalized, float timeToDash, float resetTime, float dashLength, CharacterSpace space = CharacterSpace.Camera){
		// If instant (make appear instant, avoid division by 0)
		if (timeToDash == 0) {
			timeToDash = 0.001f;
		}

		// cache position at the start of this dash
		Vector3 startPos = transform.position;

		// toDash is the target dash based on the dash length and our dash vector (NORMALIZED)
		Vector3 toDash = (new Vector3 (dashVectorNormalized.x, 0, dashVectorNormalized.y) * dashLength);

		// The stepping amount of dash
		Vector3 lastSubDashPos = new Vector3(0, 0, 0);


		// Disallow a dash to begin
		m_canDash = false;

		// Reset for safety
		m_dashResetTimer = 0;

		// Perform dash for the full time
		while(m_dashResetTimer < (timeToDash + resetTime)){
			if (m_dashResetTimer < timeToDash) {
				// Get the sub dash vector (timer based)
				Vector3 thisLerp = Vector3.Lerp (Vector3.zero, toDash, m_dashResetTimer / timeToDash);

				// Actually move the player
				MovementLibrary.MovePlayer (this, thisLerp - lastSubDashPos, space);

				// Set the last position to the current position
				lastSubDashPos = thisLerp;
			}

			m_dashResetTimer += Time.deltaTime;

			yield return null;
		}

		// Allow for re-dashing
		m_canDash = true;

		// Drop out of dash (completed)
		yield return null;
	}


	//-------------------------------------


	void OrientPlayerAndCamera(Vector3 movementRawL, Vector3 movementRawR){
		if (!m_cameraLocked) {
			/*
			 * 
			 * 	Free camera, target is not locked to an enemy
			 * 
			 * 
			 */

			// Rotate the camera root (orbits character)
			m_cameraAnchor.transform.Rotate (0, movementRawR.x * m_cameraRotSpeed, 0);

			// Can I walk currently?
			if (m_canWalk) {
				// Rotate character graphics towards move vector
				if (movementRawL != Vector3.zero) {
					Quaternion targetRot = Quaternion.LookRotation (movementRawL.normalized) * Quaternion.Euler (0, m_cameraAnchor.transform.rotation.eulerAngles.y, 0);

					float angleBetween = Quaternion.Angle (transform.rotation, targetRot);

					angleBetween /= 180f;

					angleBetween = angleBetween < .75f ? .75f : angleBetween;


					// Do the rotation here
					transform.rotation = Quaternion.Lerp (
						transform.rotation, 
						targetRot, 
						Time.deltaTime * angleBetween * m_rotSpeed
					);
				}
			}

			Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, m_orthoBaseScale, Time.deltaTime * Mathf.Abs (Camera.main.fieldOfView - m_orthoBaseScale) * m_cameraLockZoomSpeed);
		} else {
			/*
			 * 
			 * 	Lock camera, target is an enemy, externally parsed
			 * 
			 * 
			*/

			// Target to look at, on the xz plane at my y position
			Vector3 myPlaneTarget = new Vector3 (m_targetTransform.position.x, transform.position.y, m_targetTransform.position.z);

			// Can I walk currently?
			if (m_canWalk) {
				if ((Mathf.Abs (movementRawR.z) > 0 || Mathf.Abs (movementRawR.x) > 0)) {

					Vector2 lockedDash = Vector2.zero;

					// Clamp the axis to 8 directional presence vector
					lockedDash.x = movementRawR.x > 0 ? 1 : (movementRawR.x < 0 ? -1 : 0);
					lockedDash.y = movementRawR.z > 0 ? 1 : (movementRawR.z < 0 ? -1 : 0);

					// Add .5f to the locked dash, giving it a more controlled feel (as you're dashing diagonally across a lock)
					lockedDash = lockedDash.x != 0 ? new Vector2 (lockedDash.x, lockedDash.y + .5f) : lockedDash;

					// Send this new dash vector to the dash function
					Dash (lockedDash, m_dashTime, .1f, 2.5f);
				}


				transform.LookAt (myPlaneTarget);
			}

			Quaternion targetRot = Quaternion.LookRotation (myPlaneTarget - m_cameraAnchor.transform.position);

			float angleBetween = Quaternion.Angle (transform.rotation, targetRot);

			angleBetween /= 180f;
			angleBetween = angleBetween < .5f ? .5f : angleBetween;

			// Do the rotation here
			m_cameraAnchor.transform.rotation = Quaternion.Lerp (
				m_cameraAnchor.transform.rotation, 
				targetRot, 
				Time.deltaTime * angleBetween * m_rotLockSpeed
			);

			Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, (m_orthoBaseScale + m_orthoLockMod), Time.deltaTime * Mathf.Abs (Camera.main.fieldOfView - (m_orthoBaseScale + m_orthoLockMod)) * m_cameraLockZoomSpeed);
		}
	}

	// ------------------------------------ Properties Set & Get ------------------------------------

	/*
	 * Decided against property for the get state, as it implies a read only encapsulation, but also a public set function with explicit functionality
	*/
	// Set the locked state
	public void SetCameraLocked(GameObject m_targetObject = null){
		if (m_targetObject != null) {
			m_cameraLocked = true;
			m_targetTransform = m_targetObject.transform;
		} else {
			m_cameraLocked = false;
		}
	}
	// Get the locked state
	public bool GetCameraLocked(){
		return m_cameraLocked;
	}


	// Is Character Grounded property
	public bool IsGrounded {
		set{
			m_isGrounded = value;
		}
		get {
			return m_isGrounded;
		}
	}

	// Is player allowed to walk
	public bool CanWalk {
		set{
			m_canWalk = value;
		}
		get {
			return m_canWalk;
		}
	}

	// Is player allowed to dash
	public bool CanDash {
		set{
			m_canDash = value;
		}
		get {
			return m_canDash;
		}
	}

	// Current vertical velocity property
	public Vector3 CharVelocity {
		set {
			m_fallVelocity = value;
		}

		get {
			return m_fallVelocity;
		}
	}

	// The ground normal for the curved base of my character controller (currently)
	public Vector3 CurGroundNormal {
		set {
			m_currentGroundNormal = value;
		}

		get {
			return m_currentGroundNormal;
		}
	}

	// CharacterMass property
	public float CharMass {
		set {
			m_characterMass = value;
		}

		get {
			return m_characterMass;
		}
	}
}
