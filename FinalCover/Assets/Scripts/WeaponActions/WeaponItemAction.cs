using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Action")]
public class WeaponItemAction : ScriptableObject
{
    public string actionID;

    public virtual void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        //we should always keep track of what weapon a player is using 
        if (playerPerformingAction)
        {
            playerPerformingAction.playerCombatManager.currentWeaponBeingUsedID.SetString(weaponPerformingAction.itemID);
        }

    }
}
