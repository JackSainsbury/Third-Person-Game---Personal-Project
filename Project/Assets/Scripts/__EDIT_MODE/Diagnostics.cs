using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Diagnostics : MonoBehaviour {

	static StackTrace stackTrace;

	public static void StackTraceFunction(){
		stackTrace = new StackTrace ();
		print ("stackTrace !! " + stackTrace.GetFrame (1).GetMethod ().Name);
	}
}
