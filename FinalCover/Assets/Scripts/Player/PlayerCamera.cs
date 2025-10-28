using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;


public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    //public Camera cam; //if NOT using cinemachine
    public CinemachineCamera vCam; //if using cinemachine
    public PlayerManager player;

    private List<CharacterManager> availableTargets = new List<CharacterManager>();
    public CharacterManager nearestLockOnTarget;
    public CharacterManager leftLockOnTarget;
    public CharacterManager rightLockOnTarget;

    private void Awake()
    {
        //one at a time 
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerAsFollowTarget()
    {
        vCam.Target.TrackingTarget = player.gameObject.transform;
        vCam.Target.LookAtTarget = player.gameObject.transform;
    }
}
