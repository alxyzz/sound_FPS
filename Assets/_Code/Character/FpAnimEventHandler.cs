using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.VFX;

public class FpAnimEventHandler : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] public FPSMovementController _fpsCont;
    private VisualEffect v;

    private WeaponData selfWepData;


    public bool IsLocalPlayer { get; set; }
    protected  void Start()
    {
        _animator = GetComponent<Animator>();
        selfWepData = GetComponent<WeaponData>();
        v = GetComponent<VisualEffect>();
        _fpsCont = selfWepData.moveController;
    }






    public void RevShootEvent()
    {
        if (_fpsCont == null)
        {
            Debug.Log("fpscont was null");
        }
        if (SoundList.instance == null)
        {
            Debug.Log("SoundList.instance was null");
        }

        _fpsCont.PlayGunFire(SoundList.instance.pistolFire);
        selfWepData.ShootPreparedRay();

    }
    public void SmgShootEvent()
    {
        _fpsCont.PlayGunFire(SoundList.instance.smgFire);
        selfWepData.ShootPreparedRay();

    }
    public void SniperShootEvent()
    {
        _fpsCont.PlayGunFire(SoundList.instance.sniperFire);   selfWepData.ShootPreparedRay();

    }


    public void RevReload()
    {
        _fpsCont.PlayGunFire(SoundList.instance.GetSound("Pistol"));
        selfWepData.ShootPreparedRay();

    }
    public void SmgReload()
    {
        _fpsCont.PlayGunFire(SoundList.instance.GetSound("Pistol"));
        selfWepData.ShootPreparedRay();

    }
    public void SniperReload()
    {
        _fpsCont.PlayGunFire(SoundList.instance.GetSound("Pistol"));
        selfWepData.ShootPreparedRay();

    }

}
