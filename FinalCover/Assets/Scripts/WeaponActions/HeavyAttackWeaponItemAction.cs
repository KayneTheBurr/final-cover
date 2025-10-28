using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions /Heavy Attack")]
public class HeavyAttackWeaponItemAction : WeaponItemAction
{
    [Header("Heavy Attacks")]
    [SerializeField] string heavy_Attack_01 = "SS_Main_Heavy_Attack_01";
    [SerializeField] string heavy_Attack_02 = "SS_Main_Heavy_Attack_02";

    //[Header("Heavy Run Attacks")]
    //[SerializeField] string heavy_run_attack_01 = "SS_Main_Run_Attack_01";

    //[Header("Heavy Rolling Attacks")]
    //[SerializeField] string heavy_roll_attack_01 = "SS_Main_Roll_Attack_01";

    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);

        //check for anything that stops the action
        if (!playerPerformingAction) return;

        //if out of stamina cant attack
        if (playerPerformingAction.playerStatsManager.currentStamina.GetFloat() <= 0) return;

        //if player is not on the ground cant attack
        if (!playerPerformingAction.isGrounded) return;

        //if we are sprinting, perform a running attack 
        //if (playerPerformingAction.characterNetworkManager.isSprinting.Value)
        //{
        //    PerformHeavyRunningAttack(playerPerformingAction, weaponPerformingAction);
        //    return;
        //}

        //if we are rolling, perform a rolling attack 
        //if (playerPerformingAction.characterCombatManager.canPerformRollingAttack)
        //{
        //    PerformHeavyRollingAttack(playerPerformingAction, weaponPerformingAction);
        //    return;
        //}

        PerformHeavyAttack(playerPerformingAction, weaponPerformingAction);
    }
    private void PerformHeavyAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        //if we are attacking and we can perform a combo, perform the combo attack
        if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

            //figure out which attack we are currently performing, perform the next one
            //perform the attack based on the previous attack
            if (playerPerformingAction.playerCombatManager.lastAttackAnimation == heavy_Attack_01)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyAttack02, heavy_Attack_02, true);
            }

            else if (playerPerformingAction.playerCombatManager.lastAttackAnimation == heavy_Attack_02)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyAttack01, heavy_Attack_01, true);
            }
        }
        //otherwise perform a normal heavy attack
        else if (!playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyAttack01, heavy_Attack_01, true);

        }
    }
    private void PerformHeavyRunningAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        //if we are 2 handing, play 2 handing 
        //else perform one handed heavy running attack 

        //if (playerPerformingAction.playerNetworkManager.isUsingRightHand.Value)
        //{
        //    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyRunningAttack01, heavy_run_attack_01, true);
        //}
        //if (playerPerformingAction.playerNetworkManager.isUsingLeftHand.Value)
        //{

        //}
    }
    private void PerformHeavyRollingAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        //if we are 2 handing, play 2 handing 
        //else perform one handed heavy running attack 

        //if (playerPerformingAction.playerNetworkManager.isUsingRightHand.Value)
        //{
        //    playerPerformingAction.playerCombatManager.canPerformRollingAttack = false;
        //    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyRollingAttack01, heavy_roll_attack_01, true);
        //}
        //if (playerPerformingAction.playerNetworkManager.isUsingLeftHand.Value)
        //{

        //}
    }

}
