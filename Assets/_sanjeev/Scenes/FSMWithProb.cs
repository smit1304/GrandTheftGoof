using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(VisionDetector))]
[RequireComponent(typeof(NavMeshAgent))]
public class FSMWithProb : MonoBehaviour
{
    [Header("Target / Movement")]
    public GameObject player;             // Player reference
    public float distanceToChase = 10f;   // Distance to start chasing
    public float distanceToAttack = 1f;   // Distance to start attacking
    public float speed = 1.106003f;            // Base movement speed for NavMeshAgent
    public float chaseSpeed = 1.2f;            // Base movement speed for NavMeshAgent

    private VisionDetector visionDetector;
    private NavMeshAgent agent;
    private StateMachine stateMachine;

    [System.Serializable]
    private class WeightedState
    {
        public string stateName;
        public float weight;
    }

    [Header("Random choice when target is NOT visible")]
    [Tooltip("Relative chance to pick Patrol when the NPC loses sight of the player.")]
    public float patrolWeight = 3f;

    [Tooltip("Relative chance to pick Rest when the NPC loses sight of the player.")]
    public float restWeight = 1f;

    private WeightedState[] noTargetStates;

    [Header("Idle decision timing")]
    [Tooltip("How often (in seconds) the NPC re-decides Patrol/Rest when player is not visible.")]
    public float idleDecisionInterval = 5f;
    private float idleDecisionTimer;

    [Header("Power-up reaction")]
    [Tooltip("NPCs only evade if they are within this distance from the powered-up player.")]
    public float powerUpEvadeRadius = 15f;

    // Cached flag for player's power up
    private bool isPlayerPoweredUp = false;

    public static Action OnPlayerCaught;

