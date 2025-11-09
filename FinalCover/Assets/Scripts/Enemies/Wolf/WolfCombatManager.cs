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

    [Header("Wolf Turn Values")]
    [SerializeField] string turnLeftState = "Wolf_Combat_Turn_L";
    [SerializeField] string turnRightState = "Wolf_Combat_Turn_R";
    [SerializeField] string quickTurnState = "Wolf_QuickTurn_Slash";
    [SerializeField] AnimationClip turn90Clip;    // 90 degree turn anim
    [SerializeField] float desired90Time = 0.35f; // speed of turning 90 deg
    [SerializeField] float turnThreshold = 40f;   // start in-place turn if abs(angle) >= this
    [SerializeField] float quickTurnThreshold = 150f;
    [SerializeField] float quickTurnRange = 2.5f;
    [SerializeField] float quickTurnCooldown = 3f;
    private float _nextQuickTurn;

    public override void PivotTowardsTarget(EnemyCharacterManager wolfChar)
    {
        //dont include base, want to do different things
        
        if (currentTarget == null) return;
        if (wolfChar.isPerformingAction) return;

        Vector3 dir = currentTarget.transform.position - wolfChar.transform.position;
        float angle = WorldUtilityManager.instance.GetAngleOfTarget(wolfChar.transform, dir);
        float absA = Mathf.Abs(angle);

        // Quick turn when the player is close enough and right behind them 
        if (absA >= quickTurnThreshold && distanceFromTarget <= quickTurnRange) //&& Time.time >= _nextQuickTurn
        {
            Debug.Log("Do a quickturn?");
            wolfChar.characterAnimationManager.PlayTargetActionAnimation(quickTurnState, true);

            wolfChar.isPerformingAction = true;
            wolfChar.canRotate = false;
            actionRecoveryTimer = 0.5f;
            return;
        }

        // 2) Regular in-place turn (pick side by sign)
        if (absA >= turnThreshold)
        {
            Debug.Log("Turn Normally?");
            string state = angle >= 0f ? turnRightState : turnLeftState;
            wolfChar.animator.speed = SpeedForTurn(Mathf.Min(absA, 90f));
            wolfChar.characterAnimationManager.PlayTargetActionAnimation(state, true);

            wolfChar.isPerformingAction = true;
            wolfChar.canRotate = false;
            actionRecoveryTimer = desired90Time * 0.5f;
        }
    }
    public float SpeedForTurn(float angleDeg)
    {
        if (!turn90Clip) return 1f;
        float targetTime = desired90Time * (angleDeg / 90f);
        float baseLen = Mathf.Max(turn90Clip.length, 0.001f);
        return Mathf.Clamp(baseLen / targetTime, 0.5f, 3f);
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
