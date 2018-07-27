using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Base : MonoBehaviour
{
    // Length of time the total spell cast should take
    public float m_spellLengthSeconds = 3;

    // Overall timer for the casting of a spell, determines the point in the cast
    protected float m_spellTimer = 0;
    // Overall state machine for the stage we're at in spellcasting
    protected int m_playState = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void CastSpell_Ext()
    {

    }
}
