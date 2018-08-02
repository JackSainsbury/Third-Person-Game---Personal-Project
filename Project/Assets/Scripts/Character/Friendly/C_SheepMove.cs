using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_SheepMove : MonoBehaviour {
    // Graphics animator
    public Animator m_animator;

    // State of the sheep's AI
    private int m_AIState = 0;

    // Hash id for the state which is stored on the animator
    private int m_sheepAnimatorStateID;

    // Speed idle / attacked
    private float m_runRoamSpeed = 6;
    private float m_slowRoamSpeed = 1;

    // Direction change delays idle / attacked
    private float m_runRoamDirDelay = 3;
    private float m_slowRoamDirDelay = 1;

    private float m_directionChangeDelay;

    private IEnumerator m_roamRoutine;

    // Use this for initialization
    void Start() {
        m_sheepAnimatorStateID = Animator.StringToHash("State");

        m_roamRoutine = Roam();
        StartCoroutine(m_roamRoutine);

        m_directionChangeDelay = m_slowRoamDirDelay;
    }


    // Update is called once per frame
    void Update() {
        switch (m_AIState) {
            case 0:
                m_animator.SetInteger(m_sheepAnimatorStateID, 0);
                break;
            case 1:
                m_animator.SetInteger(m_sheepAnimatorStateID, 1);

                transform.Translate(Vector3.forward * m_slowRoamSpeed * Time.deltaTime);

                m_directionChangeDelay = m_slowRoamDirDelay;

                break;
            case 2:
                m_animator.SetInteger(m_sheepAnimatorStateID, 2);

                transform.Translate(Vector3.forward * m_runRoamSpeed * Time.deltaTime);

                m_directionChangeDelay = m_runRoamDirDelay;

                break;
            case 3:
                StopCoroutine(m_roamRoutine);
                m_animator.SetInteger(m_sheepAnimatorStateID, 3);
                break;
        }
	}


    IEnumerator Roam()
    {
        yield return new WaitForSeconds(Random.Range(0.75f, m_directionChangeDelay));

        m_AIState = Random.Range(0, 2);

        transform.rotation = Quaternion.Euler(transform.rotation.x, Random.Range(0, 360), transform.rotation.z);

        m_roamRoutine = Roam();
        StartCoroutine(m_roamRoutine);
    }

    public int State
    {
        get
        {
            return m_AIState;
        }
        set
        {
            m_AIState = value;
        }
    }
}
