using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_Gems : MonoBehaviour {
	// Text to display the current gem count
	public Text m_gemCounter;

	// The player's gem count
	private int m_gemCount = 0;

	// Use this for initialization
	void Start () {
		m_gemCounter.text = "x" + m_gemCount.ToString ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ModGems(int value){

        // Candidate value
        int newCount = m_gemCount + value;

        // Perform the mod, purchase or add

        if (newCount >= 0) {
            // Update gem count
            m_gemCount = newCount;

            //Gem colour & anim
            m_gemCounter.text = "x" + m_gemCount.ToString ();

		}// else {
			// Couldn't afford purchase

		//}
	}
}
