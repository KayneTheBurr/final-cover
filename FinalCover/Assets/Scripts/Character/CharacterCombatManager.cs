using UnityEngine;

public class CharacterCombatManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Attack Type")]
    public AttackType currentAttackType;

    [Header("Attack Target")]
    public CharacterManager currentTarget;

    [Header("Last Attack Animation Performed")]
    public string lastAttackAnimation;

    [Header("Lock On Transform")]
    public Transform lockOnTransform;

    [Header("Attack Flags")]
    public bool canPerformRollingAttack = false;
    public bool canPerformBackStepAttack = false;
    public bool isInvulnerable = false;
    public bool isChargingAttack = false;
    public bool isLockedOn = false;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();

        if (lockOnTransform == null)
        {
            lockOnTransform = GetComponentInChildren<LockOnTarget>().GetComponent<Transform>();
        }
    }

    public virtual void SetTarget(CharacterManager newTarget)
    {
        if (newTarget != null)
        {
            currentTarget = newTarget;
        }
        else
        {
            currentTarget = null;
        }
    }
    public virtual void EnableCanDoCombo()
    {

    }
    public virtual void DisableCanDoCombo()
    {

    }
    public void EnableIFrames()
    {
        isInvulnerable = true;
    }
    public void DisableIFrames()
    {
        isInvulnerable = false;
    }
    public void EnableCanDoRollingAttack()
    {
        canPerformRollingAttack = true;
    }
    public void DisableCanDoRollingAttack()
    {
        canPerformRollingAttack = false;
    }
    public void EnableCanDoBackStepAttack()
    {
        canPerformBackStepAttack = true;
    }
    public void DisableCanDoBackStepAttack()
    {
        canPerformBackStepAttack = false;
    }

}
