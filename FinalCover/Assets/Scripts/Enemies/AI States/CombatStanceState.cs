using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AI / States / Combat Stance")]
public class CombatStanceState : AIStates
{
    //1. Select an attack for the attack state based on: DIstance, Angle, Chance/weight, probability etc
    //2. process combat logic while waiting to attack(dodge/block/strafe etc)
    //3. if target moves out of range, return to pursue state to chase
    //4. if target is too far away/no longer there, switch to idle state

    [Header("Attacks")]
    public List<EnemyAttackAction> aiCharacterAttacks; //list of all possible attacks the character CAN do
    private List<EnemyAttackAction> potentialAttacks; //all attacks possible in the situation based on angle/distance
    [SerializeField] private EnemyAttackAction chosenAttack;
    [SerializeField] private EnemyAttackAction previousAttack;
    protected bool hasAttacked = false;

    [Header("Combo")]
    [SerializeField] protected bool canPerformCombo = false;
    [SerializeField] protected int chanceToPerfromCombo = 25; //0-100 for if enemy will perform combo attack 
    protected bool hasRolledForComboChance = false;

    [Header("Engagement Distance")]
    public float maxEngagementDistance = 5; //The distance we have to be away from target before returning to pursue state

    public override AIStates Tick(EnemyCharacterManager enemy)
    {
        if (enemy.isPerformingAction) return this;

        if (!enemy.navMeshAgent.enabled)
            enemy.navMeshAgent.enabled = true; //turn on nev mesh if it was off 

        //get the ai character to turn and face the target when its outisde its FOV
        if (!enemy.enemyMovementManager.isMoving.GetBool())
        {
            if (enemy.enemyCombatManager.viewableAngle < -35 || enemy.enemyCombatManager.viewableAngle < 35)
            {
                enemy.enemyCombatManager.PivotTowardsTarget(enemy);
            }
        }

        //rotate to face our target
        enemy.enemyCombatManager.RotateTowardsAgent(enemy);

        //if our target is no longer there, switch back to idle 
        if (enemy.enemyCombatManager.currentTarget == null)
            return SwitchState(enemy, enemy.idle);

        if (!hasAttacked)
        {
            GetNewAttack(enemy);
        }
        else
        {
            enemy.attack.currentAttack = chosenAttack;

            //roll for combo chance or other outcome

            return SwitchState(enemy, enemy.attack);
        }

        //if we get outside of the combat state range, return to pursue state
        if (enemy.enemyCombatManager.distanceFromTarget > maxEngagementDistance)
            return SwitchState(enemy, enemy.pursueTarget);

        NavMeshPath path = new NavMeshPath();
        enemy.navMeshAgent.CalculatePath(enemy.enemyCombatManager.currentTarget.transform.position, path);
        enemy.navMeshAgent.SetPath(path);
        return this;
    }
    protected virtual void GetNewAttack(EnemyCharacterManager aiCharacter)
    {
        potentialAttacks = new List<EnemyAttackAction>();
        foreach (var attack in aiCharacterAttacks)
        {
            //if we are too close, then continue onto the next attack choice
            if (attack.minAttackDistance > aiCharacter.enemyCombatManager.distanceFromTarget) continue;

            //if we are too far away, then continue onto the next attack choice
            if (attack.maxAttackDistance < aiCharacter.enemyCombatManager.distanceFromTarget) continue;

            //check the attack angle, if outside FOV for the attack check the next attack
            if (attack.minAttackAngle > aiCharacter.enemyCombatManager.distanceFromTarget) continue;
            if (attack.maxAttackAngle < aiCharacter.enemyCombatManager.distanceFromTarget) continue;

            potentialAttacks.Add(attack);
        }
        if (potentialAttacks.Count <= 0)
        {
            Debug.Log("no attack option");
            return;
        }
        var totalWeight = 0;

        foreach (var attack in potentialAttacks)
        {
            totalWeight += attack.attackWeight;
        }
        var randomWeight = Random.Range(1, totalWeight + 1);
        var processedWeight = 0;

        foreach (var attack in potentialAttacks)
        {
            processedWeight += attack.attackWeight;
            if (randomWeight <= processedWeight)
            {
                chosenAttack = attack;
                previousAttack = chosenAttack;
                hasAttacked = true;
                return;
            }
        }

        // pick one of the remaining attacks randomly form list 
    }
    protected virtual bool RollForOutcomeChance(int outcomeChance)
    {
        bool outcomeWillBePerformed = false;
        int randomPercentage = Random.Range(0, 100);
        if (randomPercentage < outcomeChance)
        {
            outcomeWillBePerformed = true;
        }
        return outcomeWillBePerformed;
    }
    protected override void ResetStateFlags(EnemyCharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);

        hasRolledForComboChance = false;
        hasAttacked = false;
    }
}
