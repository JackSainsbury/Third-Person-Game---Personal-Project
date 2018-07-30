using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Attack : MonoBehaviour {

	// The dash speed for an attack
	public float m_attackDashSpeed = 25;
	// Aniamtor for the sword, passed a controller
	public GameObject m_TEMP_SWORD;
	// Root for the hand position (temporary till skeleton)
	public GameObject m_handObject;


	// The currently equiped weapon in hand
	protected GameObject m_equipedWeapon;
	// The weapon script for the eqipedWeapon
	protected EQ_Weapon m_eqipedWeaponScript;
	// The current (active) attack coroutine, if null, not attacking
	protected IEnumerator m_attackCoRoutine;
	// Can the player currently attack and enemy
	protected bool m_isAllowedToAttack = true;
	// If this GameObject is null then there is no attack being performed
	protected GameObject m_swingColliderObject;

	// Is this a player character
	[SerializeField]
	protected bool m_isPlayer = false;
	// The duration of the currently playing animation
	protected float m_curAnimDuration = 0;


	// Hash the animator parameter
	private int animStateParameter;
	// Reference to my animator
	private Animator m_swordAnimator;

	protected virtual void Start(){
		// Store the parameter as a hashed reference (string calls are slow so do once)
		animStateParameter = Animator.StringToHash ("AnimState");

		// Serialized field might already have a sword selected
		if (m_TEMP_SWORD != null) {
			EquipWeapon (m_TEMP_SWORD);
		}
	}

	// Send a weapon to the script, running initialization, placing it in the player's hand and prepping all references
	public void EquipWeapon(GameObject weaponToEquip) {
		// Unload the last weapon
		if (m_equipedWeapon != null) {
			m_eqipedWeaponScript.m_colliderObject.GetComponent<DamageCollider> ().UnEquip ();
			Destroy (m_equipedWeapon);
		}

		// Instance the weapon prefab
		m_equipedWeapon = Instantiate (weaponToEquip, m_handObject.transform);

		m_equipedWeapon.transform.position = m_handObject.transform.position;
		m_equipedWeapon.transform.rotation = m_handObject.transform.rotation;

		// Load new weapon
		m_eqipedWeaponScript = m_equipedWeapon.GetComponent<EQ_Weapon>();
		m_swingColliderObject = m_eqipedWeaponScript.m_colliderObject;
		// Get the animator for this weapon
		m_swordAnimator = m_eqipedWeaponScript.m_weaponAnimator;

		// Pass damage to deal to the new collider and perform the set up accordingly
		m_swingColliderObject.GetComponent<DamageCollider> ().InitCollider (m_eqipedWeaponScript.m_swingObject.m_damageTicks, this);
		// Prep collider for correct targets to deal damage to
		m_swingColliderObject.GetComponent<DamageCollider>().PlayerOwnership = m_isPlayer;

	}

	public virtual bool TryAttack(){
		if (m_isAllowedToAttack) {

			// Store reference to the coroutine we're starting, so that we can interrupt it later
			m_attackCoRoutine = SwingSword (m_eqipedWeaponScript);
			// Begin the swing
			StartCoroutine (m_attackCoRoutine);

			return true;
		}
		return false;

	}

	public void InterruptAttack(){
		// Stop the swingSword coroutine
		StopCoroutine (m_attackCoRoutine);

		// Begin interrupt coroutine
		m_attackCoRoutine = InterruptSword (m_eqipedWeaponScript);
		StartCoroutine (m_attackCoRoutine);
	}

	protected IEnumerator InterruptSword (EQ_Weapon equipedWeapon) {
		// Disable the collider, as we have interrupted the swing
		m_swingColliderObject.GetComponent<DamageCollider> ().EndSwing ();

		// Set the param to interrupted state, and the animation length
		m_swordAnimator.SetInteger (animStateParameter, 4);
		m_curAnimDuration = equipedWeapon.m_swingObject.m_swingInterrupt;

		// DO INTERRUPT ANIMATION
		yield return new WaitForSeconds (equipedWeapon.m_swingObject.m_swingInterrupt);

		m_swordAnimator.SetInteger (animStateParameter, 3);

		// Allow for another attack now we're reset
		m_isAllowedToAttack = true;
	}

	// CoRoutine to swing sword, perform an attack
	protected IEnumerator SwingSword (EQ_Weapon equipedWeapon) {
		// Whilst coroutine is performing, prevent further attacks being queued
		m_isAllowedToAttack = false;

		// Swing is currently not allowed to start yet, Play charge up animations and fx etc
		m_swordAnimator.SetInteger (animStateParameter, 0);
		m_curAnimDuration = equipedWeapon.m_swingObject.m_swingDelay;
		yield return new WaitForSeconds (equipedWeapon.m_swingObject.m_swingDelay);

		// Swing is currently being performed, enable the collider
		m_swingColliderObject.SetActive (true);
		m_swingColliderObject.GetComponent<DamageCollider> ().BeginSwing ();

		// Flag animations
		m_swordAnimator.SetInteger (animStateParameter, 1);
		m_curAnimDuration = equipedWeapon.m_swingObject.m_swingTime;

		yield return new WaitForSeconds (equipedWeapon.m_swingObject.m_swingTime);
		// Swing has finished, but is within the recovery time, do recovery animations and fx etc
		// Clean up collider
		if (m_swingColliderObject != null) {
            // End swing will also SetActive m_swingColliderObject(false) internally
			m_swingColliderObject.GetComponent<DamageCollider> ().EndSwing ();
		}
			
		m_swordAnimator.SetInteger (animStateParameter, 2);
		m_curAnimDuration = equipedWeapon.m_swingObject.m_swingRecovery;
		yield return new WaitForSeconds (equipedWeapon.m_swingObject.m_swingRecovery);

		m_isAllowedToAttack = true;
		m_swordAnimator.SetInteger (animStateParameter, 3);
		m_curAnimDuration = 0;
		yield return null;
	}

	public EQ_Weapon EquipedWeaponScript {
		get {
			return m_eqipedWeaponScript;
		}
		set {
			m_eqipedWeaponScript = value;
		}
	}

	public float CurAnimDur {
		get {
			return m_curAnimDuration;
		}
	}
}
