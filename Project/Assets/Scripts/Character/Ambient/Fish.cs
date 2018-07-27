using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour {

    public float m_fwdSpeed = 1;
    public float m_turnSpeedGlide = 5;
    public float m_hitTurnSpeed = 25;

    public Color m_col;
    public GameObject m_visuals;

    private float m_turnSpeed = 5;
    private int m_moveDir = 0;
    private bool m_overrideMovement = false;

    private MaterialPropertyBlock m_probBlock;
    private int m__ColorID;

	// Use this for initialization
	void Start () {
        StartCoroutine(randTurn());
        m__ColorID = Shader.PropertyToID("_Color");
        m_probBlock = new MaterialPropertyBlock();
        m_probBlock.SetColor(m__ColorID, m_col);

        m_visuals.GetComponent<Renderer>().SetPropertyBlock(m_probBlock);
    }

    IEnumerator randTurn()
    {
        yield return new WaitForSeconds(Random.Range(1, 3));

        if (!m_overrideMovement)
        {
            m_moveDir = Random.Range(-1, 2);
            m_turnSpeed = m_turnSpeedGlide;
        }

        yield return StartCoroutine(randTurn());
    }

    // Update is called once per frame
    void Update() {

        switch (m_moveDir) {
            case -1:
                transform.Rotate(0, -m_turnSpeed * Time.deltaTime, 0);
                break;
            case 0:
                break;
            case 1:
                transform.Rotate(0, m_turnSpeed * Time.deltaTime, 0);
                break;
        }

        transform.Translate(0, 0, -m_fwdSpeed * Time.deltaTime);
    }
}
