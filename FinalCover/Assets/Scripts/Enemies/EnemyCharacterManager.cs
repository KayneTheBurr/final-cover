using System.Globalization;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCharacterManager : CharacterManager
{
    [HideInInspector] public EnemyCombatManager enemyCombatManager;
    [HideInInspector] public EnemyAnimationManager enemyAnimationManager;
    [HideInInspector] public EnemyMovementManager enemyMovementManager;
    [HideInInspector] public EnemyStatsManager enemyStatsManager;

    [Header("Character Name")]
    public string characterName = "";

    [Header("NavMesh Agent")]
    public NavMeshAgent navMeshAgent;

    [Header("Current State")]
    [SerializeField] AIStates currentState;

    [Header("States")]
    public IdleState idle;
    public PursueTargetState pursueTarget;
    public CombatStanceState combatStance;
    public AttackState attack;

    protected override void Awake()
    {
        base.Awake();

        navMeshAgent = GetComponentInChildren<NavMeshAgent>();

        enemyCombatManager = GetComponent<EnemyCombatManager>();
        enemyAnimationManager = GetComponent<EnemyAnimationManager>();
        enemyMovementManager = GetComponent<EnemyMovementManager>();
        enemyStatsManager = GetComponent<EnemyStatsManager>();

        idle = Instantiate(idle);
        pursueTarget = Instantiate(pursueTarget);

        currentState = idle;
    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void OnEnable()
    {
        base.OnEnable();



    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }
    private void ProcessStateMachine()
    {
        //check if current state is NOT null, and if so, run the TICK function. THis is called in fixed update. 
        AIStates nextState = currentState?.Tick(this);

        if (nextState != null)
        {
            currentState = nextState;
        }

        //reset the position/rotation after teh state machine has processed it
        navMeshAgent.transform.localPosition = Vector3.zero;
        navMeshAgent.transform.localRotation = Quaternion.identity;

        if (enemyCombatManager.currentTarget != null)
        {
            enemyCombatManager.distanceFromTarget = Vector3.Distance(enemyCombatManager.currentTarget.transform.position, transform.position);
            enemyCombatManager.targetDirection = enemyCombatManager.currentTarget.transform.position - transform.position;
            enemyCombatManager.viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, enemyCombatManager.targetDirection);
        }


        if (navMeshAgent.enabled)
        {
            Vector3 agentDestination = navMeshAgent.destination;
            float remainingDistance = Vector3.Distance(agentDestination, transform.position);

            if (remainingDistance > navMeshAgent.stoppingDistance)
            {
                enemyMovementManager.isMoving.SetBool(true);
            }
            else
            {
                enemyMovementManager.isMoving.SetBool(false);
            }
        }
        else
        {
            enemyMovementManager.isMoving.SetBool(false);
        }


    }
    protected override void Update()
    {
        base.Update();
        enemyCombatManager.HandleActionRecovery(this);
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        ProcessStateMachine();
    }

}
