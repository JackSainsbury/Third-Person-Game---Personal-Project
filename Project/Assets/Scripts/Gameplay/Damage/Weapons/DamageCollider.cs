using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour {
	// Colliders shape properties
	public float m_width = 1;
	public float m_height = 1;

	public bool m_useDynamicMeshCollider = false;

	//Nodes to describe the collider dimensions
	public GameObject A;
	public GameObject B;

	// The ownership type of this collider
	private bool m_isPlayerOwned = false;

	// List of enemies which I have performed damage on
	private List<GameObject> m_enemiesDamaged;

	// Owning attack component
	private C_Attack m_attackScript;

	// The amount of damage this swing should do if the collider is entered
	private int m_damageTicksToDeal = 0;

	// Vertices that make up this damage collider (box)
	private Vector3[] m_curVerts;
	private Vector3[] m_lastVerts;

	// Container for my sword component
	private GameObject m_colliderContainer;
	private MeshCollider m_collider;

    // First tick of damage enabled, (for every swing)
    private bool m_firstTick = false;
    
    // End the swing on the next frame (avoids last frame of dynamic collider being lost to late update).
    private bool m_flaggedForEndSwing = false;

	void OnTriggerEnter(Collider other){
		if (other.tag == "SwingInterrupt") {
			// I hit a wall
			m_attackScript.InterruptAttack();
		}else if (other.tag == "Destructable") {
			// I hit a ground object
			Env_Destructable destructScript = other.GetComponent<Env_Destructable> ();

			// Both enemies and player can destroy destructables (if enemy is allowed to)
			if (PlayerOwnership == true || (PlayerOwnership == false && destructScript.m_canEnemyDestroy)) {
				destructScript.Attacked ((other.transform.position - transform.position).normalized);
			}
		} else {
			// If owned by the player
			if (PlayerOwnership == true) {
				// Owner is player

				if (other.tag == "Enemy") {
					if (CheckTarget (other)) {
						other.GetComponent<C_Health> ().ModHealth (-m_damageTicksToDeal);
					}
				}
			} else {
				// Owner is enemy

				if (other.tag == "Player") {
					if (CheckTarget (other)) {
						other.GetComponent<P_Health> ().ModHealth (-m_damageTicksToDeal);
					}
				}
			}
		}
	}


	Vector3[] GetBoxPoints(){
		return new Vector3[]{
			A.transform.position + (A.transform.up * (m_height / 2)) - (A.transform.right * (m_width / 2)),
			B.transform.position + (A.transform.up * (m_height / 2)) - (A.transform.right * (m_width / 2)),
			A.transform.position - (A.transform.up * (m_height / 2)) + (A.transform.right * (m_width / 2)), 
			B.transform.position - (A.transform.up * (m_height / 2)) + (A.transform.right * (m_width / 2)), 
			A.transform.position + (A.transform.up * (m_height / 2)) + (A.transform.right * (m_width / 2)), 
			B.transform.position + (A.transform.up * (m_height / 2)) + (A.transform.right * (m_width / 2)), 
			A.transform.position - (A.transform.up * (m_height / 2)) - (A.transform.right * (m_width / 2)), 
			B.transform.position - (A.transform.up * (m_height / 2)) - (A.transform.right * (m_width / 2)), 
		};
	}

    IEnumerator EndSwingNextFrame() {
        // Wait for the end of the frame
        yield return new WaitForEndOfFrame();
        m_flaggedForEndSwing = true;
    }

    // This will only run when the object is enabled (during a swing)
    void LateUpdate(){
		if (m_useDynamicMeshCollider) {
            if (A != null && B != null) {

                if (m_flaggedForEndSwing)
                {
                    ClearAttackList();

                    if (m_useDynamicMeshCollider)
                    {
                        m_colliderContainer.SetActive(false);
                    }

                    // m_swingColliderObject.SetActive (false); from C_Attack, essentially
                    gameObject.SetActive(false);
                }
                else
                {
                    // Get the current box points from the box parameters and the node positions/orientation of A
                    m_curVerts = GetBoxPoints();

                    // Wait for last points to catch up
                    if (!m_firstTick)
                    {
                        m_firstTick = true;
                        m_lastVerts = m_curVerts;
                    }

                    // Make a vert array
                    Vector3[] allVerts = new Vector3[16];
                    for (int i = 0; i < 8; ++i)
                    {
                        allVerts[i] = m_curVerts[i];
                        allVerts[i + 8] = m_lastVerts[i];
                    }

                    // Update the mesh (heavy)
                    Mesh newMesh = new Mesh();
                    newMesh.vertices = allVerts;
                    newMesh.triangles = new int[] { 0, 1, 8 };

                    m_collider.sharedMesh = newMesh;

                    m_lastVerts = m_curVerts;
                }
			}
		}
	}

	public bool CheckTarget(Collider other){
		// Should I damage this enemy
		bool safe = true;

		//Check the current against the other enemies, for already damaged targets (prevents double damage in one swing)
		for (int i = 0; i < m_enemiesDamaged.Count; ++i) {
			if (other.gameObject == m_enemiesDamaged [i]) {
				safe = false;
			}
		}

		if (safe) {
			// Add enemy to the list of damaged objects
			m_enemiesDamaged.Add (other.gameObject);
		}

		return safe;
	}

	public void InitCollider(int damageToDeal, C_Attack attackScript){
		m_curVerts = new Vector3[8];
		m_lastVerts = new Vector3[8];

		// Create new list for collider
		m_enemiesDamaged = new List<GameObject> ();

		// Bring damage in from the scriptable object
		m_damageTicksToDeal = damageToDeal;

		// Store the attack script reference for interrupts and callbacks
		m_attackScript = attackScript;

		// Set up dynamic mesh collider (heavy)
		if (m_useDynamicMeshCollider) {
			// Disable the basic collider shape (uncessary processing otherwise and can cause weird results)
			GetComponent<Collider> ().enabled = false;

			m_colliderContainer = new GameObject ("Sword_Col_Container", typeof(MeshCollider), typeof(DynamicDamageMesh));
			m_colliderContainer.transform.position = Vector3.zero;
			m_colliderContainer.transform.rotation = Quaternion.identity;

			// Get the container's collider and set it to convex
			m_collider = m_colliderContainer.GetComponent<MeshCollider> ();
			// Set the collision layer
			m_colliderContainer.layer = LayerMask.NameToLayer ("SwordCollider");
			// Set to convex hull
			m_collider.convex = true;
			// Set to act as trigger
			m_collider.isTrigger = true;

			// Disable by default
			m_colliderContainer.SetActive (false);
		}
	}

	// End/Interrupt
	void ClearAttackList(){
		m_enemiesDamaged.Clear ();
		m_firstTick = false;
        m_flaggedForEndSwing = false;
    }

	// Cleans up the container when unequipping
	public void UnEquip(){
		if (m_colliderContainer != null) {
			Destroy (m_colliderContainer);
		}
	}

	// Begin a swing state
	public void BeginSwing(){
        if (m_useDynamicMeshCollider) {
			m_colliderContainer.SetActive (true);
			m_colliderContainer.GetComponent<DynamicDamageMesh> ().InitDynaMesh (m_attackScript, this, m_damageTicksToDeal);
		}
	}

	// End a swing state (includes interrupts), FLAG FOR END SWING
	public void EndSwing(){
        // Wait for the end of this frame, then flag for end swing
        StartCoroutine(EndSwingNextFrame());
	}

	public bool PlayerOwnership{
		get{
			return m_isPlayerOwned;
		}
		set{
			m_isPlayerOwned = value;
		}
	}
}
