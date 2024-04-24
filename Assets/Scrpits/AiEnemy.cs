using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    public NavMeshAgent agent;

    Animator anim;

    public int patroller = 2;

    public Transform player;

    public Transform AttackPoint;

    public LayerMask whatIsGround, whatIsPlayer;

    public GameObject projectile;

    [Header("Patrolling")]
    public Vector3 WalkP;
    bool WPSet;
    public float WPRange;

    [Header("Attacking")]
    public float TimeBtwAttack;
    public bool attacked;
    public float howHigh;
    public float howLong;

    [Header("States")]
    public float attackRange;
    public float sightRange;
    public bool PlayerOnSight;
    public bool PlayerOnAttack;

    bool canAttack = true;

    [Header("TakingDamage")]
    public int health;
    public int damage;
    public bool isDead;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        health = 100;
        isDead = false;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerOnSight = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        PlayerOnAttack = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        //OnDrawGizmosSelected();
        if (!PlayerOnSight && !PlayerOnAttack)
        {
            anim.SetBool("Attack", false);
            Patroll();
        }
        if (PlayerOnSight && PlayerOnAttack)
        {
            anim.SetBool("Patrolling", true);
            Attack();
        }
        if (PlayerOnSight && !PlayerOnAttack) {
            anim.SetBool("Attack", false);
            Chase(); 
        }
        
    }

    private void Patroll()
    {
        if (patroller == 2)
        {
            anim.SetBool("Patrolling", true);
            if (!WPSet) WPSearch();
            if (WPSet)
                agent.SetDestination(WalkP);

            Vector3 distanceToWP = transform.position - WalkP;

            if (distanceToWP.magnitude < 1f)
                WPSet = false;
            
        }
        else
        {
            anim.SetBool("Patrolling", true);
            if (patroller == 3)
            {
                if (!WPSet)
                {
                    WalkP = new Vector3(4f, transform.position.y, 84f);
                    patroller = 4;
                } else if(WPSet) agent.SetDestination(WalkP);
            } 
            else if (patroller == 4)
            {
                if (!WPSet)
                {
                    WalkP = new Vector3(4f, transform.position.y, 103f);
                    patroller = 3;
                } else if(WPSet) agent.SetDestination(WalkP);
            }

            if (Physics.Raycast(WalkP, -transform.up, 2f, whatIsGround))
                WPSet = true;
            Vector3 distanceToWP = transform.position - WalkP;
            if (distanceToWP.magnitude < 1f)
                    WPSet = false;
           

        }
    }

    private void WPSearch()
    {
        float randomZ = Random.Range(-WPRange, WPRange);
        float randomX = Random.Range(-WPRange, WPRange);

        WalkP = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(WalkP, -transform.up, 2f, whatIsGround)) 
            WPSet = true;
    }
    private void Chase()
    {
        agent.SetDestination(player.position);
        anim.SetBool("Patrolling", true);
    }
    private void Attack()
    {
        if (canAttack)
        {
            agent.SetDestination(transform.position);

            transform.LookAt(player);

            if (!attacked)
            {
                anim.SetBool("Attack", true);
                //Debug.Log("Hit");
                //player.GetComponent<MovementWithCam>().TakeDamage(10);
                Rigidbody rb = Instantiate(projectile, AttackPoint.transform.position, Quaternion.identity).GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * howLong, ForceMode.Impulse);
                rb.AddForce(transform.up * howHigh, ForceMode.Impulse);

                attacked = true;
                Invoke(nameof(ResetAttack), TimeBtwAttack);
                
            }
        }
    }

    private void ResetAttack()
    {
        attacked = false;
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            agent.Stop();
            canAttack = false;
            anim.SetBool("isDead", true);
            isDead = true;
        }
        if(isDead)
        {
            Invoke("Destroyer", 1.5f);           
        }
    }
    private void Destroyer()
    {
        Destroy(gameObject);
    }
}
