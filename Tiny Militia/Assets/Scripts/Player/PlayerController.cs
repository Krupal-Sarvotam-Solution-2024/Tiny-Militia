using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    #region Variables

    #region Variables For Movements
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    #endregion

    #region Variables For Shooting
    public Transform gunTransform;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    #endregion

    #region Variables of Ammo and Bomb Prefeb
    public GameObject bulletPrefab;
    public GameObject bombPrefab;
    public bool abletoShoot;
    #endregion

    #region Variables for Booster
    public float jetpackForce = 5f;
    public float jetpackFuel = 100f;
    public float jetpackFuelConsumptionRate = 10f;
    public float jetpackFuelRechargeRate = 5f;
    private float currentJetpackFuel;
    #endregion

    #region boolen for checking gun state is reloading or not
    bool isReloading;
    #endregion

    #region Variables For Player Health
    public int maxHealth = 100;
    public int currentHealth;
    public float healthRecoveryRate = 2f;
    #endregion

    #region Variables For Guns Scriptable Obect
    public List<Gun> guns; // List of ScriptableObject Guns
    public int currentGunIndex = 0;
    private int alternateGunIndex = -1;
    #endregion

    #region Getting Player Rigidbody for Physics
    private Rigidbody2D rb;
    #endregion

    #region Boolen for Chceking that Player is In Ground Or Not
    private bool isGrounded;
    #endregion

    #region Variables For Jpoystick
    public FixedJoystick movementJoystick;
    public FixedJoystick aimJoystick;
    #endregion 


    #endregion

    #region Methods
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentJetpackFuel = jetpackFuel;
        currentHealth = maxHealth;
        Gun Gun = guns[currentGunIndex];

        UIManager.instance.AmmoInfo_text.text = Gun.currentAmmoInMagazine.ToString() + " / " + Gun.currentTotalAmmo.ToString();
        UpdateHealthImage();

        // Initialize all guns
        foreach (Gun gun in guns)
        {
            gun.Initialize();
        }


        StartCoroutine(AutoHealthRecovery());
    }

    void Update()
    {
        Move();
        Jump();
        Aim();
        Shoot();
        Jetpack();
        UpdateBoosterLevel();
        HandleGunSwitching();
        HandleBombThrowing();
    }

    void Move()
    {
        float moveInput = movementJoystick.Horizontal;
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

    }

    void Jump()
    {
        float moveInput = movementJoystick.Vertical;
        //if (Input.GetButtonDown("Jump") && isGrounded)
        if (moveInput > 0 && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void Aim()
    {
        Vector3 rotation = new Vector3(aimJoystick.Horizontal, aimJoystick.Vertical, 0);
        float moveInput = movementJoystick.Vertical;
        //  Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 aimDirection = rotation;

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        gunTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (aimDirection.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            gunTransform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (aimDirection.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            gunTransform.localScale = new Vector3(-1f, -1f, 1f);
        }
        else if (aimDirection.x == 0)
        {
            gunTransform.localScale = Vector3.one;
        }
    }

    void Shoot()
    {
        if (isReloading == false)
        {
            if (aimJoystick.Horizontal > .75 || aimJoystick.Vertical > .75 || aimJoystick.Horizontal < -.75f || aimJoystick.Vertical < -.75f)
                abletoShoot = true;
            else
            {
                abletoShoot = false;
            }
            if (abletoShoot && Time.time > guns[currentGunIndex].lastShotTime + guns[currentGunIndex].shootCooldown)
            {
                Gun currentGun = guns[currentGunIndex];

                if (currentGun.currentAmmoInMagazine > 0)
                {
                    if (currentGun.gunType == "Bomb")
                    {
                        ThrowBomb();
                    }
                    else
                    {
                        FireBullet();
                    }

                    currentGun.lastShotTime = Time.time;
                    currentGun.currentAmmoInMagazine--;
                    if (currentGun.currentTotalAmmo > 0)
                    {
                        UIManager.instance.ReloadButton.interactable = true;
                    }
                    else
                    {
                        UIManager.instance.ReloadButton.interactable = false;
                    }
                    UIManager.instance.AmmoInfo_text.text = currentGun.currentAmmoInMagazine.ToString() + " / " + currentGun.currentTotalAmmo.ToString();
                }
                else if (isReloading == false)
                {
                    if (currentGun.currentTotalAmmo > 0)
                    {
                        UIManager.instance.ReloadButton.interactable = true;
                        StartCoroutine(Reload(currentGun));
                    }
                    else
                    {
                        UIManager.instance.ReloadButton.interactable = false;
                    }
                }
            }
        }
    }

    void FireBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.tag = "Player_Bullet";
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.gun = guns[currentGunIndex];  // Pass the current gun reference
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = firePoint.right * bulletSpeed;
    }

    void ThrowBomb()
    {
        GameObject bomb = Instantiate(bombPrefab, firePoint.position, firePoint.rotation);
        bomb.tag = "Player_Bomb";
        Rigidbody2D bombRb = bomb.GetComponent<Rigidbody2D>();
        bombRb.velocity = firePoint.right * bulletSpeed;
    }

    public IEnumerator Reload(Gun gun)
    {
        isReloading = true;

        // Starting fill amount (fully filled)
        float currentFill = 1f;

        // Initialize fillAmount to full
        UIManager.instance.ReloadImage.fillAmount = currentFill;
        UIManager.instance.ReloadImage.gameObject.SetActive(true);


        float timer = 0f;

        while (timer < gun.reloadTime)
        {
            timer += Time.deltaTime;
            float ratio = timer / gun.reloadTime;

            // Decrease fillAmount over time
            UIManager.instance.ReloadImage.fillAmount = Mathf.Lerp(currentFill, 0f, ratio);
            yield return null;
        }

        // Optional initial delay if needed
        yield return new WaitForSeconds(0);

        // Ensure final fillAmount is exactly 0
        UIManager.instance.ReloadImage.fillAmount = 0f;

        UIManager.instance.ReloadButton.interactable = true;
        int ammoNeeded = gun.magazineSize - gun.currentAmmoInMagazine;
        int ammoToReload = Mathf.Min(ammoNeeded, gun.currentTotalAmmo);
        gun.currentAmmoInMagazine += ammoToReload;
        gun.currentTotalAmmo -= ammoToReload;
        UIManager.instance.AmmoInfo_text.text = gun.currentAmmoInMagazine.ToString() + " / " + gun.currentTotalAmmo.ToString();
        UIManager.instance.ReloadImage.gameObject.SetActive(false);
        UIManager.instance.ReloadImage.fillAmount = 1;
        isReloading = false;

    }

    void Jetpack()
    {
        float moveInput = movementJoystick.Vertical;
        if (moveInput > 0 && !isGrounded && currentJetpackFuel > 0)
        {

            rb.velocity = new Vector2(rb.velocity.x, jetpackForce * moveInput);
            currentJetpackFuel -= jetpackFuelConsumptionRate * Time.deltaTime;
        }
        else if (isGrounded)
        {
            currentJetpackFuel = Mathf.Min(jetpackFuel, currentJetpackFuel + jetpackFuelRechargeRate * Time.deltaTime);
        }
    }

    void UpdateBoosterLevel()
    {
        float fillAmount = currentJetpackFuel / jetpackFuel;
        UIManager.instance.boosterLevelImage.fillAmount = fillAmount;
    }

    void UpdateHealthImage()
    {
        float fillAmount = (float)currentHealth / maxHealth;
        UIManager.instance.healthImage.fillAmount = fillAmount;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        UpdateHealthImage();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        SceneManager.LoadScene("Menu");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Bot_Bullet"))
        {
            Gun botGun = collision.gameObject.GetComponent<Bullet>().gun;
            TakeDamage(botGun.damagePerBullet);
        }

        if(collision.gameObject.CompareTag("Gun_Uzi"))
        {
            for (int i = 0; i < guns.Count; i++)
            {
                if(guns[i].ObjectTag == "Gun_Uzi")
                {
                    if (guns[i].currentTotalAmmo < guns[i].maxAmmo)
                    {
                        guns[i].currentTotalAmmo = guns[i].maxAmmo;
                        Gun currentGun = guns[currentGunIndex];
                        UIManager.instance.AmmoInfo_text.text = currentGun.currentAmmoInMagazine.ToString() + " / " + currentGun.currentTotalAmmo.ToString();
                        Destroy(collision.gameObject);
                    }
                }
            }
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    IEnumerator AutoHealthRecovery()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (currentHealth < maxHealth)
            {
                currentHealth += 1;
                UpdateHealthImage();
            }
        }
    }

    void HandleGunSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (alternateGunIndex == -1)
            {
                alternateGunIndex = currentGunIndex;
                currentGunIndex = (currentGunIndex + 1) % guns.Count;
                UIManager.instance.GunIndex = currentGunIndex;
                Gun currentGun = guns[currentGunIndex];              
                UIManager.instance.AmmoInfo_text.text = currentGun.currentAmmoInMagazine.ToString() + " / " + currentGun.currentTotalAmmo.ToString();
            }
            else
            {
                int temp = currentGunIndex;
                currentGunIndex = alternateGunIndex;
                alternateGunIndex = temp;
                UIManager.instance.GunIndex = currentGunIndex;
                Gun currentGun = guns[currentGunIndex];

                UIManager.instance.AmmoInfo_text.text = currentGun.currentAmmoInMagazine.ToString() + " / " + currentGun.currentTotalAmmo.ToString();
            }
        }
    }

    void HandleBombThrowing()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ThrowBomb();
        }
    }

    #endregion

}