using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_SheepHealth : C_Health
{
    public C_SheepMove m_sheepMoveScript;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void ModHealth(int value)
    {
        // Can be damaged
        if (m_isAlive)
        {
            // Call the base health mod, basic value mod and health bar things
            base.ModHealth(value);

            if (m_isAlive)
            {
                // Damage animation
                m_sheepMoveScript.State = 2;
            }
            else
            {
                m_sheepMoveScript.State = 3;
            }
        }
    }
}
