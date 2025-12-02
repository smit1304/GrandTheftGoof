using UnityEngine;

// Demonstrating the use of the StateMachine class with added "Patrol" and "Rest" states
public class FSMWithProb : MonoBehaviour
{
    public GameObject player;             // Player GameObject reference
    public float distanceToChase = 10;   // Distance to start chasing player
    public float distanceToAttack = 2;   // Distance to start attacking player
    public float FOV = 60;                // Field of View (degrees)
    public float speed = 1f;              // Movement speed
    public bool StrongerThanPlayer = true;

    private float FOV_in_RAD;
    public float csCosFOV_2;              // Cosine of half FOV angle

    StateMachine stateMachine;

    public float Deg2Rad(float deg)
    {
        return deg / 180f * Mathf.PI;
    }
    public float Rad2Deg(float rad)
    {
        return rad * 180 / Mathf.PI;
    }

    void Start()
    {
        stateMachine = new StateMachine();
        csCosFOV_2 = Mathf.Cos(Deg2Rad(FOV) / 2);

        // Seek_Waypoint state - transitions to Chase, Attack, Evade, or Patrol if no player in sight
        var seekWaypoint = stateMachine.CreateState("Seek_Waypoint");
        seekWaypoint.onEnter = delegate {
            Debug.Log("In Seek_Waypoint.onEnter");
        };
        seekWaypoint.onStay = delegate {
            Vector3 playerHeading = player.transform.position - this.transform.position;
            float distanceToPlayer = playerHeading.magnitude;
            Vector3 directionToPlayer = playerHeading.normalized;

            bool InFront = (Vector3.Dot(this.transform.forward, directionToPlayer) >= csCosFOV_2);

            if (InFront)
            {
                if (distanceToPlayer <= distanceToAttack)
                {
                    stateMachine.TransitionTo("Attack");
                }
                else if (distanceToPlayer <= distanceToChase)
                {
                    if (this.StrongerThanPlayer)
                    {
                        stateMachine.TransitionTo("Chase");
                    }
                    else
                    {
                        stateMachine.TransitionTo("Evade");
                    }
                }
                else
                {
                    // Player is visible but too far -> Patrol
                    stateMachine.TransitionTo("Patrol");
                }
            }
            else
            {
                // Player not in view -> Patrol
                stateMachine.TransitionTo("Patrol");
            }
        };
        seekWaypoint.onExit = delegate {
            Debug.Log("In Seek_Waypoint.onExit");
        };

        // Chase state - Move toward player; switch to Evade if weaker
        var chase = stateMachine.CreateState("Chase");
        chase.onEnter = delegate { Debug.Log("In Chase.onEnter"); };
        chase.onStay = delegate {
            Vector3 E = this.transform.position;
            Vector3 P = player.transform.position;
            Vector3 Heading = P - E;
            Vector3 HeadingDir = Heading.normalized;
            this.transform.position += HeadingDir * speed * Time.deltaTime;

            if (!this.StrongerThanPlayer)
            {
                stateMachine.TransitionTo("Evade");
            }
        };
        chase.onExit = delegate { Debug.Log("In Chase.onExit"); };

        // Attack state - Placeholder for attack logic
        var attack = stateMachine.CreateState("Attack");
        attack.onEnter = delegate { Debug.Log("In Attack.onEnter"); };
        attack.onStay = delegate { };
        attack.onExit = delegate { Debug.Log("In Attack.onExit"); };

        // Evade state - Move away from player; switch to Chase if stronger
        var evade = stateMachine.CreateState("Evade");
        evade.onEnter = delegate { Debug.Log("In Evade.onEnter"); };
        evade.onStay = delegate {
            Vector3 E = this.transform.position;
            Vector3 P = player.transform.position;
            Vector3 Heading = E - P;
            Vector3 HeadingDir = Heading.normalized;
            this.transform.position += HeadingDir * speed * Time.deltaTime;

            if (this.StrongerThanPlayer)
            {
                stateMachine.TransitionTo("Chase");
            }
        };
        evade.onExit = delegate { Debug.Log("In Evade.onExit"); };

        // New Patrol state - soldier moves randomly or along waypoints
        var patrol = stateMachine.CreateState("Patrol");
        patrol.onEnter = delegate { Debug.Log("In Patrol.onEnter"); };
        patrol.onStay = delegate {
            // Example patrol logic: move forward constantly 
            this.transform.position += this.transform.forward * (speed / 2) * Time.deltaTime;

            // If player comes into view within chase distance, go back to Seek_Waypoint to reevaluate
            Vector3 playerHeading = player.transform.position - this.transform.position;
            bool playerInView = (Vector3.Dot(this.transform.forward, playerHeading.normalized) >= csCosFOV_2);
            if (playerHeading.magnitude <= distanceToChase && playerInView)
            {
                stateMachine.TransitionTo("Seek_Waypoint");
            }
        };
        patrol.onExit = delegate { Debug.Log("In Patrol.onExit"); };

        // New Rest state - soldier stands still or performs idle animation
        var rest = stateMachine.CreateState("Rest");
        rest.onEnter = delegate {
            Debug.Log("In Rest.onEnter");
            // Could trigger idle animation here
        };
        rest.onStay = delegate {
            // Could add logic for recovery or waiting
        };
        rest.onExit = delegate { Debug.Log("In Rest.onExit"); };

        // Start the FSM in Seek_Waypoint state
        stateMachine.TransitionTo("Seek_Waypoint");
    }

    void Update()
    {
        // Debug current strength relation
        Debug.Log("StrongerThanPlayer = " + StrongerThanPlayer);
        stateMachine.Update();
    }
}
