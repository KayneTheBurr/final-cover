using UnityEngine;

public class WolfCombatManager : EnemyCombatManager
{
    [Header("Damage Colliders")]
    [SerializeField] WolfDamageCollider teethDamageCollider;
    [SerializeField] WolfDamageCollider rightClawDamageCollider;
    [SerializeField] WolfDamageCollider leftClawDamageCollider;

    [Header("Damage")]
    [SerializeField] float physicalDamage = 15;
    [SerializeField] float poisonDamage = 10;
    [SerializeField] float biteAttack_01_DamageModifier = 1f;
    [SerializeField] float swipeAttack_01_DamageModifier = 1.2f;
    [SerializeField] float swipeAttack_02_DamageModifier = 1.5f;


    public override void PivotTowardsTarget(EnemyCharacterManager aICharacter)
    {
        //dont include base, want to do different things 

        //use angles and distance to determine which way to turn

        //if they are in a certain area right behind, do the quick turn + aoe 

    }
    public void SetBiteAttack01Damage()
    {
        teethDamageCollider.physicalDamage = physicalDamage * biteAttack_01_DamageModifier;
        teethDamageCollider.poisonDamage = poisonDamage * biteAttack_01_DamageModifier;

    }
    public void OpenTeethDamageCollider()
    {
        teethDamageCollider.EnableDamageCollider();
    }
    public void CloseTeethDamageCollider()
    {
        teethDamageCollider.DisableDamageCollider();
    }
    public void OpenRightHandDamageCollider()
    {
        rightClawDamageCollider.EnableDamageCollider();
    }

    public void CloseRightHandDamageCollider()
    {
        rightClawDamageCollider.DisableDamageCollider();
    }

    public void OpenLeftHandDamageCollider()
    {
        leftClawDamageCollider.EnableDamageCollider();
    }

    public void CloseLeftHandDamageCollider()
    {
        leftClawDamageCollider.DisableDamageCollider();
    }
}
