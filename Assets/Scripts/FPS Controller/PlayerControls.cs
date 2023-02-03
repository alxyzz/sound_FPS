using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

public class PlayerControls : NetworkBehaviour
{

    [Header("Weaponry")] 
    [HideInInspector]public WeaponData currWep;
    
    [HideInInspector]public WeaponData obj_Pistol, obj_SMG, obj_Sniper;

    [HideInInspector] public Animator anim_Pistol, anim_SMG, anim_Sniper, currWepAnim;
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

    public bool isDead;
    public bool isStunned;
    public bool onbeat;

    public bool canMove
    {
        get
        {
            if (!isDead && !isStunned)
            {
                return true;
            }

            return false;
        }
    }


    [Header("References")]
    public Transform orientation;
    public Transform headCube;
    public Transform capsule;
    public Transform camPosition;
    public FPS_UI_Component HUDComponent;
    public PlayerController PlayController;
    public GameObject cameraHolderPrefab;

    [SerializeField] Rigidbody rb;

    [Header("Multiplayer Config")]
    public GameObject playerModel;
    public SkinnedMeshRenderer playerMesh;
    public Material[] playerColors;
    public Material characterVisorMat;
    public GameObject characterVisorObject;
    public AudioSource localSoundPlayer;

    Vector3 moveDirection;
    float horizontalInput;
    float verticalInput;

    //Misc
    bool cameraSpawned = false;

