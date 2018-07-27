using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(E_Health)), RequireComponent(typeof(E_Attack))]
public class E_AI : MonoBehaviour {
	// Range for enemy to stop and perform attack
	public float m_enemyStopRange = 5;

	// Range for enemy to aggro at
	public float m_enemyAggroRange = 25;

	// Colour this to show the state
	public GameObject m_actionIdentifier;


	// Reference to my nav agent
	private NavMeshAgent m_agent;

	// Reference Enemy Health script
	private E_Health m_health;
	// Reference Enemy Health script
	private E_Attack m_attack;

	// Reference to the player object
	private GameObject m_playerObject;

	// Reference to the active ai coroutine
	private IEnumerator m_aiCoRoutine;

	// State machine
	private int m_AI_State = 0;

	public void Aggro () {
		m_AI_State = 1;
	}

	// Use this for initialization
	void Start () {
		// Get agent reference
		m_agent = GetComponent <NavMeshAgent> ();
		// Get health reference
		m_health = GetComponent <E_Health> ();
		// Get the attack reference
		m_attack = GetComponent <E_Attack> ();

		// Get player instance
		m_playerObject = GameObject.FindGameObjectWithTag ("Player");

		m_aiCoRoutine = FollowTarget (m_enemyStopRange, m_playerObject.transform);
		StartCoroutine(m_aiCoRoutine);
	}
	
	// Update is called once per frame
	void Update () {

	}

	private IEnumerator FollowTarget(float range, Transform target) {
		Vector3 previousTargetPosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity);
		while (m_health.isAlive){
			switch(m_AI_State){
			case 0:
				m_actionIdentifier.GetComponent<Renderer> ().material.color = Color.white;

				// I am outside of the player range, I should move to
				if (Vector3.SqrMagnitude (transform.position - target.position) <= m_enemyAggroRange) {
					// Aggro the player
					m_AI_State = 1;
				}
				break;
			case 1:
				m_actionIdentifier.GetComponent<Renderer> ().material.color = Color.red;

				float squarDistToPlayer = Vector3.SqrMagnitude (transform.position - target.position);

				// I am outside of the player range, I should move to
				if (squarDistToPlayer > range || Vector3.Dot ((target.position - transform.position).normalized, transform.forward) < .75f) {

					// did target move more than at least a minimum amount since last destination set?
					if (Vector3.SqrMagnitude (previousTargetPosition - target.position) > 0.1f) {
						m_agent.SetDestination (target.position);
						previousTargetPosition = target.position;
					}
				} else {

					if (squarDistToPlayer < 3f) {
						transform.position -= transform.forward;
					}
					
					// Pause the state machine
					m_AI_State = 2;

					// Start the attack
					m_attack.TryAttack ();

					// I have reached the range, stop moving
					m_agent.SetDestination (transform.position);
				}
				 
				break;
			case 2:
				m_actionIdentifier.GetComponent<Renderer> ().material.color = Color.blue;

				// Wait for swing to finish
				yield return new WaitForSeconds (
					m_attack.EquipedWeaponScript.m_swingObject.m_swingDelay + 
					m_attack.EquipedWeaponScript.m_swingObject.m_swingTime + 
					m_attack.EquipedWeaponScript.m_swingObject.m_swingRecovery
				);

				m_AI_State = 1;

				//transform.LookAt (new Vector3(m_playerObject.transform.position.x, transform.position.y, m_playerObject.transform.position.z));

				break;
			case 3:
				break;
			case 4:
				break;
			}

			yield return new WaitForSeconds (0.1f);
		}
		// Drop out, I have died
		yield return null;
	}
}
