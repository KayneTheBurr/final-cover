using UnityEngine;

public class Enums : MonoBehaviour
{}

public enum RotationMode
{
    SoulsLike,  //souls like direction depends on camera direction
    DiabloLike      //poe/diablo style with fixed camera
}
public enum CharacterGroup
{
    Friendly,
    Enemy
}
public enum WeaponClass
{
    Fists,
    Katana, 
    Greatsword,
    Bow
}
public enum AttackType 
{
    LightAttack01,
    LightAttack02,
    LightAttack03,
    LightAttack04,
    HeavyAttack01,
    HeavyAttack02,
    HeavyAttack03,
    ChargeAttack01,
    ChargeAttack02,
    LightRunningAttack01,
    LightRollingAttack01,
    LightBackStepAttack01
}

public enum WeaponModelSlot
{
    RightHand,
    LeftHand//,
    //RightHip,
    //LeftHip,
    //Back
}