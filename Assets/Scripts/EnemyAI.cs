using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Idle, Patrol, Chase, Attack }

    public Transform target;
    public Transform patrolPoint;
    public NavMeshAgent ai;

    public EnemyState enemyState;
    private Animator anim;
    private float distanceToTarget;
    private Coroutine idleToPatrol;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ai = GetComponent<NavMeshAgent>();
        enemyState = EnemyState.Idle;
        anim = GetComponent<Animator>();
        distanceToTarget = Mathf.Abs(Vector3.Distance(target.position, transform.position));
    }

    private IEnumerator SwitchToPatrol()
    {
        yield return new WaitForSeconds(5);
        enemyState = EnemyState.Patrol;
        idleToPatrol = null;
    }

    private void SwitchState(int newState)
    {
        if (anim != null && anim.GetInteger("State") != newState)
        {
            anim.SetInteger("State", newState);
        }
    }

    // Update is called once per frame
    void Update()
    {
        distanceToTarget = Mathf.Abs(Vector3.Distance(target.position, transform.position));

        switch (enemyState)
        {
            case EnemyState.Idle:
                SwitchState(0);
                if (ai != null)
                    ai.SetDestination(transform.position);

                if (idleToPatrol == null)
                    idleToPatrol = StartCoroutine(SwitchToPatrol());
                break;

            case EnemyState.Patrol:
                float distanceToPatrolPoint = Mathf.Abs(Vector3.Distance(patrolPoint.position, transform.position));
                if (distanceToPatrolPoint > 2f)
                {
                    SwitchState(1);
                    if (ai != null)
                        ai.SetDestination(patrolPoint.position);
                }
                else
                {
                    SwitchState(0);
                }

                if (distanceToTarget <= 15f)
                    enemyState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                SwitchState(2);

                if (distanceToTarget <= 5f)
                {
                    enemyState = EnemyState.Attack;
                }
                else if (distanceToTarget > 15f)
                {
                    enemyState = EnemyState.Idle;
                }

                if (ai != null)
                    ai.SetDestination(target.position);
                break;

            case EnemyState.Attack:
                SwitchState(3);

                if (distanceToTarget > 5f && distanceToTarget <= 15f)
                {
                    enemyState = EnemyState.Chase;
                }
                else if (distanceToTarget > 15f)
                {
                    enemyState = EnemyState.Idle;
                }
                break;

            default:
                SwitchState(0);
                break;
        }
    }
}
