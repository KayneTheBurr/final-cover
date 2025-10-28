using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;

    [HideInInspector] public PlayerHUDManager playerHUDManager;
    //[HideInInspector] public PlayerUIPopUpManager playerUIPopUpManager;

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

        playerHUDManager = GetComponentInChildren<PlayerHUDManager>();
        //playerUIPopUpManager = GetComponentInChildren<PlayerUIPopUpManager>();
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
