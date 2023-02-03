using UnityEngine;
using UnityEngine.VFX;

public class FpAnimEventHandler : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] public PlayerControls _fpsCont;
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
        _fpsCont = selfWepData.moveController;

        _fpsCont.PlayGunFire(SoundList.instance.pistolFire);
        selfWepData.moveController.ShootPreparedRay();

    }
    public void SmgShootEvent()
    {
        _fpsCont = selfWepData.moveController;

        _fpsCont.PlayGunFire(SoundList.instance.smgFire);
        selfWepData.moveController.ShootPreparedRay();

    }
    public void SniperShootEvent()
    {
        _fpsCont = selfWepData.moveController;

        _fpsCont.PlayGunFire(SoundList.instance.sniperFire);   selfWepData.moveController.ShootPreparedRay();

    }


    public void RevReload()
    {
        //_fpsCont.PlayGunFire(SoundList.instance.GetSound("Pistol"));
        

    }
    public void SmgReload()
    {
       // _fpsCont.PlayGunFire(SoundList.instance.GetSound("Pistol"));
       

    }
    public void SniperReload()
    {
       // _fpsCont.PlayGunFire(SoundList.instance.GetSound("Pistol"));

    }

}
