using Passer;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    [SerializeField] float aggroRange = 20f;
    [SerializeField] float attackRange = 3f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float accelaration = 10f;



    Transform target;
    Rigidbody rb;
    Animator animator;
    Survivability survivability;
    RagdollController rd;
    Combat combat;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = gameObject.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        animator = gameObject.GetComponent<Animator>();
        animator.applyRootMotion = false;
        survivability = GetComponent<Survivability>();
        rd = GetComponent<RagdollController>();
        combat = GetComponent<Combat>();
        player = GameObject.FindGameObjectWithTag("Player");

    }

 

    // Update is called once per frame
    void Update()
    {
 

        if (survivability.IsStunned()|| rd.IsRagdoll())
        {
            return;
        }

        if (target == null) {
            animator.SetBool("isRunning", false);
            return;
        }
        if (player.GetComponent<RagdollController>().IsRagdoll())
        {
            if (target == player.transform)
            {
                
                target = target.GetComponent<RagdollController>().GetRagdollMainBody().transform;
                
            }
            
        }
        else
        {
            target = player.transform;
        }
        if (Vector3.Distance(transform.position, target.transform.position) <= attackRange && combat.CanHit())
        {
            AttackPlayer();
            return;
        }

        if (Vector3.Distance(transform.position , target.transform.position) <= aggroRange)
        {
            ChasePlayer();
            return;
        }
        animator.SetBool("isRunning", false);

    }
    public void AttackPlayer()
    {
 
        combat.Attack(target.gameObject);
    }
    private void ChasePlayer()
    {  
        MoveTowardsPlayer();
    }


    private void MoveTowardsPlayer()
    {
        Vector3 moveDirection = (target.position - transform.position).normalized;
        moveDirection.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(moveDirection);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, lookRotation, 360 * Time.deltaTime));

        Vector3 targetVel = moveDirection * moveSpeed;
        Vector3 currentVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        Vector3 finalVel = Vector3.MoveTowards(currentVel, targetVel, accelaration * Time.deltaTime);

        rb.velocity = new Vector3(finalVel.x, rb.velocity.y, finalVel.z);

        animator.SetBool("isRunning", true);
    }



}