    private void Start()
    {
        stateMachine = new StateMachine();
        visionDetector = GetComponent<VisionDetector>();
        agent = GetComponent<NavMeshAgent>();

        // Use our speed value on the NavMeshAgent
        agent.speed = speed;
        agent.stoppingDistance = distanceToAttack * 0.9f; // small buffer

        // Try to find player by tag if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj;
            }
        }

        // Build weighted states in code (no strings in Inspector)
        noTargetStates = new[]
        {
            new WeightedState { stateName = "Patrol", weight = patrolWeight },
            new WeightedState { stateName = "Rest",   weight = restWeight }
        };

        // Subscribe to per-NPC vision events
        visionDetector.OnTargetSpotted += HandleTargetSpotted;
        visionDetector.OnTargetLost += HandleTargetLost;

        // Subscribe to global power-up events
        EventManager.OnPlayerPowerUpStarted += HandlePlayerPowerUpStart;
        EventManager.OnPlayerPowerUpEnded += HandlePlayerPowerUpEnd;

        // ----------------- DEFINE STATES -----------------

        // EVALUATE TARGET: decide Chase vs Attack depending on distance
        var evaluateTarget = stateMachine.CreateState("EvaluateTarget");
        evaluateTarget.onEnter = delegate
        {
            Debug.Log("FSM: Enter EvaluateTarget");
            agent.isStopped = false;
        };
        evaluateTarget.onStay = delegate
        {
            // If nearby powered-up player, always evade
            if (ShouldEvadePowerUp())
            {
                stateMachine.TransitionTo("Evade");
                return;
            }

            if (player == null || visionDetector == null) return;

            // Only make decisions if we see the player
            if (!visionDetector.isTargetVisible)
                return;

            float distanceToPlayer =
                Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= distanceToAttack)
            {
                stateMachine.TransitionTo("Attack");
            }
            else if (distanceToPlayer <= distanceToChase)
            {
                // always chase when in range
                stateMachine.TransitionTo("Chase");
            }
        };
        evaluateTarget.onExit = delegate { Debug.Log("FSM: Exit EvaluateTarget"); };

        // CHASE: use NavMeshAgent to move towards player
        var chase = stateMachine.CreateState("Chase");
        chase.onEnter = delegate
        {
            Debug.Log("FSM: Enter Chase");
            agent.isStopped = false;
            agent.speed = chaseSpeed;
        };
        chase.onStay = delegate
        {
            if (ShouldEvadePowerUp())
            {
                stateMachine.TransitionTo("Evade");
                return;
            }

            if (player == null) return;

            float dist = Vector3.Distance(transform.position, player.transform.position);

            // If we're in attack range, switch to Attack instead of keeping the chase state
            if (dist <= distanceToAttack)
            {
                stateMachine.TransitionTo("Attack");
                return;
            }

            // Tell the NavMeshAgent to chase the player
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
        };
        chase.onExit = delegate { agent.speed = speed; Debug.Log("FSM: Exit Chase"); };

        // ATTACK: placeholder for attack logic / animation
        var attack = stateMachine.CreateState("Attack");
        attack.onEnter = delegate
        {
            Debug.Log("FSM: Enter Attack");
            // stop at attack range
            agent.isStopped = true;
            agent.ResetPath();

            OnPlayerCaught?.Invoke();
        };
        attack.onStay = delegate
        {
            if (ShouldEvadePowerUp())
            {
                stateMachine.TransitionTo("Evade");
                return;
            }

            // Optional: face player
            if (player != null)
            {
                Vector3 lookDir = (player.transform.position - transform.position);
                lookDir.y = 0f;
                if (lookDir.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.LookRotation(lookDir.normalized),
                        Time.deltaTime * 5f
                    );
                }
            }

            // If player moved away > attack range, go back to EvaluateTarget
            if (player != null)
            {
                float dist = Vector3.Distance(transform.position, player.transform.position);
                if (dist > distanceToAttack * 1.2f) // small hysteresis
                {
                    stateMachine.TransitionTo("EvaluateTarget");
                }
            }
        };
        attack.onExit = delegate { Debug.Log("FSM: Exit Attack"); };

        // EVADE: run away while player is (dangerously close and) powered; when it ends, return to idle / evaluate
        var evade = stateMachine.CreateState("Evade");
        evade.onEnter = delegate
        {
            Debug.Log("FSM: Enter Evade");
            agent.isStopped = false;
        };
        evade.onStay = delegate
        {
            if (player == null) return;

            // Always run away from the player.
            Vector3 fromPlayer = transform.position - player.transform.position;
            Vector3 dir = fromPlayer.normalized;

            float evadeDistance = distanceToChase; // how far to run away
            Vector3 rawTargetPos = transform.position + dir * evadeDistance;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(rawTargetPos, out hit, 2f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            else
            {
                agent.SetDestination(rawTargetPos);
            }

            // If power up ended OR we got far enough, decide what to do next:
            if (!isPlayerPoweredUp || !IsPlayerWithinPowerRadius())
            {
                // If we can see the player, go back to EvaluateTarget
                if (visionDetector != null && visionDetector.isTargetVisible)
                {
                    stateMachine.TransitionTo("EvaluateTarget");
                }
                else
                {
                    // Otherwise, go to idle (Patrol/Rest) via helper
                    GoToRandomIdleState();
                }
            }
        };
        evade.onExit = delegate { Debug.Log("FSM: Exit Evade"); };

        // PATROL: simple "walk forward" using NavMesh
        var patrol = stateMachine.CreateState("Patrol");
        patrol.onEnter = delegate
        {
            Debug.Log("FSM: Enter Patrol");
            agent.isStopped = false;
            ResetIdleDecisionTimer();
            SetForwardPatrolDestination();
        };
        patrol.onStay = delegate
        {
            if (ShouldEvadePowerUp())
            {
                stateMachine.TransitionTo("Evade");
                return;
            }

            // If we've reached our destination (or close), pick another small step forward
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
            {
                SetForwardPatrolDestination();
            }

            // Handle 5s re-decision for Patrol/Rest while player not visible
            UpdateIdleDecision();
        };
        patrol.onExit = delegate { Debug.Log("FSM: Exit Patrol"); };

        // REST: stand still / idle
        var rest = stateMachine.CreateState("Rest");
        rest.onEnter = delegate
        {
            Debug.Log("FSM: Enter Rest");
            agent.isStopped = true;
            agent.ResetPath();
            ResetIdleDecisionTimer();
        };
        rest.onStay = delegate
        {
            if (ShouldEvadePowerUp())
            {
                stateMachine.TransitionTo("Evade");
                return;
            }

            // Handle 5s re-decision for Patrol/Rest while player not visible
            UpdateIdleDecision();
        };
        rest.onExit = delegate { Debug.Log("FSM: Exit Rest"); };

        // Start in Patrol by default
        stateMachine.TransitionTo("Patrol");
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void OnDestroy()
    {
        if (visionDetector != null)
        {
            visionDetector.OnTargetSpotted -= HandleTargetSpotted;
            visionDetector.OnTargetLost -= HandleTargetLost;
        }

        EventManager.OnPlayerPowerUpStarted -= HandlePlayerPowerUpStart;
        EventManager.OnPlayerPowerUpEnded -= HandlePlayerPowerUpEnd;
    }

    // ===================== Event handlers =====================

    private void HandleTargetSpotted()
    {
        if (ShouldEvadePowerUp())
        {
            stateMachine.TransitionTo("Evade");
            return;
        }

        Debug.Log("FSM: Target spotted → EvaluateTarget");
        stateMachine.TransitionTo("EvaluateTarget");
    }

    private void HandleTargetLost()
    {
        if (ShouldEvadePowerUp())
        {
            stateMachine.TransitionTo("Evade");
            return;
        }

        Debug.Log("FSM: Target lost → idle decision (Patrol/Rest)");
        GoToRandomIdleState();
    }

    private void HandlePlayerPowerUpStart()
    {
        isPlayerPoweredUp = true;
        Debug.Log("FSM: Player power-up started");

        // Only immediately evade if this NPC is near the player
        if (IsPlayerWithinPowerRadius())
        {
            Debug.Log("FSM: Player power-up nearby → Evade");
            stateMachine.TransitionTo("Evade");
        }
    }

    private void HandlePlayerPowerUpEnd()
    {
        isPlayerPoweredUp = false;
        Debug.Log("FSM: Player power-up ended.");
        // Evade state handles exiting when this flag is false.
    }

    // ===================== Helpers =====================

    private bool IsPlayerWithinPowerRadius()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.transform.position) <= powerUpEvadeRadius;
    }

    private bool ShouldEvadePowerUp()
    {
        return isPlayerPoweredUp && IsPlayerWithinPowerRadius();
    }

    // Called when we enter an idle state (Patrol/Rest)
    private void ResetIdleDecisionTimer()
    {
        idleDecisionTimer = idleDecisionInterval;
    }

    // Called every frame in Patrol/Rest to handle "every 5 seconds, decide again"
    private void UpdateIdleDecision()
    {
        // Only care about re-deciding if we DON’T see the player
        if (visionDetector != null && visionDetector.isTargetVisible)
            return;

        if (idleDecisionInterval <= 0f)
            return;

        idleDecisionTimer -= Time.deltaTime;

        if (idleDecisionTimer <= 0f)
        {
            GoToRandomIdleState();
        }
    }

    // Decide Patrol vs Rest using weighted randomness
    private void GoToRandomIdleState()
    {
        string nextState = PickRandomWeightedState(noTargetStates);
        if (!string.IsNullOrEmpty(nextState))
        {
            ResetIdleDecisionTimer();
            stateMachine.TransitionTo(nextState);
        }
        else
        {
            // Fallback if something is misconfigured
            ResetIdleDecisionTimer();
            stateMachine.TransitionTo("Patrol");
        }
    }

    // pick a point a little bit forward and use NavMeshAgent to move there
    private void SetForwardPatrolDestination()
    {
        float step = 3f;
        Vector3 rawTarget = transform.position + transform.forward * step;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(rawTarget, out hit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            agent.SetDestination(rawTarget);
        }
    }

    // Picks a state name from the given list, using their weights as probabilities.
    private string PickRandomWeightedState(WeightedState[] states)
    {
        if (states == null || states.Length == 0)
        {
            Debug.LogWarning("FSM: No weighted states configured.");
            return null;
        }

        float total = 0f;
        for (int i = 0; i < states.Length; i++)
        {
            if (states[i].weight > 0f)
                total += states[i].weight;
        }

        if (total <= 0f)
        {
            Debug.LogWarning("FSM: All state weights are zero or negative.");
            return null;
        }

        float r = UnityEngine.Random.value * total;
        float cumulative = 0f;

        for (int i = 0; i < states.Length; i++)
        {
            if (states[i].weight <= 0f)
                continue;

            cumulative += states[i].weight;
            if (r <= cumulative)
            {
                return states[i].stateName;
            }
        }

        // Fallback – should not be hit
        return states[states.Length - 1].stateName;
    }
}
