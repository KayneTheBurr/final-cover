using UnityEngine;

public class PlayerStatManager : CharacterStatManager
{
    PlayerManager player;

    [Header("Player Attributes")]
    public ObservableVariable heart = new ObservableVariable(10);
    public ObservableVariable strength = new ObservableVariable(10);
    public ObservableVariable agility = new ObservableVariable(10);
    public ObservableVariable arcana = new ObservableVariable(10);
    public ObservableVariable essence = new ObservableVariable(10);
    public ObservableVariable cunning = new ObservableVariable(10);
    public ObservableVariable omen = new ObservableVariable(10);


    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }
    protected override void Start()
    {
        base.Start();

        //why calc here? when we spawn the character in need to calculate the stats iniitally until the 
        
    }
    protected override void OnEnable()
    {
        currentStamina.OnFloatChanged += ResetManaRegenTimer;
        currentMana.OnFloatChanged += ResetManaRegenTimer;
        
    }
    protected override void OnDisable()
    {
        currentStamina.OnFloatChanged -= ResetManaRegenTimer;
        currentMana.OnFloatChanged -= ResetManaRegenTimer;
    }
    public void SetNewMaxHealthValue(int oldValue, int newValue)
    {
        maxHealth.SetInt(CalculateHealthBasedOnVitality(newValue));
        PlayerUIManager.instance.playerHUDManager.SetMaxHealthValue(maxHealth.GetInt());
        currentHealth.SetFloat(maxHealth.GetInt());
    }
    public void SetNewMaxStaminaValue(int oldValue, int newValue)
    {
        maxStamina.SetInt(CalculateStaminaBasedOnEndurance(newValue));
        PlayerUIManager.instance.playerHUDManager.SetMaxStaminaValue(maxStamina.GetInt());
        currentStamina.SetFloat(maxStamina.GetInt());
    }
}
