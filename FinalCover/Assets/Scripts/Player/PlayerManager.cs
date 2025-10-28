using UnityEngine;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public PlayerMovementManager playerMovementManager;
    [HideInInspector] public PlayerCombatManager playerCombatManager;
    [HideInInspector] public PlayerAnimationManager playerAnimatorManager;
    [HideInInspector] public PlayerStatManager playerStatsManager;
    [HideInInspector] public PlayerInventoryManager playerInventoryManager;
    [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;

    public ObservableVariable characterName = new ObservableVariable("");


    protected override void Awake()
    {
        //do character manager awake stuff 
        base.Awake();

        //after doing charcter stuff then player only stuff gets done next 
        playerMovementManager = GetComponent<PlayerMovementManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerAnimatorManager = GetComponent<PlayerAnimationManager>();
        playerStatsManager = GetComponent<PlayerStatManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        
    }

    protected override void Start()
    {
        base.Start();
        DontDestroyOnLoad(gameObject);

        InputManager.instance.player = this;
        PlayerCamera.instance.player = this;
        //WorldSaveGameManager.instance.player = this;

        PlayerCamera.instance.SetPlayerAsFollowTarget();

        AddAllVariableListeners();
    }

    protected override void Update()
    {
        base.Update();
        playerMovementManager.AllMovement();
        playerStatsManager.RegenerateStamina();
        playerStatsManager.RegenerateMana();
    }


    public void AddAllVariableListeners()
    {
        //change all the listeners from being added using network variables to using interfaces to track when things change?

        //when a player attribute is changed(heart, strength, agility, etc.)
        //call the CalculateHealthBasedOnHeart(int heart) function any time the variable changes:

        //    public int CalculateHealthBasedOnVitality(int vitality) //(example function)
        //    {float health = 0;
        //    health = vitality * 15;
        //    return Mathf.RoundToInt(health);}

        //on stamina/health/mana/hunger values changing, call a function
        //like( SetNewStaminaValue, ResetStaminaRegenTimer(), etc. will also need to send this to the UI for bars to update


        //when the weapon ID of the equiped weapons are changed looking at old/new IDs 

        //passing in states to the animator when things change: isMoving, isChargingAttack, isLockeOn, etc




    }


}
    

