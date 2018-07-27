using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp_Base : MonoBehaviour {
	// Init launch velocity power (vertical power from source height)
	public float m_launchPowerVer = 10;
	// Init launch velocity power (lateral power from source)
	public float m_launchPowerHor = 10;

	public float m_mass = .5f;

	// Hovering frequency and amplitude
	public float m_hoverFreq = 5;
	public float m_hoverAmp = 5;

	// Spinning frequency
	public float m_spinFreq = 5;

	private float m_hoverTimer = 0;

	public int m_pickUpType = 0;

	// Visual heart for animation
	public GameObject m_visuals;
	public GameObject m_shadows;

	// Launch by default on start, can be turned off
	public bool m_shouldDropLaunch = true;

	protected GameObject m_playerReference;


	// Launch bool controller
	private bool m_isLaunching = true;
	// Launch coroutine while running or null
	private IEnumerator m_launchRoutine;
	// current launch velocity
	private Vector3 m_velocity;

	private float m_landHeight;
	private Vector3 m_lastVisualsPos;

	// Use this for initialization
	protected virtual void Start () {
		m_playerReference = GameObject.FindGameObjectWithTag ("Player");


		m_visuals.GetComponent<Renderer> ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		m_visuals.GetComponent<Renderer> ().receiveShadows = false;

		// Launch or not?
		if (m_shouldDropLaunch) {

			// Set the initial velocity for the pickup
			m_velocity = new Vector3 (Random.Range(-1, 2), 0,Random.Range(-1, 2)).normalized;

			m_velocity = new Vector3 (m_velocity.x * m_launchPowerHor, m_launchPowerVer, m_velocity.z * m_launchPowerHor);

			// Launch
			DropLaunch ();
		} else {
			m_isLaunching = false;
		}

		// Initial vertical offset of visuals to hover (for land raycast)
		m_landHeight = Mathf.Abs (transform.position.y - m_visuals.transform.position.y);
	}

	// Update is called once per frame
	void Update () {
		if (!m_isLaunching) {
			// Do the hovering (subtract so it initially moves down, after landing from launch!)
			m_visuals.transform.position -= new Vector3 (0, Mathf.Sin (Mathf.Deg2Rad * m_hoverTimer * m_hoverFreq) * m_hoverAmp, 0);

			m_visuals.transform.rotation = Quaternion.Euler (0, m_hoverTimer * m_spinFreq, 0);

			m_hoverTimer += Time.deltaTime;

			// Lossy method of loop
			if (m_hoverTimer > 360) {
				m_hoverTimer = 0;
			}
		}
	}

	/// <summary>
	/// Launches as the object is instantiated
	/// </summary>
	public void DropLaunch (){
		// Launch if not already
		if (m_launchRoutine == null) {
			m_lastVisualsPos = m_visuals.transform.position;

			m_launchRoutine = Launch ();

			StartCoroutine (m_launchRoutine);
		}
	}

	IEnumerator Launch(){
		// While I'm launching
		while (m_isLaunching) {

			Vector3 newPos = m_visuals.transform.position + m_velocity * Time.deltaTime;

			Vector3 rayDir = newPos - m_lastVisualsPos;

			Debug.DrawRay (m_visuals.transform.position, rayDir, Color.red);

			RaycastHit hit;

			if (Physics.Raycast (m_visuals.transform.position - new Vector3(0, m_landHeight, 0), rayDir, out hit, rayDir.magnitude, MovementLibrary.m_ground_layerMask)) {
				// LANDED
				m_shadows.SetActive (true);

				m_isLaunching = false;
				transform.position = hit.point;
				m_visuals.transform.position = transform.position + new Vector3 (0, m_landHeight, 0);

			} else {
				m_shadows.SetActive (false);

				// translate
				m_visuals.transform.position = newPos;

				// Mod the velocity
				m_velocity -= new Vector3 (0, 9.81f * m_mass * Time.deltaTime, 0);

				m_lastVisualsPos = m_visuals.transform.position;
			}
			yield return null;
		}

		yield return null;
	}

	/// <summary>
	/// Raises the trigger enter event.
	/// (Player touch, clean up object, derrived 
	/// classes will handle target effects)
	/// </summary>
	/// <param name="other">Other.</param>
	protected virtual void OnTriggerEnter(Collider other){
		if (other.tag == "Player") {
			Destroy (gameObject);
		}
	}
}
