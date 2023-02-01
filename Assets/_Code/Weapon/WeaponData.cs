using System;
using System.Collections;
using Mirror.Examples.AdditiveLevels;
using UnityEngine;
using Random = UnityEngine.Random;

public enum WeaponType
{
    PROJECTILE,
    HITSCAN
}











public class WeaponData : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private string _weaponName;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] public Animator anim;
    public string WeaponName => _weaponName;
    [SerializeField] private float _maxRange;
    public float MaxRange => _maxRange;
    enum WeaponFireType
    {
        Pistol,
        SMG,
        Sniper
    }

    void Start()
    {
        _currentAmmo = _maxAmmo;
    }

    [Header("Fire")] [Tooltip("is the weapon an automatic firearm (be able to hold LMB to fire)")] [SerializeField]
    private WeaponFireType fireType;

    public int ShootingType
    {
        get
        {
            switch (fireType)
            {
                case WeaponFireType.Pistol:
                    return 1;
                case WeaponFireType.SMG:
                    return 2;
                case WeaponFireType.Sniper:
                    return 3;
                default:
                    return 3;
            }
        }
    }

    //void Update()
    //{
    //    timeSinceLastShot += Time.deltaTime;
    //}
    //private float timeSinceLastShot;
    
    public void TryShoot()
    {
        //Debug.Log("TryShoot()");
        //if (!isLoaded)
        //{
        //    return;
        //}

        if (fireType == WeaponFireType.Sniper)
        {
            anim.SetTrigger("shot"); CmdShoot(BaseDamage);
            return;
        }

        if (CheckAndDoShootingLogistics())
        {
            anim.SetTrigger("shot");
            CmdShoot(BaseDamage);
        }
        else
        {
            _currentAmmo = _maxAmmo;
            anim.SetTrigger("reload");
        }
        
      
       
    }



    public void CmdShoot(int damage)
    {

    }


    public bool CheckAndDoShootingLogistics()
    {
        if (CurrentAmmo > 0)
        {
            _currentAmmo--;
            return true;
        }

        return false;


    }


    void Update()
    {
        try
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
        }
       
    }

    private void Shoot(Transform source, Vector3 direction)
    {
        Debug.LogWarning("SHOTS FIRED");

        Vector3 d = source.forward;
        d.x += Random.Range(-FireSpread, FireSpread);
        d.y += Random.Range(-FireSpread, FireSpread);




        Vector3 target = source.position + direction * 100f;
        RaycastHit hit;
        GameObject b = Instantiate(bulletPrefab, source.position, Quaternion.identity);
        b.GetComponent<BulletScript>().direction = target;

        ;
        if (Physics.Raycast(Camera.main.transform.position, direction, out hit, 100f))
        {
            try
            {
                if (hit.transform.GetComponent<PlayerObjectController>() != false)
                {
                    
                }
                PlayerObjectController enemy = hit.transform.gameObject.GetComponent<PlayerObjectController>();
                enemy.health -= (uint)BaseDamage;
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                throw;
            }

            Debug.Log("Hit " + hit.transform.name);
        }
    }

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
        yield return new WaitForSecondsRealtime(FireDelay);
        _currentAmmo = _maxAmmo;
        reloading = false;
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
    [SerializeField] private int _baseDamage;
    [SerializeField] private float _dmgHeadMultiplier = 2f;
    [SerializeField] private float _dmgBodyMultiplier = 1f;
    
    public int BaseDamage => _baseDamage;
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
