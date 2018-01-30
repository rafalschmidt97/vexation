using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollBehavior : StateMachineBehaviour {
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.GetComponent<Hero>().Roll = true;
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.GetComponent<Hero>().Roll = false;
	}
}
