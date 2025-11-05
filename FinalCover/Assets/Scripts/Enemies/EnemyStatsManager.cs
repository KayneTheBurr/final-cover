using UnityEngine;
using UnityEngine.TextCore.Text;

public class EnemyStatsManager : CharacterStatManager
{
    EnemyCharacterManager enemy;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<EnemyCharacterManager>();
    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        currentHealth.OnFloatChanged += CheckHP;

        if (enemy.characterUIManager.hasFloatingHPBar)
            currentHealth.OnFloatChanged += enemy.characterUIManager.OnHPChanged;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        currentHealth.OnFloatChanged -= CheckHP;

        if (enemy.characterUIManager.hasFloatingHPBar)
            currentHealth.OnFloatChanged -= enemy.characterUIManager.OnHPChanged;
    }
}
