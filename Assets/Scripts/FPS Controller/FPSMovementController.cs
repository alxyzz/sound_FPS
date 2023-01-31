using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

public class FPSMovementController : NetworkBehaviour
{

    [Header("Weaponry")] 
    [HideInInspector]public WeaponData currWep;

    public WeaponData obj_Pistol, obj_SMG, obj_Sniper;
    //public Transform _fpsRoot;

    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;

    [Range(0.1f, 1f)] [Tooltip("1 = Same as ground. 0.1 = Least amount of control")]
    public float airMultiplier;
    bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;      //Be sure to update this if you change models or something
    public LayerMask whatIsGround;
    public bool grounded;
    
    [Header("References")]
    public Transform orientaion;
    public Transform headCube;
    public Transform capsule;
    public Transform camPosition;
    public FPS_UI_Component HUDComponent;

    public GameObject cameraHolderPrefab;

    [SerializeField] Rigidbody rb;

    [Header("Multiplayer Config")]
    public GameObject playerModel;
    public MeshRenderer playerMesh;
    public Material[] playerColors;

    Vector3 moveDirection;
    float horizontalInput;
    float verticalInput;

    //Misc
    bool cameraSpawned = false;

    private void Start() {
        playerModel.SetActive(false);
    }

    public override void OnStartAuthority()
    {
        rb.freezeRotation =  true;
        rb.useGravity = false;
        ResetJump();
    }


    private void MoveFPSModelTocameraObject()
    {
        //_fpsRoot.SetParent(playerCamera);
        //_fpsRoot.transform.position = playerCamera.position;
    }

    private Transform playerCamera;
    
    private void Update() {
        if (SceneManager.GetActiveScene().name == "Game") {
            if (playerModel.activeSelf == false) {
                SetRandomPosition();
                PlayerCosmeticSetup();
                playerModel.SetActive(true);
               

                if (hasAuthority && cameraSpawned == false) {
                    rb.useGravity = true;
                    GameObject cameraHolderInstance = Instantiate(cameraHolderPrefab, transform.position, transform.rotation);
                    cameraHolderInstance.GetComponent<CameraHolder>().cameraPosition = camPosition;
                    cameraHolderInstance.GetComponent<CameraHolder>().cameraController.orientation = orientaion;
                    cameraHolderInstance.GetComponent<CameraHolder>().cameraController.headCube = headCube;
                    cameraHolderInstance.GetComponent<CameraHolder>().cameraController.capsule = capsule;

                    headCube.gameObject.SetActive(false);

                    playerCamera = cameraHolderInstance.GetComponent<CameraHolder>().fpsCamera.transform;
                    cameraSpawned = true;

                    MoveFPSModelTocameraObject();
                   
                    HUDComponent.transform.position = cameraHolderInstance.GetComponent<CameraHolder>().cameraPosition.position;

                }
            }

            if (hasAuthority) {
                //Ground check
                grounded = Physics.Raycast(orientaion.position, Vector3.down, playerHeight * 0.5f + 0.34f, whatIsGround);

               // _fpsRoot.rotation = playerCamera.rotation;
                
                
                GetKeyboardInput();
                SpeedControl();
               
                //Applying drag
                if (grounded) {
                    rb.drag = groundDrag;
                } else {
                    rb.drag = 0;
                }
            }
        }
    }



    private void FixedUpdate() {
        if (!hasAuthority) return;
        if (SceneManager.GetActiveScene().name == "Game") {
            MovePlayer();
        }
    }

    private void GetKeyboardInput() {
        //Fetching keyboard input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && grounded) {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeWeapon(obj_Pistol);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeWeapon(obj_SMG);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeWeapon(obj_Sniper);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log(playerCamera);
            if (currWep != null)
            {
                currWep.TryShoot(transform, playerCamera.transform.forward);
                
            }
           
        }

    }

    private void ChangeWeapon(WeaponData w)
    {
        Debug.LogWarning("Changed weapon to " + w);
        if (currWep != null)
        {
            currWep.gameObject.SetActive(false);
        }
        
        currWep = w;
        currWep.gameObject.SetActive(true);
    }
    


    //[Command]
    private void MovePlayer() {
        //Calculating movement direction
        moveDirection = orientaion.forward * verticalInput + orientaion.right * horizontalInput;
        
        if(horizontalInput == 0 && verticalInput == 0) {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            return;
        }

        if (grounded) {
            rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Impulse);
        } else if (!grounded) {
            rb.AddForce(moveDirection.normalized * moveSpeed * airMultiplier, ForceMode.Impulse);
        }
    }

    private void SpeedControl() {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //Limiting speed
        if(flatVelocity.magnitude > moveSpeed) {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    //[Command]
    private void Jump() {
        //Resetting y(upwards) velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump() {
        //Debug.Log("Ready to jump");
        readyToJump = true;
    }

    public void SetRandomPosition() {
        transform.position = new Vector3(Random.Range(-20f, 20f), 4f, Random.Range(-20f, 20f));
    }

    public void PlayerCosmeticSetup() {
        playerMesh.material = playerColors[GetComponent<PlayerObjectController>().PlayerColor];
    }
}
