using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class P_Health : C_Health {
	// UI Pixel buffer
	public float m_HBBuffer = 15;
	// Speed to flash health bar when poisoned
	public float m_posionFlashSpeed = 3f;
	// Number of hearts to draw before looping down
	public int m_loopLineAt = 8;

	// Sprite set
	public Sprite m_fullHeart_Sprite;
	public Sprite m_threeQuartHeart_Sprite;
	public Sprite m_halfHeart_Sprite;
	public Sprite m_quartHeart_Sprite;
	public Sprite m_emptyHeart_Sprite;

	// Prefab to display heart sprites
	public GameObject m_heartPrefab;
	public GameObject m_uiCanvas;


	private float m_effectTimer = 0;

	private Vector2 m_heartDimensions;

	// Use this for initialization
	protected override void Start () {
		base.Start ();

		// Pull the rect image rect
		Rect heartRect = m_heartPrefab.GetComponent<RectTransform> ().rect;
		// Set the square dimensions
		m_heartDimensions = new Vector2(heartRect.width, heartRect.height);

		// Spawn the heart VISUALS
		for (int i = 0; i < m_maxHearts; ++i) {
			// Create new UI element
			GameObject newHeart = Instantiate (m_heartPrefab);
			newHeart.transform.SetParent (m_uiCanvas.transform);

			float yPos = (m_heartDimensions.y / 2 + m_HBBuffer);
			float xCount = i;

			if (i >= m_loopLineAt) {
				yPos *= 2;
				xCount -= (m_loopLineAt);
			}

			// Position correctly
			newHeart.transform.position = new Vector3(((m_heartDimensions.x + m_HBBuffer) * xCount) + (m_heartDimensions.x / 2 + m_HBBuffer), Screen.height - yPos, 0);
			newHeart.GetComponent<Image> ().sprite = m_fullHeart_Sprite;

			// Connect the hearts visuals to the hearts
			m_healthBar [i].m_heartObject = newHeart;
		}
	}


	// ------------------------------------

	public IEnumerator SetColours(){
		while (m_effectTimer < 6.3f) {
			m_effectTimer -= Time.deltaTime * m_posionFlashSpeed;

			float colval = Mathf.Abs (Mathf.Cos (m_effectTimer));

			if (m_isPoisoned) {
				foreach (Heart h in m_healthBar) {
					h.m_heartObject.GetComponent<Image> ().color = new Color (
						colval, 
						1, 
						colval
					);
				}
			}

			yield return false;
		}

		yield return true;
	}

	// ------------------------------------

	public override void ModHealth (int value)
	{
		base.ModHealth (value);

		// Quickly run through the health bar, post health done and update the tick images
		for (int i = 0; i < m_healthBar.Length; ++i) {
			// display the correct heart information on our active pointer, either pre or active as it has not been modified
			switch (m_healthBar [i].m_curFill) {
			case 0:
				m_healthBar [i].m_heartObject.GetComponent<Image> ().sprite = m_emptyHeart_Sprite;
				break;
			case 1:
				m_healthBar [i].m_heartObject.GetComponent<Image> ().sprite = m_quartHeart_Sprite;
				break;
			case 2:
				m_healthBar [i].m_heartObject.GetComponent<Image> ().sprite = m_halfHeart_Sprite;
				break;
			case 3:
				m_healthBar [i].m_heartObject.GetComponent<Image> ().sprite = m_threeQuartHeart_Sprite;
				break;
			case 4:
				m_healthBar [i].m_heartObject.GetComponent<Image> ().sprite = m_fullHeart_Sprite;
				break;
			}
		}

		if (!isAlive) {
			StartCoroutine(KillPlayer ());
		}
	}

	// Character has died, perform general character death logic
	IEnumerator KillPlayer(){
		GetComponent<P_Move> ().enabled = false;
		yield return new WaitForSeconds (1);

		SceneManager.LoadScene (SceneManager.GetActiveScene().name);
		yield return null;
	}
}
