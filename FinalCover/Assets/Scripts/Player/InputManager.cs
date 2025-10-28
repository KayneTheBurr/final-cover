using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    PlayerControls playerControls;
    public PlayerManager player;

    [Header("Player Movement")]
    public Vector2 movement_Input;
    public float horizontal_Input;
    public float vertical_Input;
    public float moveAmount;

    [Header("Bumper (R1/L1) Inputs")]
    [SerializeField] bool lightAttck_Input = false;
    [SerializeField] bool heavyAttack_Input = false;
    [SerializeField] bool charge_HA_Input = false;

    [Header("Player Action Inputs")]
    [SerializeField] bool dodge_Input = false;
    [SerializeField] bool sprint_Input = false;
    [SerializeField] bool jump_Input = false;

    [Header("Qued Inputs")]
    [SerializeField] float que_Input_Timer = 0;
    [SerializeField] float default_Que_Input_Timer = 0.5f;
    [SerializeField] bool input_Que_Active = false;
    [SerializeField] bool qued_LA_input = false;
    [SerializeField] bool qued_HA_input = false;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnSceneChange(Scene oldScene, Scene newScene)
    {

        //when moving from menu scene to main gameplay scene, enable controls
        if (newScene.buildIndex == 1)
        {
            instance.enabled = true;
            if (playerControls != null)
            {
                playerControls.Enable();
            }
        }
        else
        {
            instance.enabled = false;
            if (playerControls != null)
            {
                playerControls.Disable();
            }
        }
    }
    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Move.performed += i => movement_Input = i.ReadValue<Vector2>();
            //playerControls.PlayerCamera.CameraControls.performed += i => camera_Input = i.ReadValue<Vector2>();

            //Movement Actions
            playerControls.PlayerMovement.Dodge.performed += i => dodge_Input = true;
            playerControls.PlayerMovement.Jump.performed += i => jump_Input = true;

            //Sprint Holds
            playerControls.PlayerMovement.Sprint.performed += i => sprint_Input = true;
            playerControls.PlayerMovement.Sprint.canceled += i => sprint_Input = false;

            //Attack Actions
            playerControls.PlayerActions.HeavyAttack.performed += i => heavyAttack_Input = true;
            playerControls.PlayerActions.ChargedHeavyAttack.performed += i => charge_HA_Input = true;
            playerControls.PlayerActions.ChargedHeavyAttack.canceled += i => charge_HA_Input = false;

            //Qued Inputs
            playerControls.PlayerActions.QuedLightAttack.performed += i => QuedInput(ref qued_LA_input);
            playerControls.PlayerActions.QuedHeavyAttack.performed += i => QuedInput(ref qued_HA_input);
        }
    }
    private void OnApplicationFocus(bool focus)
    {
        if (enabled)
        {
            if (focus)
            {
                playerControls.Enable();
            }
            else
            {
                playerControls.Disable();
            }
        }
    }
    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += OnSceneChange;
        instance.enabled = false;

        if (playerControls != null)
        {
            playerControls.Disable();
        }
    }
    private void Update()
    {
        HandleMovementInput();
        HandleLightAttackInput();
        HandleHeavyAttackInput();
        //HandleDodgeInput();
        //HandleSprintInput();
        //HandleJumpInput();
    }
    private void HandleMovementInput()
    {
        vertical_Input = movement_Input.y;
        horizontal_Input = movement_Input.x;

        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 && moveAmount <= 1)
        {
            moveAmount = 1.0f;
        }

        if (player == null) return;

        if (moveAmount != 0)
        {
            player.playerMovementManager.isMoving.SetBool(true);
        }
        else
        {
            player.playerMovementManager.isMoving.SetBool(false);
        }

        if (player.playerCombatManager.isLockedOn || player.playerMovementManager.isSprinting)
        {
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontal_Input, vertical_Input, player.playerMovementManager.isSprinting);
        }
        else
        {
            //only pass vertical since strafing is only while locked on, want to only run forward with camera movement right now 
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerMovementManager.isSprinting);
        }

    }

    private void HandleLightAttackInput()
    {
        if (lightAttck_Input)
        {
            lightAttck_Input = false;
            //do nothing if UI window is open 

            //do attack 

        }
    }
    private void HandleHeavyAttackInput()
    {
        if (heavyAttack_Input)
        {
            heavyAttack_Input = false;
            //do nothing if UI window is open 

            //do attack 
        }
    }

    private void QuedInput(ref bool quedInput) //passing a ref means we pass a specific bool, and not the value of the bool(True or false)
    {
        //reset all qued inputs, only one at a time allowed
        qued_LA_input = false;
        qued_HA_input = false;
        //qued_l1_input = false;
        //qued_l2_input = false;

        //check for UI window being open, return if so 

        if (player.isPerformingAction || player.playerMovementManager.isJumping)
        {
            quedInput = true;
            que_Input_Timer = default_Que_Input_Timer;
            input_Que_Active = true;
        }
    }
    private void ProcessQuedInputs()
    {
        if (player.isDead) return;

        if (qued_LA_input) lightAttck_Input = true;
        if (qued_HA_input) heavyAttack_Input = true;
    }
    private void HandleQuedInputs()
    {
        if (input_Que_Active)
        {
            //while timer is > 0, keep attempting the input press
            if (que_Input_Timer > 0)
            {
                que_Input_Timer -= Time.deltaTime;
                ProcessQuedInputs();
            }
            else
            {
                qued_LA_input = false;
                qued_HA_input = false;
                //qued_l1_input = false;
                //qued_l2_input = false;
                input_Que_Active = false;
                que_Input_Timer = 0;
            }
        }
    }


}
