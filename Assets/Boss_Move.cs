using UnityEngine;
using System.Collections;


public class Boss_Move : StateMachineBehaviour
{
    Transform player;
    Rigidbody2D rb;

    BossBrain bossBrain;

    public float speed = 6f;
    public float meleeRange = 3f;



    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();

        // Get the BossBrain component
        bossBrain = animator.GetComponent<BossBrain>();

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossBrain.LookAtPlayer();
        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 new_position = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(new_position);

        if (Vector2.Distance(player.position, rb.position) <= meleeRange)
        {
            animator.SetTrigger("MeleeAttack");
        }
        

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("MeleeAttack");
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