    private void Start() {
        playerModel.SetActive(false);
        PlayController = GetComponent<PlayerController>();
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


    [Command]
    public void ShootPreparedRay()
    {
        ShootRay(currWep.BaseDamage, transform, playerCamera.transform.forward);


    }



    [ClientRpc]
    public void ShootRay(uint damage, Transform source, Vector3 direction)
    {



       currWep.DoFireEvent();

        Vector3 look = playerCamera.transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(source.position, look, Color.green, 555, false);

        RaycastHit shootHit;
        Ray shootRay = new Ray(source.position, look);
        GameObject b = Instantiate(currWep.bulletPrefab, source.position, Quaternion.identity);
        b.GetComponent<BulletScript>().direction = look * 100;
        if (Physics.Raycast(shootRay, out shootHit))
        {
            if (shootHit.transform.tag == "Player")
            {
                if (shootHit.transform == transform)
                {
                    return;
                }
                Debug.Log("Just hit player. Will damage for " + currWep.BaseDamage + " now.");
                var ENEMY = shootHit.transform.GetComponent<PlayerController>();

                ENEMY.CmdTakeDamage(currWep.BaseDamage);
                //ENEMY.RefreshHealthUI();
            }
            Debug.Log("Just hit " + shootHit.transform.gameObject.name);
        }
    }

    private CameraHolder camholder;


  private Transform playerCamera;
    
    private void Update() {
        if (SceneManager.GetActiveScene().name == "Game") {
            if (playerModel.activeSelf == false) {
                SetRandomPosition();
                PlayerCosmeticSetup();
                playerModel.SetActive(true);
                localSoundPlayer = GetComponent<AudioSource>();
                    
                if (hasAuthority && cameraSpawned == false) {
                    rb.useGravity = true;
                    GameObject cameraHolderInstance = Instantiate(cameraHolderPrefab, transform.position, transform.rotation);
                    cameraHolderInstance.GetComponent<CameraHolder>().cameraPosition = camPosition;
                    cameraHolderInstance.GetComponent<CameraHolder>().cameraController.orientation = orientation;
                    cameraHolderInstance.GetComponent<CameraHolder>().cameraController.headCube = headCube;
                    cameraHolderInstance.GetComponent<CameraHolder>().cameraController.capsule = capsule;
                     camholder = cameraHolderInstance.GetComponent<CameraHolder>();
                    headCube.gameObject.SetActive(false);
                    //NetworkServer.Spawn(cameraHolderInstance, new NetworkConnectionToServer());
                    capsule.gameObject.layer = LayerMask.NameToLayer("InvisibleToSelf");

                    obj_SMG = camholder.obj_SMG;
                    obj_Pistol = camholder.obj_Pistol;
                    obj_Sniper = camholder.obj_Sniper;
                    currWep = obj_Pistol;
                    

                    anim_Pistol = obj_Pistol.GetComponent<Animator>();
                    anim_SMG = obj_Pistol.GetComponent<Animator>();
                    anim_Sniper = obj_Pistol.GetComponent<Animator>();
                    currWepAnim = anim_Pistol;
                    playerCamera = cameraHolderInstance.GetComponent<CameraHolder>().fpsCamera.transform;
                    cameraSpawned = true;

                   
                    HUDComponent.transform.position = cameraHolderInstance.GetComponent<CameraHolder>().cameraPosition.position;
                    HUDComponent.fpscontrol = this;
                    //HUDComponent.StartBeat();

                }
            }

            if (hasAuthority) {
                //Ground check
                grounded = Physics.Raycast(orientation.position, Vector3.down, playerHeight * 0.5f + 0.34f, whatIsGround);

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



    public void PlayGunFire(AudioClip b)
    {
        localSoundPlayer.PlayOneShot(b);
        HUDComponent.ChangeAmmoCounter(currWep.CurrentAmmo);




    }


    private void FixedUpdate() {
        if (!hasAuthority) return;
        if (SceneManager.GetActiveScene().name == "Game") {
            MovePlayer();
        }
    }

    public float period;

    private void GetKeyboardInput() {
        //Fetching keyboard input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        period += Time.deltaTime;
       

        if(Input.GetKey(jumpKey) && readyToJump && grounded) {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
          
            Debug.Log("Take Damage test.");
          PlayController.CmdTakeDamage(5);
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

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (currWep != null)
            {
                
                
                if (isLocalPlayer)
                {
                   
                    switch (currWep.ShootingType)
                    {
                        case WeaponData.WeaponFireType.Pistol: 
                            if (period > 1.2) //0.2s clearance
                        {
                            
                            currWep.moveController = this;
                            currWep.TryShoot(playerCamera, transform);
                            }
                            break;
                        case WeaponData.WeaponFireType.SMG:
                            if (period > 0 && period > 0.4) //0.2s clearance
                            {
                               
                                currWep.moveController = this;
                                currWep.TryShoot(playerCamera, transform);
                            }
                            break;
                        case WeaponData.WeaponFireType.Sniper:
                            if (period > 0 && period > 0.1) //0.2s clearance
                            {

                                currWep.moveController = this;
                                currWep.TryShoot(playerCamera, transform);
                            }
                            break;
                    }
                }

            }




        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currWep != null)
            {
                currWep.Reload();
               

            }

        }
        if (period > 1.2f) //0.2s clearance
        {
            localSoundPlayer.PlayOneShot(SoundList.instance.beatsound); //only plays on player due to no clientRPC
            period = 0;
        }
    }

    public GameObject testobj;
    private void ChangeWeapon(WeaponData w)
    {
        Debug.LogWarning("Changed weapon to " + w);
        if (currWep != null)
        {
            currWep.gameObject.SetActive(false);
        }

       
        currWep = w;
        currWepAnim = currWep.GetComponent<Animator>();
        currWep.gameObject.SetActive(true);
    }
    


    //[Command]
    private void MovePlayer() {
        //Calculating movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        if(horizontalInput == 0 && verticalInput == 0) {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);

            if (currWepAnim != null && currWep != null)
            {
                currWepAnim.SetBool("walking", false);
            }
            return;
        }
        
       

        if (grounded && canMove) {
            rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Impulse);
            currWepAnim.SetBool("walking", true);
        } else if (!grounded && canMove) {
            rb.AddForce(moveDirection.normalized * moveSpeed * airMultiplier, ForceMode.Impulse);
            currWepAnim.SetBool("walking", false);
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

    public void SetRandomPosition()
    {
        FPSNetworkManager.singleton.GetStartPosition();
        //transform.position = new Vector3(Random.Range(-20f, 20f), 4f, Random.Range(-20f, 20f));
        transform.position = FPSNetworkManager.singleton.GetStartPosition().position;
    }

    public void PlayerCosmeticSetup() {
        playerMesh.material = playerColors[GetComponent<PlayerController>().PlayerColor];
    }
}
