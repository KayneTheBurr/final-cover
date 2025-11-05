using UnityEngine;

[CreateAssetMenu(menuName = "AI / States / Idle")]
public class IdleState : AIStates
{
    public override AIStates Tick(EnemyCharacterManager enemy)
    {
        if (enemy.characterCombatManager.currentTarget != null)
        {
            //return the pursue target state instead

            return SwitchState(enemy, enemy.pursueTarget);
        }
        else
        {
            //return the same (idle) state, keep searching for a target
            enemy.enemyCombatManager.FindATargetViaLineOfSight(enemy);
            return this;
        }

    }
}
