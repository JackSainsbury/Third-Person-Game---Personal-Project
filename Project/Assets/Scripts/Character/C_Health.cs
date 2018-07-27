using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart {
	public int m_curFill = 4;
	public GameObject m_heartObject;

	public Heart(GameObject m_newUIOBJ){
		m_heartObject = m_newUIOBJ;
	}

	public Heart(){
	}
}

/// <summary>
/// C health.
/// Base script to manage the health of characters
/// P_Health will add an interface system to draw UI to the screen
/// Enemies may use another script to create WS health bars
/// </summary>

public class C_Health : MonoBehaviour {
	// Hearts to start on
	public int m_maxHearts = 5;


	// Healing has started and not finished animating yet, do colour and fx
	public bool m_isHealing = false;
	// Player is poisoned and losing health, do colour and fx
	public bool m_isPoisoned = false;


	// Holds the active hearts
	protected Heart[] m_healthBar;
	protected int m_currentHeartPointer;


	// Current alive state of the player;
	protected bool m_isAlive = true;



	// Use this for initialization
	protected virtual void Start () {
		// Player is alive
		m_isAlive = true;

		// Initialize containers
		m_healthBar = new Heart[m_maxHearts];
		m_currentHeartPointer = m_healthBar.Length - 1;

		// Spawn the hearts
		for (int i = 0; i < m_maxHearts; ++i) {
			// Link to the health object
			m_healthBar [i] = new Heart ();
		}
	}


	// Get the current tick health of the player as an integer value
	public int GetCurHealth(){
		return m_currentHeartPointer * 4 + m_healthBar [m_currentHeartPointer].m_curFill;
	}
	// Get the max tick health of the player as an integer value
	public int GetMaxHealth(){
		// 4 ticks per container
		return m_healthBar.Length * 4;
	}

	// Add or subtract health from the player
	public virtual void ModHealth(int value){
		// Has the health been changed
		bool modDone = false;

		// Loop
		while (modDone == false) {
			int preModPointer = m_currentHeartPointer;

			// Get the filled heart's current health
			int currentHeartFill = m_healthBar [preModPointer].m_curFill;

			// The attempted mod value, apply all damage to the current heart
			int newFill = currentHeartFill + value;

			// Check for overflow
			if (newFill >= 4) {
				newFill = 4;

				// >4 so healing is being done, subtract the amount healed from the healing power left to apply
				value -= 4 - currentHeartFill;

				// Next will be looking at heart up as this one is full
				m_currentHeartPointer++;
			} else if (newFill <= 0) {
				newFill = 0;

				// <0 so damage is being done, add the already damaged health to the negative damaging power left to apply
				value += currentHeartFill;

				// Next will be looking at heart down as this one is empty
				m_currentHeartPointer--;
			} else {
				// either healing or damaging is being done, but we have ended up not overflowing, all damage has been applied and so no more left to apply
				value = 0;
			}
			
			// Mod the health of the current heart
			m_healthBar [preModPointer].m_curFill = newFill;


			// Candidate if we have finished damagine, breaking from the while loop
			if (Mathf.Abs (value) <= 0) { 	// No mod left to apply
				modDone = true;
			}

			if (m_currentHeartPointer < 0) {		// No more health
				m_currentHeartPointer = 0;

				m_isAlive = false;
				modDone = true;
			}

			if (m_currentHeartPointer > (m_maxHearts - 1)) { 		// Max health reached
				m_currentHeartPointer = m_maxHearts - 1;
				modDone = true;
			}
		}
	}

	// ------------------------------------ Properties Set & Get ------------------------------------
	// Read only external alive state
	public bool isAlive{
		get{
			return m_isAlive;
		}
	}
}
