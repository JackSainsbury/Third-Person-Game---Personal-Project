using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// 
/// C animation speed.
/// 	Script will look for an attack component on the root (character), get the current animation duration and
/// 	set the correct speed for all clips playing (on all layers), in order to perform the attack in the right 
/// 	amount of time.
/// 
/// </summary>

public class C_AnimSpeed : StateMachineBehaviour {
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		// Set speed to 1 initially
		animator.speed = 1;
		// Get the target time from the root
		float targetTime = animator.transform.root.GetComponent<C_Attack> ().CurAnimDur;

		// Get all layers
		int layerCount = animator.layerCount;

		// For each layer
		for (int i = 0; i < layerCount; ++i) {
			// If I have a clip playing on this layer
			if (animator.GetCurrentAnimatorClipInfoCount (i) > 0) {
				// Get the current clip length (actual time)
				float clipLength = animator.GetCurrentAnimatorClipInfo (i) [0].clip.length;

				// If the target time is not 0 (avoids divide by zero), set the speed to play over seconds
				if (targetTime > 0) {
					animator.speed = clipLength / targetTime;
				}
			}
		}
	}

}
