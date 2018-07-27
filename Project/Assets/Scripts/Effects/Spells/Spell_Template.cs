using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Template : Spell_Base
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 
        if (Input.GetKeyDown(KeyCode.Space))
        {
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

    IEnumerator CastSpell()
    {
        // Main loop for the spell routine
        while (m_spellTimer < m_spellLengthSeconds)
        {
            switch (m_playState)
            {
                case 0:
                    break;
            }

            // Increase timer
            m_spellTimer += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }
}
