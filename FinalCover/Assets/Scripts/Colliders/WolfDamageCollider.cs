using UnityEngine;

public class WolfDamageCollider : DamageCollider
{
    public EnemyCharacterManager wolfCharacter;

    protected override void Awake()
    {
        base.Awake();
        damageCollider = GetComponent<Collider>();
        wolfCharacter = GetComponentInParent<EnemyCharacterManager>();
    }

    protected override void DamageTarget(CharacterManager damageTarget)
    {
        if (charactersDamaged.Contains(damageTarget)) return;
        charactersDamaged.Add(damageTarget);

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);

        damageEffect.physicalDamage = physicalDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.lightningDamage = lightningDamage;
        damageEffect.iceDamage = iceDamage;
        damageEffect.poisonDamage = poisonDamage;
        damageEffect.shadowDamage = shadowDamage;
        damageEffect.decayDamage = decayDamage;

        damageEffect.contactPoint = contactPoint;
        damageEffect.angleHitFrom = Vector3.SignedAngle(wolfCharacter.transform.forward, damageTarget.transform.forward, Vector3.up);

        Debug.Log("Deal Damage!");

        damageTarget.characterEffectsManager.ProcessInstantEffects(damageEffect);
    }
}
