using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks
{
    // Variables For Movements
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    // Variables For Shooting
    public Transform gunTransform;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public Transform rightgunTransform;
    public Transform leftgunTransform;
    public Transform leftgunboneTransform;

    // Variables of Ammo and Bomb Prefeb
    public GameObject bulletPrefab;
    public GameObject bombPrefab;
    public bool abletoShoot;

    // Variables for Booster
    public float jetpackForce = 5f;
    public float jetpackFuel = 100f;
    public float jetpackFuelConsumptionRate = 10f;
    public float jetpackFuelRechargeRate = 5f;
    private float currentJetpackFuel;

    // boolen for checking gun state is reloading or not
    bool isReloading;

    // Variables For Player Health
    public int maxHealth = 100;


    public int currentHealth;
    public float healthRecoveryRate = 2f;

    // Variables For Guns Scriptable Obect
    public List<Gun> guns; // List of ScriptableObject Guns
    public Gun bomb;
    public int currentGunIndex = 0;
    private int alternateGunIndex = -1;

    // Getting Player Rigidbody for Physics
    private Rigidbody2D rb;

    // Boolen for Chceking that Player is In Ground Or Not
    private bool isGrounded;

    // Variables For Jpoystick
    public FixedJoystick movementJoystick;
    public FixedJoystick aimJoystick;
    public PhotonView view;

    // Variables For Changing The Gun
    public bool isSwitching;

    void Start()
    {

        view = transform.GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody2D>();
        currentJetpackFuel = jetpackFuel;
        currentHealth = maxHealth;
        Gun Gun = guns[currentGunIndex];
        Camera.main.orthographicSize = Gun.maxScope;
        UIManager.instance.AmmoInfo_text.text = Gun.currentAmmoInMagazine.ToString() + " / " + Gun.currentTotalAmmo.ToString();
        movementJoystick = GameObject.FindWithTag("joyStick_Movement").GetComponent<FixedJoystick>();
        aimJoystick = GameObject.FindWithTag("joyStick_Aim").GetComponent<FixedJoystick>();
        UpdateHealthImage();

        // Initialize all guns
        foreach (Gun gun in guns)
        {
            gun.Initialize();
        }
        Camera MainCamera = Camera.main;
        if (PhotonNetwork.InRoom)
        {

            UIManager.instance.ScopeText.text = (MainCamera.orthographicSize - 4).ToString() + "x";

            leftgunTransform.GetComponent<SpriteRenderer>().sprite = guns[0].GunSprite;
            rightgunTransform.GetComponent<SpriteRenderer>().sprite = guns[1].GunSprite;

            StartCoroutine(AutoHealthRecovery());
        }
    }

    void Update()
    {
        if (view.IsMine || !PhotonNetwork.InRoom)
        {
            Move();
            Jump();
            Aim();
            Shoot();
            Jetpack();
            UpdateBoosterLevel();
            HandleGunSwitching();
        }

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
        // Get joystick input for aiming
        float aimHorizontal = aimJoystick.Horizontal;
        float aimVertical = aimJoystick.Vertical;

        // Calculate the aim direction
        Vector3 aimDirection = new Vector3(aimHorizontal, aimVertical, 0);
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        //gunTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));


        if (PhotonNetwork.InRoom)
        {
            view.RPC("RPCAim", RpcTarget.Others, view.ViewID, aimDirection, angle);
        }

        // Flip character and gun based on aim direction
        if (aimDirection.x > 0)
        {
            leftgunboneTransform.localScale = new Vector3(-1, -1, 1f);
            leftgunboneTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 57.745f));
            transform.localScale = new Vector3(-0.15f, 0.15f, 1);
            //  gunTransform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (aimDirection.x < 0)
        {
            leftgunboneTransform.localScale = new Vector3(1, 1, 1f);
            leftgunboneTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 47.25f));
            transform.localScale = new Vector3(0.15f, 0.15f, 1);
            //   gunTransform.localScale = new Vector3(-1f, -1f, 1f);
        }
    }

    [PunRPC]
    void RPCAim(int Player_ID, Vector3 aimDirection, float angle)
    {
        PlayerController player = PhotonNetwork.GetPhotonView(Player_ID).GetComponent<PlayerController>();
        //// Get joystick input for aiming
        //float aimHorizontal = player.aimJoystick.Horizontal;
        //float aimVertical = player.aimJoystick.Vertical;

        //// Calculate the aim direction
        //Vector3 aimDirection = new Vector3(aimHorizontal, aimVertical, 0);
        //float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        ////gunTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Flip character and gun based on aim direction
        if (aimDirection.x > 0)
        {
            player.leftgunboneTransform.localScale = new Vector3(-1, -1, 1f);
            player.leftgunboneTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 57.745f));
            player.transform.localScale = new Vector3(-0.15f, 0.15f, 1);
            //  gunTransform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (aimDirection.x < 0)
        {
            player.leftgunboneTransform.localScale = new Vector3(1, 1, 1f);
            player.leftgunboneTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 47.25f));
            player.transform.localScale = new Vector3(0.15f, 0.15f, 1);
            //   gunTransform.localScale = new Vector3(-1f, -1f, 1f);
        }
    }

    void Shoot()
    {
        Gun currentGun = guns[currentGunIndex];
        if (isReloading == false)
        {

            if (Input.GetMouseButton(0) && Time.time > guns[currentGunIndex].lastShotTime + guns[currentGunIndex].shootCooldown)
            {
                if (currentGun.currentAmmoInMagazine < currentGun.magazineSize)
                {
                    UIManager.instance.ReloadButton.interactable = true;
                }
                else
                {
                    UIManager.instance.ReloadButton.interactable = false;
                }

                if (aimJoystick.Horizontal > .75 || aimJoystick.Vertical > .75 || aimJoystick.Horizontal < -.75f || aimJoystick.Vertical < -.75f)
                {
                    abletoShoot = true;
                }
                else
                {
                    abletoShoot = false;
                }
                if (abletoShoot && Time.time > guns[currentGunIndex].lastShotTime + guns[currentGunIndex].shootCooldown)
                {

                    if (currentGun.currentAmmoInMagazine > 0)
                    {
                        if (currentGun.gunType == "Bomb")
                        {
                            if (PhotonNetwork.InRoom)
                            {
                                view.RPC("ThrowBomb", RpcTarget.All, view.ViewID);
                            }
                            else
                            {
                                ThrowBomb(view.ViewID);
                            }
                        }
                        else
                        {
                            if (PhotonNetwork.InRoom)
                            {
                                view.RPC("FireBullet", RpcTarget.All, view.ViewID);
                            }
                            else
                            {
                                FireBullet(view.ViewID);
                            }

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
    }

    [PunRPC]
    void FireBullet(int firePoint_ID)
    {
        PlayerController FirePOINT;
        if (PhotonNetwork.InRoom)
        {
            FirePOINT = PhotonNetwork.GetPhotonView(firePoint_ID).transform.GetComponent<PlayerController>();
        }
        else
        {
            FirePOINT = transform.GetComponent<PlayerController>();
        }
        GameObject bullet = Instantiate(bulletPrefab, FirePOINT.firePoint.position, FirePOINT.firePoint.rotation);
        bullet.tag = "Player_Bullet";
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.gun = guns[currentGunIndex];  // Pass the current gun reference
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = FirePOINT.firePoint.right * -bulletSpeed;
    }

    [PunRPC]
    public void ThrowBomb(int firePoint_ID)
    {
        PlayerController FirePOINT;
        if (PhotonNetwork.InRoom)
        {
            FirePOINT = PhotonNetwork.GetPhotonView(firePoint_ID).transform.GetComponent<PlayerController>();
        }
        else
        {
            FirePOINT = transform.GetComponent<PlayerController>();
        }
        GameObject bomb = Instantiate(bombPrefab, firePoint.position, firePoint.rotation);
        // bomb.tag = "Player_Bomb";
        Rigidbody2D bombRb = bomb.GetComponent<Rigidbody2D>();
        bombRb.isKinematic = false;
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
        if(view.IsMine)
        {

        float fillAmount = (float)currentHealth / maxHealth;
        UIManager.instance.healthImage.fillAmount = fillAmount;
        }
    }

    [PunRPC]
    public void TakeDamage(int damageAmount, int Health_ID)
    {
        PlayerController Health;
        Health = PhotonNetwork.GetPhotonView(Health_ID).transform.GetComponent<PlayerController>();
        Health.currentHealth -= damageAmount;
        UpdateHealthImage();

        if (Health.currentHealth <= 0)
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
            currentHealth -= (int)botGun.damagePerBullet;
            UpdateHealthImage();

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        if (collision.gameObject.CompareTag("Player_Bullet"))
        {
            if (view.IsMine)
            {

                Gun PlayerGun = collision.gameObject.GetComponent<Bullet>().gun;
                //TakeDamage(PlayerGun.damagePerBullet);
                view.RPC("TakeDamage", RpcTarget.All, PlayerGun.damagePerBullet, view.ViewID);
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

    public void HandleGunSwitching()
    {

        if (isSwitching == true)
        {
            if (alternateGunIndex == -1)
            {
                alternateGunIndex = currentGunIndex;
                currentGunIndex = (currentGunIndex + 1) % guns.Count;

            }
            else
            {
                int temp = currentGunIndex;
                currentGunIndex = alternateGunIndex;
                alternateGunIndex = temp;

            }
            Gun currentGun = guns[currentGunIndex];
            Gun alternateGun = guns[alternateGunIndex];
            leftgunTransform.GetComponent<SpriteRenderer>().sprite = currentGun.GunSprite;
            rightgunTransform.GetComponent<SpriteRenderer>().sprite = alternateGun.GunSprite;
            UIManager.instance.GunIndex = currentGunIndex;
            UIManager.instance.AmmoInfo_text.text = currentGun.currentAmmoInMagazine.ToString() + " / " + currentGun.currentTotalAmmo.ToString();
            UIManager.instance.CurrentGunImage.GetComponent<Image>().sprite = currentGun.GunSprite;
            UIManager.instance.ScopeText.text = (Camera.main.orthographicSize - 4).ToString() + "x";
            Camera.main.orthographicSize = currentGun.maxScope;
            isSwitching = false;
        }
    }

    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        stream.SendNext(currentHealth);
    //    }
    //    else
    //    {
    //        currentHealth = (int)stream.ReceiveNext();
    //    }
    //}
}