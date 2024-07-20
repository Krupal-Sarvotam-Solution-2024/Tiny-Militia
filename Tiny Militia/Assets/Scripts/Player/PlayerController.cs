using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlayerController : MonoBehaviourPunCallbacks
{
    [Space(5)]
    [Header("// Variables For Movements")]
    [Space(2)]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Space(5)]
    [Header("// Variables For Shooting")]
    [Space(2)]
    public Transform gunTransform;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public Transform rightgunTransform;
    public Transform leftgunTransform;
    public Transform leftgunboneTransform;

    [Space(5)]
    [Header("// Variables of Ammo and Bomb Prefeb")]
    [Space(2)]
    public GameObject bulletPrefab;
    public GameObject[] bombPrefab;
    public bool abletoShoot;

    [Space(5)]
    [Header("// Variables for Booster")]
    [Space(2)]
    public float jetpackForce = 5f;
    public float jetpackFuel = 100f;
    public float jetpackFuelConsumptionRate = 10f;
    public float jetpackFuelRechargeRate = 5f;
    private float currentJetpackFuel;

    [Space(5)]
    [Header("// boolen for checking gun state is reloading or not")]
    [Space(2)]
    bool isReloading;

    [Space(5)]
    [Header(" // Variables For Player Health")]
    [Space(2)]
    public int maxHealth = 100;

    [Space(5)]
    [Header("")]
    [Space(2)]
    public int currentHealth;
    //public float healthRecoveryRate = 2f;

    [Space(5)]
    [Header("// Variables For Guns Scriptable Obect")]
    [Space(2)]
    public List<Gun> guns; // List of ScriptableObject Guns
    public Gun bomb;
    public int currentGunIndex = 0;
    private int alternateGunIndex = -1;
    public int maxbomb;
    private int explosivebomb =3,timebomb=0,poisenbomb=0;
    private int totoalmomb =3;
    public Bomb.bombtype selectedbomb;
   

    [Space(5)]
    [Header("// Getting Player Rigidbody for Physics")]
    [Space(2)]
    private Rigidbody2D rb;

    [Space(5)]
    [Header("// Boolen for Chceking that Player is In Ground Or Not")]
    [Space(2)]
    private bool isGrounded;

    [Space(5)]
    [Header(" // Variables For Jpoystick")]
    [Space(2)]
    public FixedJoystick movementJoystick;
    public FixedJoystick aimJoystick;
    public PhotonView view;

    [Space(5)]
    [Header("// Variables For Punching")]
    [Space(2)]
    public bool isPunching;


    [Space(5)]
    [Header("// Variable for Stop the Coroutine")]
    [Space(2)]
    public Coroutine reload_co;

    [Space(5)]
    [Header("// Variable for Sitting Position")]
    [Space(2)]
    public Transform Leftleg;
    public Transform Rightleg;

    [Space(5)]
    [Header("// Variables for Score Counter")]
    [Space(2)]
    public int Kill_Count;
    public int Score_Count;


    #region Unity Predefine Method with Own Functionality

    // Start method for checking the player's photon view and set the Camera
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

        }
        StartCoroutine(AutoHealthRecovery());
    }

    // Update method for User Controls
    void Update()
    {
        if (view.IsMine || !PhotonNetwork.InRoom)
        {
            Move();
            Jump();
            Aim();
            Shoot();
            Jetpack();
            Sitting();
            UpdateBoosterLevel();
        }


    }

    // Collision Enter Method for CHecking the Player is in Which State
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Bot_Bullet"))
        {
            Gun botGun = collision.gameObject.GetComponent<Bullet>().gun;
            currentHealth -= botGun.damagePerBullet;
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

        if (isPunching == true)
        {
            Gun currentgun;
            currentgun = guns[currentGunIndex];


            if (collision.gameObject.transform.TryGetComponent<BotController>(out BotController bot))
            {
                bot.currentHealth -= currentgun.damagePerBullet * 2;
            }
            else
            {
                view.RPC("TakeDamage", RpcTarget.All, currentgun.damagePerBullet * 2, collision.transform.GetComponent<PlayerController>().view.ViewID);
            }
        }

    }

    // Collision Exit Method for CHecking the Player is in Which State
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<GunsData>().currentData.gunName == guns[0].gunName)
        {
            if (guns[0].currentTotalAmmo < guns[0].maxAmmo)
            {
                guns[0].currentTotalAmmo = guns[0].maxAmmo;
                UIManager.instance.AmmoInfo_text.text = guns[0].currentAmmoInMagazine.ToString() + " / " + guns[0].currentTotalAmmo.ToString();
            }
        }
        else if (collision.GetComponent<GunsData>().currentData.gunName == guns[1].gunName)
        {
            if (guns[1].currentTotalAmmo < guns[1].maxAmmo)
            {
                guns[1].currentTotalAmmo = guns[1].maxAmmo;
                UIManager.instance.AmmoInfo_text.text = guns[1].currentAmmoInMagazine.ToString() + " / " + guns[1].currentTotalAmmo.ToString();
            }
        }
        else if (collision.GetComponent<GunsData>().currentData.gunName != guns[0].gunName ||
            collision.GetComponent<GunsData>().currentData.gunName != guns[1].gunName)
        {
            UIManager.instance.GunChangeButton.gameObject.SetActive(true);
            UIManager.instance.GunChangeButton.transform.GetChild(0).GetComponent<Image>().sprite = collision.GetComponent<GunsData>().currentData.GunSprite;
            UIManager.instance.changingGunData = collision.GetComponent<GunsData>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<GunsData>().currentData.gunName != guns[0].gunName ||
            collision.GetComponent<GunsData>().currentData.gunName != guns[1].gunName)
        {
            UIManager.instance.GunChangeButton.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Method for Updatae UI

    // Method for update booster image in UI
    void UpdateBoosterLevel()
    {
        float fillAmount = currentJetpackFuel / jetpackFuel;
        UIManager.instance.boosterLevelImage.fillAmount = fillAmount;
    }

    // Method for update Health image in UI
    void UpdateHealthImage()
    {
        if (PhotonNetwork.InRoom)
        {
            if (view.IsMine)
            {
                float fillAmount = (float)currentHealth / maxHealth;
                UIManager.instance.healthImage.fillAmount = fillAmount;
            }
        }
        else
        {
            float fillAmount = (float)currentHealth / maxHealth;
            UIManager.instance.healthImage.fillAmount = fillAmount;
        }
    }

    #endregion


    #region Methods for Movement

    // Methos for Movement
    void Move()
    {

        float moveInput = movementJoystick.Horizontal;
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

    }

    // Methos for Jump
    void Jump()
    {
        float moveInput = movementJoystick.Vertical;
        //if (Input.GetButtonDown("Jump") && isGrounded)
        if (moveInput > 0 && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    // Method for Handle the jetpack (Booster)
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

    // Method for Sitting
    void Sitting()
    {
        float moveInput = movementJoystick.Vertical;
        if (moveInput < 0 && isGrounded == true)
        {
            Leftleg.GetChild(0).transform.rotation = Quaternion.identity;
            Rightleg.GetChild(0).transform.rotation = Quaternion.identity;
            moveSpeed = 2.5f;
        }
        else if (moveInput > 0 || moveInput == 0 || !isGrounded)
        {
            Leftleg.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90f));
            Rightleg.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90f));
            moveSpeed = 5f;
        }

    }

    #endregion

    #region Methods For Shooting and Throwing Bomb

    // Method for taking aim in offline mode
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

    // Method for taking aim in online mode
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

    // Method for get value of aim joystick for shooting
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
                            leftgunTransform.GetComponentInChildren<Animator>().Play("Fire");
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
                            reload_co = StartCoroutine(Reload(currentGun));
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

    // Method for shooting in offilne and online mode
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

    // Method for throwing the bomb in offline and online mode

    [PunRPC]
    public void ThrowBomb(int firePoint_ID)
    {
        if (totoalmomb <= 0)
            return;
        totoalmomb--;
        if(selectedbomb == Bomb.bombtype.explodebomb)
        {
            explosivebomb--;
        }
        else if(selectedbomb == Bomb.bombtype.timebomb)
        {
            timebomb--; 
        }else if(selectedbomb == Bomb.bombtype.poisionbomb)
        {
            poisenbomb--;
        }

        PlayerController FirePOINT;
        if (PhotonNetwork.InRoom)
        {
            FirePOINT = PhotonNetwork.GetPhotonView(firePoint_ID).transform.GetComponent<PlayerController>();
        }
        else
        {
            FirePOINT = transform.GetComponent<PlayerController>();
        }
        GameObject bomb = Instantiate(bombPrefab[(int)selectedbomb], firePoint.position ,Quaternion.identity);
        // bomb.tag = "Player_Bomb";
        Rigidbody2D bombRb = bomb.GetComponent<Rigidbody2D>();
        bombRb.isKinematic = false;
        bombRb.velocity = FirePOINT.firePoint.right * -bulletSpeed;
    }

    public void GetBomb(Bomb.bombtype type)
    {
        if (totoalmomb >= 4)
            return;
        totoalmomb++;
        if(type == Bomb.bombtype.explodebomb)
        {
            explosivebomb++;
        }else if (type == Bomb.bombtype.timebomb)
        {
            timebomb++;
        }
        else if (type == Bomb.bombtype.poisionbomb)
        {
            poisenbomb++;
        }
    }

    #endregion

    #region Methods for Taking Damage and Death Function

    // Method For Taking damage in offline and online mode
    [PunRPC]
    public void TakeDamage(int damageAmount, int Health_ID)
    {
        if (PhotonNetwork.InRoom)
        {
            PlayerController Health;
            Health = PhotonNetwork.GetPhotonView(Health_ID).transform.GetComponent<PlayerController>();
            Health.currentHealth -= damageAmount;
            UpdateHealthImage();

            if (Health.currentHealth <= 0 && PhotonNetwork.GetPhotonView(Health_ID).IsMine)
            {
                Die();
            }
        }
        else
        {
            this.currentHealth -= damageAmount;
            UpdateHealthImage();

            if (this.currentHealth <= 0)
            {
                Die();
            }
        }
    }

    // Method for Checking the health and apply the Death Function To Player
    void Die()
    {
        if (PhotonNetwork.InRoom)
        {
            GameManager.Instance.StartCoroutine("PlayerRespawn");

            UIManager.instance.Pause.gameObject.SetActive(true);

            UIManager.instance.RespawnTime_Text.gameObject.SetActive(true);

            UIManager.instance.PauseExitButton.gameObject.SetActive(false);

       //     UIManager.instance.LeaveMatch.gameObject.SetActive(false);

            GameManager.Instance.isRespawning = true;

             /* 
             * Add All Player Information 
             * Ex.1 Player Name                     Kill
             *      Krupal                          10
             *      Kaushik                         8
             *      Tiny Militia                    6
             *      Mini Militia                    4
             *      Tiny                            2
             *      Mini                            0
             *      
             *     ------------------------------------
             *     NOTE :- Players Poaition is Set According to Their Kill Count
             */

            PhotonNetwork.Destroy(this.gameObject);
        }
        else
        {
            Debug.Log("hOW many");
            if (GameManager.Instance.Lifes != 0)
            {
                GameManager.Instance.Lifes -= 1;

                UIManager.instance.LifeCount.text = "X " + GameManager.Instance.Lifes.ToString();

                GameManager.Instance.StartCoroutine("PlayerRespawn");

                UIManager.instance.Pause.gameObject.SetActive(true);

                UIManager.instance.RespawnTime_Text.gameObject.SetActive(true);

                UIManager.instance.PauseExitButton.gameObject.SetActive(false);

                UIManager.instance.LeaveMatch.gameObject.SetActive(false);

                GameManager.Instance.isRespawning = true;
            }
            else
            {
                //SceneManager.LoadScene("Menu");

                UIManager.instance.Pause.gameObject.SetActive(true);

                UIManager.instance.RespawnTime_Text.gameObject.SetActive(true);

                UIManager.instance.PauseExitButton.gameObject.SetActive(false);

                GameManager.Instance.isDead = true;
            }
            GameObject Temp = Instantiate(gameObject);

            Temp.transform.tag = "Player";


            Destroy(this.gameObject);
        }
    }

    #endregion

    #region Methods for Gun and Bomb Changing in Both Mode (Onlline and Offline)

    // Method for Handle the gun switchiing in Online mode
    [PunRPC]
    public void HandleGunSwitching(int playerID)
    {
        StopCoroutine(reload_co);

        // Ensure final fillAmount is exactly 0

        UIManager.instance.ReloadImage.fillAmount = 0f;

        UIManager.instance.ReloadButton.interactable = true;

        //UIManager.instance.ReloadImage.gameObject.SetActive(false);

        UIManager.instance.ReloadImage.fillAmount = 1;

        isReloading = false;

        PlayerController player = PhotonNetwork.GetPhotonView(playerID).GetComponent<PlayerController>();//getting the player id who is switching


        if (player.alternateGunIndex == -1)
        {
            player.alternateGunIndex = player.currentGunIndex;
            player.currentGunIndex = (player.currentGunIndex + 1) % player.guns.Count;

        }
        else
        {
            int temp = player.currentGunIndex;
            player.currentGunIndex = player.alternateGunIndex;
            player.alternateGunIndex = temp;

        }//switching the gun


        Gun currentGun = player.guns[player.currentGunIndex];
        Gun alternateGun = player.guns[player.alternateGunIndex];
        player.leftgunTransform.GetComponent<SpriteRenderer>().sprite = currentGun.GunSprite;
        player.rightgunTransform.GetComponent<SpriteRenderer>().sprite = alternateGun.GunSprite;
        if (view.IsMine)
        {

            UIManager.instance.GunIndex = currentGunIndex;

            UIManager.instance.UI_Updates();
        }
    }

    // Mehtod for Handle the gun switching in offlie mode
    public void HandleGunSwitching()
    {
        if (reload_co != null)
        {
            StopCoroutine(reload_co);
        }

        // Ensure final fillAmount is exactly 0

        UIManager.instance.ReloadImage.fillAmount = 0f;

        UIManager.instance.ReloadButton.interactable = true;

        //UIManager.instance.ReloadImage.gameObject.SetActive(false);

        UIManager.instance.ReloadImage.fillAmount = 1;

        isReloading = false;

        PlayerController player = this.GetComponent<PlayerController>();


        if (player.alternateGunIndex == -1)
        {
            player.alternateGunIndex = player.currentGunIndex;
            player.currentGunIndex = (player.currentGunIndex + 1) % player.guns.Count;

        }
        else
        {
            int temp = player.currentGunIndex;
            player.currentGunIndex = player.alternateGunIndex;
            player.alternateGunIndex = temp;

        }//switching the gun


        Gun currentGun = player.guns[player.currentGunIndex];
        Gun alternateGun = player.guns[player.alternateGunIndex];
        player.leftgunTransform.GetComponent<SpriteRenderer>().sprite = currentGun.GunSprite;
        player.rightgunTransform.GetComponent<SpriteRenderer>().sprite = alternateGun.GunSprite;

        UIManager.instance.GunIndex = currentGunIndex;

        UIManager.instance.UI_Updates();

    }

    #endregion

    #region Enumeretor Methods

    // Coroutine For Auto Health Recovery
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


    // Coroutine For Reloading the Gun
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
        //UIManager.instance.ReloadImage.gameObject.SetActive(false);
        UIManager.instance.ReloadImage.fillAmount = 1;
        isReloading = false;

    }

    #endregion

    #region Methods which is Secoandary for Photon Which is not In Use

    /*
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHealth);
        }
        else
        {
            currentHealth = (int)stream.ReceiveNext();
        }
    }
    */

    #endregion
}