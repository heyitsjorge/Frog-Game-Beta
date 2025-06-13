using UnityEngine;
using System.Collections;


public class Boss_Move : StateMachineBehaviour
{
    Transform player;
    Rigidbody2D rb;

    BossBrain bossBrain;

    BossWeapon bossWeapon;

    public float speed = 6f;
    public float meleeRange = 3f;



    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();


        // Get the BossBrain component
        bossBrain = animator.GetComponent<BossBrain>();
        bossWeapon = animator.GetComponent<BossWeapon>();
        bossWeapon.playerTransform = player;

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        bossWeapon.TickDashCooldown();
        Debug.Log($"[MoveState] rb.velocity = {rb.velocity}");


        bossBrain.LookAtPlayer();
        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 new_position = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        float dist = Vector2.Distance(player.position, rb.position);

        if (!bossWeapon.isDashing && !bossWeapon.isDodging)
        {
            rb.MovePosition(new_position);
        }

        if (dist <= meleeRange)
        {
            animator.SetTrigger("MeleeAttack");
                animator.ResetTrigger("Dash");
            }
        if (dist > meleeRange && dist <= bossWeapon.dashAttackRange && bossWeapon.dashCooldownTimer <= 0 && !bossWeapon.isDodging)
        {
            animator.SetTrigger("Dash");
            animator.ResetTrigger("MeleeAttack");
            bossWeapon.dashMovement();
        }


    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("MeleeAttack");
        animator.ResetTrigger("Dash");
        bossWeapon.isDashing = false;
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
