using UnityEngine;

public class SimpleEnemyAI : MonoBehaviour
{
    public enum AIState { Patrol, Chase, Attack }

    [Header("AI Configuration")]
    [SerializeField] private AIState currentState = AIState.Patrol;
    [SerializeField] private Transform targetPlayer;
    [SerializeField] private float moveSpeed = 3.0f;

    [Header("Detection Thresholds")]
    [SerializeField] private float detectionRadius = 8.0f;
    [SerializeField] private float attackRadius = 1.5f;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolWaypoints;
    private int currentWaypointIndex = 0;

    void Update()
    {
        // Fallback check: try to find player by tag if not explicitly assigned
        if (targetPlayer == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) targetPlayer = playerObj.transform;
        }

        EvaluateState();
        PerformStateAction();
    }

    private void EvaluateState()
    {
        if (targetPlayer == null)
        {
            currentState = AIState.Patrol;
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.position);

        if (distanceToPlayer <= attackRadius)
        {
            currentState = AIState.Attack;
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            currentState = AIState.Chase;
        }
        else
        {
            currentState = AIState.Patrol;
        }
    }

    private void PerformStateAction()
    {
        switch (currentState)
        {
            case AIState.Patrol:
                PatrolBehavior();
                break;
            case AIState.Chase:
                ChaseBehavior();
                break;
            case AIState.Attack:
                AttackBehavior();
                break;
        }
    }

    private void PatrolBehavior()
    {
        if (patrolWaypoints.Length == 0) return;

        Transform targetWaypoint = patrolWaypoints[currentWaypointIndex];
        MoveTowards(targetWaypoint.position);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Length;
        }
    }

    private void ChaseBehavior()
    {
        if (targetPlayer != null)
        {
            MoveTowards(targetPlayer.position);
        }
    }

    private void AttackBehavior()
    {
        // Stop moving and look at the player to execute attack logic
        if (targetPlayer != null)
        {
            Vector3 lookPos = new Vector3(targetPlayer.position.x, transform.position.y, targetPlayer.position.z);
            transform.LookAt(lookPos);
            
            // Trigger your specific combat damage/animation handlers here
        }
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        Vector3 targetDirection = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        transform.position = Vector3.MoveTowards(transform.position, targetDirection, moveSpeed * Time.deltaTime);
        
        Vector3 directionVector = targetDirection - transform.position;
        if (directionVector.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(directionVector);
        }
    }

    // Visualize detection ranges directly in the Unity Scene View editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}