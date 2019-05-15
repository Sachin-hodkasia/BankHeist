using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievmentPanelEntry : StateMachineBehaviour
{
    string binary = "00100";
    bool calledOnce = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        calledOnce = false;
        binary = UImanager.Instance.BinaryAchievment;
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        if (calledOnce == false && stateInfo.normalizedTime > 0.5f)
        {
            //Debug.Break();
            for (int i = 0; i < 5; i++)
            {
                animator.gameObject.transform.GetChild(0).GetChild(i).GetComponent<Animator>().SetBool("New", false);
                bool NEW = false;
                if (binary[i] == '1')
                {
                    NEW = true;
                }
                else
                {
                    NEW = false;
                }
                animator.gameObject.transform.GetChild(0).GetChild(i).GetComponent<Animator>().SetBool("New", NEW);
                animator.gameObject.transform.GetChild(0).GetChild(i).GetComponent<Animator>().SetTrigger("StartAnimating");
            }
            calledOnce = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
