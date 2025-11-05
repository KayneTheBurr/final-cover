using UnityEngine;

[CreateAssetMenu(menuName = "AI / Actions / Attack ")]
public class EnemyAttackAction : ScriptableObject
{
    [Header("Attack")]
    [SerializeField] private string attackAnimation;

    [Header("Combo Actions")]
    public EnemyAttackAction comboAction; //the combo action of this attack action

    [Header("Action Values")]
    public int attackWeight;
    [SerializeField] AttackType attackType;
    //attack can be repeated
    public float actionRecoveryTime = 1.5f;
    public float minAttackAngle = -35;
    public float maxAttackAngle = 35;
    public float minAttackDistance = 0;
    public float maxAttackDistance = 3;

    public void AttemptToPerformAction(EnemyCharacterManager enemy)
    {
        enemy.characterAnimationManager.PlayTargetActionAnimation(attackAnimation, true);
    }
}
