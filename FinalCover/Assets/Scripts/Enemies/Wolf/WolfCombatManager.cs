using UnityEngine;

public class WolfCombatManager : EnemyCombatManager
{
    [Header("Damage Colliders")]
    [SerializeField] WolfDamageCollider teethDamageCollider;
    [SerializeField] WolfDamageCollider rightClawDamageCollider;
    [SerializeField] WolfDamageCollider leftClawDamageCollider;

    [Header("Damage")]
    [SerializeField] float physicalDamage = 15;
    [SerializeField] float chemicalDamage = 10;
    [SerializeField] float biteAttack_01_DamageModifier = 1f;
    [SerializeField] float swipeAttack_01_DamageModifier = 1.2f;
    [SerializeField] float swipeAttack_02_DamageModifier = 1.5f;

    public void OpenTeetchDamageCollider()
    {
        teethDamageCollider.EnableDamageCollider();
    }
    public void CloseTeetchDamageCollider()
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
