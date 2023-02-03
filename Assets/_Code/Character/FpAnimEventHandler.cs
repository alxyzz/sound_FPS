using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.VFX;

public class FpAnimEventHandler : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private FPSMovementController _fpsCont;
    private VisualEffect v;

    private WeaponData selfWepData;


    public bool IsLocalPlayer { get; set; }
    protected  void Awake()
    {
        _animator = GetComponent<Animator>();
        selfWepData = GetComponent<WeaponData>();
        v = GetComponent<VisualEffect>();

    }






    public void RevShootEvent()
    {
        SoundList.GetSound("Pistol");
        //_fpsCont.PlayGunFire(SoundList.GetSound("Pistol"));
        //selfWepData.ShootPreparedRay();

    }
    public void SmgShootEvent()
    {
        _fpsCont.PlayGunFire(SoundList.GetSound("Pistol"));
        selfWepData.ShootPreparedRay();

    }
    public void SniperShootEvent()
    {
        _fpsCont.PlayGunFire(SoundList.GetSound("Pistol"));
        selfWepData.ShootPreparedRay();

    }


    public void RevReload()
    {
        _fpsCont.PlayGunFire(SoundList.GetSound("Pistol"));
        selfWepData.ShootPreparedRay();

    }
    public void SmgReload()
    {
        _fpsCont.PlayGunFire(SoundList.GetSound("Pistol"));
        selfWepData.ShootPreparedRay();

    }
    public void SniperReload()
    {
        _fpsCont.PlayGunFire(SoundList.GetSound("Pistol"));
        selfWepData.ShootPreparedRay();

    }

}
