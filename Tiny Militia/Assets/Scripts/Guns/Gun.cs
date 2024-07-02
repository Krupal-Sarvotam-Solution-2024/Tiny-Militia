using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGun", menuName = "Guns/Gun")]
public class Gun : ScriptableObject
{
    public Sprite GunSprite;
    public string gunName;
    public string gunType;
    public int damagePerBullet;
    public string damageType;
    public float reloadTime;
    public int magazineSize;
    public int maxAmmo;
    public float shootCooldown;
    public string ObjectTag;

    [HideInInspector]
    public int currentAmmoInMagazine;
    [HideInInspector]
    public int currentTotalAmmo;
    [HideInInspector]
    public float lastShotTime;


    

    public void Initialize()
    {
        
        currentAmmoInMagazine = magazineSize;
        currentTotalAmmo = maxAmmo;
        lastShotTime = -shootCooldown;
    }
}
