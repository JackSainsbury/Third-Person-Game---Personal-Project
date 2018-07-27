using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Arcane : Spell_Base {
	public float m_chargeForSeconds = 1;
	public float m_inwardChargeForSeconds = 3.5f;
	public float m_postChargeForSeconds = 1;

    // Ring orb sprite
    public GameObject m_arcaneSphere;

    /// <summary>
    /// PARTICLES AND EFFECT SECTIONS
    /// </summary>
    public ParticleSystem m_chargeParticles;
	public ParticleSystem m_inwardChargeParticles;

	private particleAttractorLinear m_inwardAttractor;

	// Use this for initialization
	void Start () {
		m_inwardAttractor = m_inwardChargeParticles.GetComponent<particleAttractorLinear> ();
	}
	
	// Update is called once per frame
	void Update () {
		// 
		if (Input.GetKeyDown (KeyCode.Space)) {
            CastSpell_Ext();
        }
	}
    
    public override void CastSpell_Ext()
    {
        // Call the virtual base function
        base.CastSpell_Ext();

        // Start the spell routine
        StartCoroutine(CastSpell());
    }

	IEnumerator CastSpell () {
        bool scaled = false;

        // Main loop for the spell routine
        while (m_spellTimer < m_spellLengthSeconds)
        {
            switch (m_playState)
            {
                case 0:
                    // Play particle charge
                    m_chargeParticles.Play();
                    m_playState = 1;
                    break;
                case 1:
                    if(m_spellTimer >= m_chargeForSeconds)
                    {
                        m_playState = 2;
                        // Play the inner charge fx
                        m_inwardChargeParticles.Play();
                        Spell_Library.P_TrailFadeAfterEXT(m_chargeParticles.trails, m_postChargeForSeconds, this);
                    }
                    break;
                case 2:
                    if(m_spellTimer <= m_inwardChargeForSeconds + m_chargeForSeconds)
                    {
                        float interp_T = (m_spellTimer - m_chargeForSeconds) / m_inwardChargeForSeconds;

                        m_inwardAttractor.speed = interp_T * 7;

                        if (m_spellTimer >= m_inwardChargeForSeconds + (m_chargeForSeconds / 2))
                        {

                            // Transparency mod
                            if (!scaled)
                            {
                                // InnerRing
                                Spell_Library.P_ScaleObjectInOutEXT(m_arcaneSphere, 1.5f, 0.1f, 0.3f, false, this);

                                scaled = true;
                            }
                        }

                    }
                    else
                    {
                        m_playState = 3;
                    }
                    break;
                case 3:
                    break;
            }

            // Increase timer
            m_spellTimer += Time.deltaTime;

            yield return null;
        }

        yield return null;
	}
}
