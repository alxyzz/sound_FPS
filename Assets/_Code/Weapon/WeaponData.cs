using System;
using System.Collections;
using Cinemachine;
using Mirror;
using Mirror.Examples.AdditiveLevels;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public enum WeaponType
{
    PROJECTILE,
    HITSCAN
}











public class WeaponData : NetworkBehaviour
{
    [Header("General")]
    [SerializeField] private string _weaponName;
    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] public Animator anim;
    [SerializeField] public VisualEffect _visualEffect;

    [SerializeField] public ActionManager moveController;
    public string WeaponName => _weaponName;
    [SerializeField] private float _maxRange;
    public float MaxRange => _maxRange;
    private Camera userCam;
    private Transform userloc;
    public enum WeaponFireType
    {
        Pistol,
        SMG,
        Sniper
    }


    public void DoFireEvent()
    {
        _visualEffect.Play();
    }

    void Start()
    {
        _currentAmmo = _maxAmmo;

    }

    [Header("Fire")] [Tooltip("is the weapon an automatic firearm (be able to hold LMB to fire)")] [SerializeField]
    private WeaponFireType fireType;

    public WeaponFireType ShootingType
    {
        get
        {
            return fireType;
        }
    }

    //void Update()
    //{
    //    timeSinceLastShot += Time.deltaTime;
    //}
    //private float timeSinceLastShot;


    private Transform camTransform, userTransform;

    public void TryShoot(Transform c, Transform user)
    {

        Debug.Log("tried to shoot. ammo is now ()");
        fireCooldown = true;
        //if (fireType == WeaponFireType.Sniper)
        //{
        //    anim.SetTrigger("shot"); 
        //    //ShootRay(BaseDamage, c, user);
        //    return;
        //}

        if (CheckAndDoShootingLogistics())
        {
            anim.SetTrigger("shot");
            //ShootRay(BaseDamage, c, user); //we handle this during animation so it looks better
        }
        else
        {
            Reload();
        }
    }
    /// <summary>
    /// shoots the ray defined previously at a slight delay to synchronize it with the gun firing
    /// </summary>
    



   public bool CheckAndDoShootingLogistics()
    {
        if (CurrentAmmo > 0)
        {
            _currentAmmo--;
            return true;
        }

        return false;


    }

   private float timeBetweenShots;
   private bool fireCooldown;

    void Update()
    {
        //if (fireCooldown)
        //{
        //    timeBetweenShots += Time.deltaTime;
        //    if (timeBetweenShots >= FireDelay)
        //    {
        //        timeBetweenShots = 0;
        //        fireCooldown = false;
        //        Debug.LogWarning("can fire again with your gun - GUN COOLDOWN");
        //    }
        //}
        

        try
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
        }
       
    }

    //private void Shoot(Transform source, Vector3 direction)
    //{
    //    Debug.LogWarning("SHOTS FIRED");

    //    Vector3 d = source.forward;
    //    d.x += Random.Range(-FireSpread, FireSpread);
    //    d.y += Random.Range(-FireSpread, FireSpread);




    //    Vector3 target = source.position + direction * 100f;
    //    RaycastHit hit;
    //    GameObject b = Instantiate(bulletPrefab, source.position, Quaternion.identity);
    //    b.GetComponent<BulletScript>().direction = target;

    //    ;
    //    if (Physics.Raycast(Camera.main.transform.position, direction, out hit, 100f))
    //    {
    //        try
    //        {
    //            if (hit.transform.GetComponent<PlayerController>() != false)
    //            {
                    
    //            }
    //            PlayerController enemy = hit.transform.gameObject.GetComponent<PlayerController>();
    //            //enemy.health -= (uint)BaseDamage;
    //        }
    //        catch (Exception e)
    //        {
    //            //Console.WriteLine(e);
    //            throw;
    //        }

    //        Debug.Log("Hit " + hit.transform.name);
    //    }
    //}

    private int _currentAmmo;

    public int CurrentAmmo
    {
        get
        {
            return _currentAmmo;
        }
    }

    [SerializeField] private int _maxAmmo;
    public bool isLoaded
    {
        get
        {
            if (_currentAmmo == _maxAmmo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public void Reload()
    {
        Debug.Log("Current ammo count is =>" + CurrentAmmo + Environment.NewLine + " Max ammo is =>" + _maxAmmo);



        GetComponent<Animator>().SetTrigger("reload");
        StartCoroutine(ReloadDelay());
            
    }

    private bool reloading;
    IEnumerator ReloadDelay()
    {
        reloading = true;
        yield return new WaitForSecondsRealtime(0);
        _currentAmmo = _maxAmmo;
        reloading = false; 
        moveController.HUDComponent.ChangeAmmoCounter(CurrentAmmo);
    }

    [Tooltip("delay for continuous shooting")]
    [SerializeField] private float _fireDelay;
    public float FireDelay => _fireDelay;
    
    //[Tooltip("degrees the raycast will deviate randomly")]
    //[SerializeField] private float _spread;
    //public float Spread => _spread;
  
    [Header("Spread")]
    [Tooltip("basic spread on the crosshair. unit is pixel")]
    [SerializeField] private float _crosshairSpread;

    private float _temporaryCrosshairSpread;
    public float CrosshairSpread => _crosshairSpread;
    [Tooltip("spread when firing. unit is meter")]
    [SerializeField] private float _fireSpread;
    public float FireSpread => _fireSpread;
  


    [Header("Damage")]
    [SerializeField] private uint _baseDamage;
    [SerializeField] private float _dmgHeadMultiplier = 2f;
    [SerializeField] private float _dmgBodyMultiplier = 1f;
    
    public uint BaseDamage => _baseDamage;
    public int DamageHead => (int)(_baseDamage * _dmgHeadMultiplier);
    public int DamageBody => (int)(_baseDamage * _dmgBodyMultiplier);
   

    [Header("Miscellaneous")]
    [SerializeField] private float _movementMultiplier;
    public float MovementMultiplier => _movementMultiplier;
    [SerializeField] private AudioClip _fireSound;
    public AudioClip FireSound => _fireSound;
    [SerializeField] private Sprite _killIcon;
    public Sprite KillIcon => _killIcon;
}
