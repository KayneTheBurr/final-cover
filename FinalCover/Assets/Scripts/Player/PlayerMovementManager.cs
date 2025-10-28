using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementManager : CharacterMovementManager
{
    PlayerManager player;

    [HideInInspector] public float verticalMovement, horizontalMovement, moveAmount;

    [Header("Movement Settings")]
    private Vector3 moveDirection;
    private Vector3 targetRotateDirection;
    [SerializeField] float rotationSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] public float walkSpeed;
    public float sprintStaminaCost;

    [Header("Jump")]
    [SerializeField] float jumpStaminaCost = 15;
    [SerializeField] float jumpHeight = 2;
    private Vector3 jumpDirection;
    [SerializeField] float jumpForwardSpeed = 5;
    [SerializeField] float airborneManeuverSpeed = 3;

    [Header("Dodge")]
    private Vector3 dodgeDirection;
    [SerializeField] float dodgeStaminaCost = 0;
    [SerializeField] float backStepStaminaCost = 0;



    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }
    protected override void OnEnable()
    {

    }
    protected override void OnDisable()
    {

    }
    protected override void Update()
    {
        base.Update();

        
    }
    public void AllMovement()
    {
        HandleGroundedMovement();
        HandleRotation();
        HandleJumpingMovement();
        HandleAirborneMovement();
    }
    private void HandleGroundedMovement()
    {
        if (player.canMove || player.canRotate)
        {
            GetVertandHoriInputs();
        }
        if (!player.canMove) return; //if i cant move, dont let me move

        //For move direction based on camera perspective and inputs
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection += PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        ////For movement independant of camera, determine which direction to move here
        //moveDirection = player.transform.forward * verticalMovement;
        //moveDirection += player.transform.right * horizontalMovement;
        //moveDirection.Normalize();
        //moveDirection.y = 0;

        //now determine SPEED of movement
        if(isSprinting)
        {
            player.characterController.Move(moveDirection * sprintSpeed * Time.deltaTime);
        }
        else
        {
            if (InputManager.instance.moveAmount > 0.5f)
            {
                //running speed
                player.characterController.Move(moveDirection * runSpeed * Time.deltaTime);
            }
            else if (InputManager.instance.moveAmount <= 0.5f)
            {
                //walking
                player.characterController.Move(moveDirection * walkSpeed * Time.deltaTime);
            }
        }


    }
    private void GetVertandHoriInputs()
    {
        verticalMovement = InputManager.instance.vertical_Input;
        horizontalMovement = InputManager.instance.horizontal_Input;
        moveAmount = InputManager.instance.moveAmount;

        //clamp for animations
    }
    private void HandleRotation()
    {
        if (player.isDead || player.canRotate) return;
        
    }
    private void HandleJumpingMovement()
    {
        if (isJumping)
        {
            player.characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
        }
    }
    private void HandleAirborneMovement()
    {
        if(!player.isGrounded)
        {

        }
    }


}
