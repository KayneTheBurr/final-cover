using UnityEngine;

public class PlayerCombatManager : CharacterCombatManager
{
    PlayerManager player;

    public WeaponItem currentWeaponBeingUsed;
    public ObservableVariable currentWeaponBeingUsedID = new ObservableVariable("");
    public ObservableVariable currentRightWeaponID = new ObservableVariable("");
    public ObservableVariable currentLeftWeaponID = new ObservableVariable("");
    public ObservableVariable isUsingRightHand = new ObservableVariable(false);
    public ObservableVariable isUsingLeftHand = new ObservableVariable(false);

    //public WeaponItem currentWeaponBeingUsed;
    public bool canComboWithMainHandWeapon = false;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }

    public override void EnableCanDoCombo()
    {
        base.EnableCanDoCombo();
        canComboWithMainHandWeapon = true;
    }

    public override void DisableCanDoCombo()
    {
        base.DisableCanDoCombo();
        canComboWithMainHandWeapon = false;
    }
    public override void SetTarget(CharacterManager newTarget)
    {
        base.SetTarget(newTarget);

        if (player)
        {
            //PlayerCamera.instance.SetNewTargetAsLookAtTarget(newTarget);
        }
    }
    public void SetCharacterActionHand(bool rightHandedAction)
    {
        if (rightHandedAction)
        {
            isUsingRightHand.SetBool(true);
            isUsingLeftHand.SetBool(false);
        }
        else
        {
            isUsingRightHand.SetBool(false);
            isUsingLeftHand.SetBool(true);
        }
    }
    public void OnCurrentRightHandWeaponIDChange(string oldID, string newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDataBase.instance.GetWeaponByID(newID));
        player.playerInventoryManager.currentRightHandWeapon = newWeapon;
        player.playerEquipmentManager.LoadRightWeapon();

        if (player)
        {
            PlayerUIManager.instance.playerHUDManager.SetRightWeaponQuickSlotIcon(newID);
        }
    }

    public void OnCurrentLeftHandWeaponIDChange(string oldID, string newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDataBase.instance.GetWeaponByID(newID));
        player.playerInventoryManager.currentLeftHandWeapon = newWeapon;
        player.playerEquipmentManager.LoadLeftWeapon();
        if (player)
        {
            PlayerUIManager.instance.playerHUDManager.SetLeftWeaponQuickSlotIcon(newID);
        }
    }

    public void OnCurrentWeaponBeingUsedIDChange(string oldID, string newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDataBase.instance.GetWeaponByID(newID));
        player.playerCombatManager.currentWeaponBeingUsed = newWeapon;

    }

    private void PerformWeaponAction(string actionID, string weaponID)
    {
        WeaponItemAction weaponAction = WorldActionManager.instance.GetWeaponItemActionByID(actionID);

        if (weaponAction != null)
        {
            weaponAction.AttemptToPerformAction(player, WorldItemDataBase.instance.GetWeaponByID(weaponID));
        }
        else
        {
            Debug.Log("Action is null, ERROR ERROR");
        }
    }
}
