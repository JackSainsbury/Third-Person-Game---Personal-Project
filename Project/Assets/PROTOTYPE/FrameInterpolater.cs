using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameInterpolater : MonoBehaviour {

    public AnimationClip m_stateRef;

	// Use this for initialization
	void Start () {
       Debug.Log(m_stateRef.name);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
