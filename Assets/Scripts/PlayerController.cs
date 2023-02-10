using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameManager gameManager;
    private Animator playerAnimator;
    private bool inMotion = false; private bool tmp_inMotion = false;
    private Camera camera;

    //keypad input
    private float horizontalInput;
    private float forwardInput;
    private bool chargeInput = false;
    private bool chargeDoesNotInput = false;

    //mouse input
    private float horizontalMouseInput;
    private float verticalMouseInput;
    public  float sensitivityMouse;
    public static float sensitivityMouseGlobal = 0;
    public bool mouseInverted;
    public static bool mouseInvertedGlobal;

    //variables for jumps
    private bool jumpInput = false;
    private     Rigidbody rb;
    private int numberOfJumps = 1;
    private int jumpsCounter;
    private bool isOnGround = false; private bool tmp_isOnGround = false;
    //forces for jumps
    public float jumpForce;
    public float forwardForce;
    public float dashForce;
    public float chargeForce;
    public float gravityModifier;

    //variables for movement
    private bool shiftBeingPressed = false; private bool tmp_shiftBeingPressed = false;
    public float charge;
    public float speed;
    public float dashSpeed;
    public float heavyGunSpeedReduce;

    //variables for inventory
    private bool is1stItem = true;
    private bool is2ndItem = false;
    private bool is3rdItem = false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerAnimator = GameObject.Find("Player Animator").GetComponent<Animator>();
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();

        rb = GetComponent<Rigidbody>();
        jumpsCounter = numberOfJumps;
        Physics.gravity *= gravityModifier;

        sensitivityMouseGlobal = sensitivityMouse > 0 ? sensitivityMouse : 100;
        mouseInvertedGlobal = mouseInverted;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.PauseGame();
        }
        if (gameManager.isGameActive)
        {

            horizontalInput = 0f;
            forwardInput = 0f;
            //keypad
            horizontalInput = Input.GetAxis("Horizontal");
            forwardInput = Input.GetAxis("Vertical");
            if (horizontalInput != 0f || forwardInput != 0f)
            {
                tmp_inMotion = true;
            } else
            {
                tmp_inMotion = false;
            }
            if (tmp_inMotion != inMotion)
            {
                inMotion = tmp_inMotion;
                if (inMotion)
                {
                    playerAnimator.SetTrigger("inMotion");
                } else
                {
                    playerAnimator.ResetTrigger("inMotion");
                }
            }

            //mouse
            horizontalMouseInput = Input.GetAxis("Mouse X");
            verticalMouseInput = Input.GetAxis("Mouse Y");

            //shift
            chargeInput = Input.GetKeyDown(KeyCode.LeftShift);
            chargeDoesNotInput = Input.GetKeyUp(KeyCode.LeftShift);
            if (chargeDoesNotInput)
            {
                shiftBeingPressed = false;
            }
            if (chargeInput)
            {
                shiftBeingPressed = true;
            };
            if (tmp_shiftBeingPressed != shiftBeingPressed)
            {
                tmp_shiftBeingPressed = shiftBeingPressed;
                if (tmp_shiftBeingPressed)
                {
                    playerAnimator.SetTrigger("isCharge");
                }
                else
                {
                    playerAnimator.ResetTrigger("isCharge");
                }
            }
            float chargeTemp = (shiftBeingPressed && isOnGround && !gameManager.IsMachineGun()) ? charge : 1f;
            float heavyGunSpeedReduceTemp = gameManager.IsMachineGun() ? heavyGunSpeedReduce : 1f;
            float chargeForceTemp = (shiftBeingPressed && isOnGround && !gameManager.IsMachineGun()) ? chargeForce : 1f;

            //space
            jumpInput = Input.GetKeyDown(KeyCode.Space);

            transform.Rotate(Vector3.up * Time.deltaTime * horizontalMouseInput * sensitivityMouseGlobal);

            if (jumpInput && isOnGround && !gameManager.IsMachineGun())
            {
                rb.AddRelativeForce(Vector3.up * jumpForce, ForceMode.Impulse);
                rb.AddRelativeForce(Vector3.forward * forwardInput * forwardForce * chargeForceTemp, ForceMode.Impulse);
                rb.AddRelativeForce(Vector3.right * horizontalInput * dashForce, ForceMode.Impulse);
                jumpsCounter--;
                isOnGround = false;
            }

            if (isOnGround)
            {
                transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput * heavyGunSpeedReduceTemp * chargeTemp);
                transform.Translate(Vector3.right * Time.deltaTime * dashSpeed * horizontalInput * heavyGunSpeedReduceTemp);
            }
            if (tmp_isOnGround != isOnGround)
            {
                tmp_isOnGround = isOnGround;
                if (isOnGround)
                {
                    playerAnimator.SetTrigger("isOnGround");
                }
                else
                {
                    playerAnimator.ResetTrigger("isOnGround");
                }
            }

            //Inventory
            is1stItem = Input.GetKeyDown(KeyCode.Alpha1);
            is2ndItem = Input.GetKeyDown(KeyCode.Alpha2);
            is3rdItem = Input.GetKeyDown(KeyCode.Alpha3);
            if (is1stItem)
            {
                is2ndItem = false;
                is3rdItem = false;
                gameManager.SetActiveGun(0);
            } else if (is2ndItem)
            {
                is1stItem = false;
                is3rdItem = false;
                gameManager.SetActiveGun(1);
            } else if (is3rdItem) 
            { 
                is1stItem = false;
                is2ndItem = false;
                gameManager.SetActiveGun(2);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        jumpsCounter = numberOfJumps;
        isOnGround = true;
    }
}
