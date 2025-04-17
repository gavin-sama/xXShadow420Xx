using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    // Reference to the NavMeshAgent component that controls movement
    public NavMeshAgent navMeshAgent;

    // Time variables for different AI actions
    public float startWaitTime = 4;        // Time to wait at a waypoint
    public float timeToRotate = 2;         // Time for the AI to rotate before taking action
    public float speedWalk = 6;            // Walking speed of the AI
    public float speedrun = 9;             // Running speed of the AI

    // Variables to set up the AI's vision
    public float viewRadius = 15;          // Radius at which the AI can detect the player
    public float viewAngle = 90;           // Angle of the AI's view cone
    public LayerMask playerMask;           // What the AI will consider as "player"
    public LayerMask obstacleMask;         // What the AI considers as obstacles
    public float meshResolution = 1f;      // The resolution for the navmesh (pathfinding)
    public int edgeIterations = 4;         // Edge smoothing in navmesh
    public float edgeDistance = 0.5f;     // How far edges are considered in pathfinding

    // Waypoints for patrolling behavior
    public Transform[] waypoints;
    int m_CurrentWaypointIndex;

    // Variables to track player's position
    Vector3 playerLastPosition = Vector3.zero;
    Vector3 m_PlayerPosition;

    // Variables to control the AI's behavior states
    float m_WaitTime;
    float m_TimeToRotate;
    bool m_PlayerInRange;
    bool m_PlayerNear;
    bool m_IsPatrol;
    bool m_CaughtPlayer;

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerPosition = Vector3.zero;
        m_IsPatrol = true;   // AI starts patrolling
        m_CaughtPlayer = false;
        m_PlayerInRange = false;
        m_WaitTime = startWaitTime;
        m_TimeToRotate = timeToRotate;

        m_CurrentWaypointIndex = 0;
        navMeshAgent = GetComponent<NavMeshAgent>();  // Get the NavMeshAgent component attached to the AI

        navMeshAgent.isStopped = false;    // Start the agent's movement
        navMeshAgent.speed = speedWalk;    // Set the walking speed
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position); // Set initial waypoint
    }

    // Update is called once per frame
    void Update()
    {
        EnvironmentView();  // Check the environment to detect the player

        if (!m_IsPatrol)   // If AI is not patrolling, it's chasing
        {
            Chasing();
        }
        else  // Otherwise, the AI is patrolling
        {
            Patroling();
        }
    }

    // Method to move the AI with a given speed
    void Move(float speed)
    {
        navMeshAgent.isStopped = false;  // Allow movement
        navMeshAgent.speed = speed;      // Set the movement speed
    }

    // Method to stop the AI's movement
    void Stop()
    {
        navMeshAgent.isStopped = true;   // Stop the movement
        navMeshAgent.speed = 0;          // Set speed to 0
    }

    // Method called when the AI catches the player
    void CaughtPlayer()
    {
        m_CaughtPlayer = true;  // Set the caught player flag
    }

    // Method to make the AI look at the player
    void LookingPlayer(Vector3 player)
    {
        navMeshAgent.SetDestination(player);   // Set the destination to the player's position
        if (Vector3.Distance(transform.position, player) <= 0.3)  // If very close to the player
        {
            if (m_WaitTime <= 0)  // If the wait time is over, go back to patrol
            {
                m_PlayerNear = false;
                Move(speedWalk);   // Walk speed
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position); // Go back to waypoint
                m_WaitTime = startWaitTime;  // Reset wait time
                m_TimeToRotate = timeToRotate;  // Reset rotation time
            }
            else
            {
                Stop();  // Stop if waiting
                m_WaitTime -= Time.deltaTime;  // Reduce wait time
            }
        }
    }

    // Chasing method to handle the AI when chasing the player
    private void Chasing()
    {
        m_PlayerNear = false;     // Reset player near flag
        playerLastPosition = Vector3.zero;

        if (!m_CaughtPlayer)      // If the player isn't caught yet
        {
            Move(speedrun);       // Run at the running speed
            navMeshAgent.SetDestination(m_PlayerPosition);   // Set destination to the player's position
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");   // Find the player object
        if (playerObj != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerObj.transform.position);   // Measure distance to player

            // Check if AI reached the player and is far enough to go back to patrol
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (m_WaitTime <= 0 && !m_CaughtPlayer && distanceToPlayer >= 6f)
                {
                    m_IsPatrol = true;   // Switch to patrolling
                    m_PlayerNear = false;
                    Move(speedWalk);  // Walk at patrol speed
                    m_TimeToRotate = timeToRotate;
                    m_WaitTime = startWaitTime;
                    navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);  // Go back to waypoint
                }
                else
                {
                    if (distanceToPlayer >= 2.5f)  // Stop if too close
                    {
                        Stop();
                        m_WaitTime -= Time.deltaTime;  // Decrease wait time
                    }
                }
            }
        }
    }

    // Patroling method to handle the AI while patrolling
    void Patroling()
    {
        if (m_PlayerNear)  // If the player is nearby
        {
            if (m_TimeToRotate <= 0)   // If rotation time is over
            {
                Move(speedWalk);   // Start moving at patrol speed
                LookingPlayer(playerLastPosition);   // Look at the player's last position
            }
            else
            {
                Stop();  // Stop if rotating
                m_TimeToRotate -= Time.deltaTime;  // Decrease rotation time
            }
        }
        else  // If the player is not near
        {
            m_PlayerNear = false;
            playerLastPosition = Vector3.zero;  // Reset player position
            navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);  // Continue patrolling to the next waypoint
            if (m_WaitTime <= 0)   // If the AI has reached a waypoint
            {
                NextPoint();   // Go to the next waypoint
                Move(speedWalk);  // Walk speed
                m_WaitTime = startWaitTime;  // Reset wait time
            }
            else
            {
                Stop();   // Stop if waiting
                m_TimeToRotate -= Time.deltaTime;  // Decrease rotation time
            }
        }
    }

    // Method to go to the next waypoint in the patrol route
    void NextPoint()
    {
        m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;  // Loop back to the first waypoint
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);  // Set the next waypoint as destination
    }

    // Method to detect the player in the environment
    void EnvironmentView()
    {
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);  // Find all players in range

        for (int i = 0; i < playerInRange.Length; i++)
        {
            Transform player = playerInRange[i].transform;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;  // Get direction to player
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)  // If the player is within the field of view
            {
                float dstToPlayer = Vector3.Distance(transform.position, player.position);  // Distance to player
                if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obstacleMask))  // If no obstacle in the way
                {
                    m_PlayerInRange = true;  // Set player in range
                    m_IsPatrol = false;      // Stop patrolling, start chasing
                }
                else
                {
                    m_PlayerInRange = false;  // If there's an obstacle, stop chasing
                }
            }

            // If player is out of range, stop chasing
            if (Vector3.Distance(transform.position, player.position) > viewRadius)
            {
                m_PlayerInRange = false;
            }

            if (m_PlayerInRange)
            {
                m_PlayerPosition = player.transform.position;  // Set the player's position as target
            }
        }
    }
}
