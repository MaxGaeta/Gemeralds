using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateLose : StateMachineBehaviour {

	public GameLogic logic;
	
	//  OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			logic = animator.GetComponent<GameLogic>();
			if (animator.GetBool("BlowUpGem")) 
			{
				logic.loseByBlowUpGem();
			}
			else if (animator.GetBool("BlowUpDeathRock"))
			{
				logic.loseByBlowUpDeathRock();
			}
	}	

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		logic.checkReset();
		logic.checkNextLevel();
		animator.SetBool("BlowUpGem",false);
		animator.SetBool("BlowUpDeathRock",false);
		if (logic.getDefaultState())
		{
				logic.resetDefaultState();
				animator.Rebind();
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
	}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
	}
}
