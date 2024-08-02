using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;
using PlayFab;
using System.Runtime.CompilerServices;


public class PlayerController : MonoBehaviourPunCallbacks
{
    #region All Variables

    [SerializeField]private Animator anime;

    [Space(5)]
    [Header("// Variables For Movements")]
    [Space(2)]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;


    public GameObject leftbost, rightbost;
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
    public float currentHealth;
    
    //public float healthRecoveryRate = 2f;

    [Space(5)]
    [Header("// Variables For Guns Scriptable Obect")]
    [Space(2)]
    public List<Gun> guns; // List of ScriptableObject Guns
    public Gun bomb;
    public int currentGunIndex = 0;
    private int alternateGunIndex = -1;
    public int maxbomb;
    public int[] bombsamount;

    private int totoalmomb = 3;
    public Bomb.bombtype selectedbomb;
    public GameObject[] arrow;

    public float SoundDistance;

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
    [Header("// Variables for Stop the Coroutine")]
    [Space(2)]
    public Coroutine reload_co;

    [Space(5)]
    [Header("// Variables for Sitting Position")]
    [Space(2)]
    public Transform Leftleg;
    public Transform Rightleg;

    [Space(5)]
    [Header("// Variables for Score Counter")]
    [Space(2)]
    public int Kill_Count;
    public int Score_Count;

    [Space(5)]
    [Header("// Variables For taking List of All Player")]
    public List<PlayerController> allPlayer;

    #endregion

    #region Unity Predefine Method with Own Functionality

    // Start method for checking the player's photon view and set the Camera
    void Start()
    {
       // anime = GetComponent<Animator>();
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
            //  Kill_Count = DataShow.Instance.This_Match_Kill_Count;
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
            ArrowDirectionShowing();
            if(PhotonNetwork.InRoom)
            {
                SoringPlayerBoard();
            }
        }


    }

    public void ArrowDirectionShowing()
    {

        PhotonView[] photonViews = PhotonNetwork.PhotonViews;
        for (int i = 0; i < photonViews.Length; i++)
        {
            if (photonViews[i].IsMine)
            {
                for (int j = 0; j < photonViews.Length; j++)
                {

                    float distance = Vector3.Distance(photonViews[j].gameObject.transform.position, this.gameObject.transform.position);
                    if (photonViews[i].gameObject.TryGetComponent<PlayerController>(out PlayerController obj))
                    {
                        obj.arrow[j].transform.LookAt(photonViews[j].gameObject.transform.position);
                        if (distance > 40 || distance < 10)
                        {
                            obj.arrow[j].transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
                        }
                        else
                        {
                            obj.arrow[j].transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1F, 0F, 0F, 1F / (distance * 2));
                        }
                    }
                }

            }


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

            if (currentHealth <= 0 && GameManager.Instance.isDead == 0)
            {

                Debug.Log(GameManager.Instance.isDead);
                GameManager.Instance.isDead = 1;
                Die();
                Debug.Log(GameManager.Instance.isDead);

            }
        }

        if (collision.gameObject.CompareTag("Player_Bullet"))
        {
            if (view.IsMine)
            {
                Debug.Log("this player id :-" + view.ViewID + "other player id :-" + collision.gameObject.GetComponent<Bullet>().Id);

                Gun PlayerGun = collision.gameObject.GetComponent<Bullet>().gun;
                if (collision.gameObject.GetComponent<Bullet>().Id == view.ViewID)
                    return;

                //TakeDamage(PlayerGun.damagePerBullet);
                view.RPC("TakeDamageFromHit", RpcTarget.All, PlayerGun.damagePerBullet, collision.gameObject.GetComponent<Bullet>().Id);

            }
        }

        if (collision.gameObject.CompareTag("Up/Down_Border"))
        {

            currentHealth = 0;
            UpdateHealthImage();
            Die();
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
        if (view.IsMine)
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
        Debug.Log(moveInput);
        anime.SetFloat("x_postion", moveInput);
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
        anime.SetFloat("y_postion", moveInput);
        if (moveInput > 0 && !isGrounded && currentJetpackFuel > 0)
        {
            leftbost.SetActive(true);
            rightbost.SetActive(true);
            rb.velocity = new Vector2(rb.velocity.x, jetpackForce * moveInput);
            currentJetpackFuel -= jetpackFuelConsumptionRate * Time.deltaTime;
        }
        else if (moveInput <= 0)
        {
            leftbost.SetActive(false);
            rightbost.SetActive(false);
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
        UIManager.instance.AimObject.transform.position = firePoint.transform.position;

        if (PhotonNetwork.InRoom)
        {
            view.RPC("RPCAim", RpcTarget.Others, view.ViewID, aimDirection, angle);
        }

        // Flip character and gun based on aim direction
        if (aimDirection.x > 0)
        {
            leftgunboneTransform.localScale = new Vector3(-1, -1, 1f);

            leftgunboneTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 57.745f));

            UIManager.instance.AimObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 17.7f));

            transform.localScale = new Vector3(0.15f, 0.15f, 1);
        }
        else if (aimDirection.x < 0)
        {
            leftgunboneTransform.localScale = new Vector3(1, 1, 1f);

            leftgunboneTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 47.25f));

            UIManager.instance.AimObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 7f));

            transform.localScale = new Vector3(-0.15f, 0.15f, 1);
        }
    }

    // Method for taking aim in online mode
    [PunRPC]
    void RPCAim(int Player_ID, Vector3 aimDirection, float angle)
    {
        PlayerController player = PhotonNetwork.GetPhotonView(Player_ID).GetComponent<PlayerController>();
        // Get joystick input for aiming

        // Flip character and gun based on aim direction

        //  UIManager.instance.AimObject.transform.position = firePoint.transform.position;

        if (aimDirection.x > 0)
        {
            player.leftgunboneTransform.localScale = new Vector3(-1, -1, 1f);
            player.leftgunboneTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 57.745f));

            UIManager.instance.AimObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 17.7f));


            player.transform.localScale = new Vector3(0.15f, 0.15f, 1);
        }
        else if (aimDirection.x < 0)
        {
            player.leftgunboneTransform.localScale = new Vector3(1, 1, 1f);
            player.leftgunboneTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 47.25f));

            UIManager.instance.AimObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 7f));


            player.transform.localScale = new Vector3(-0.15f, 0.15f, 1);
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
                                //   photonView.RPC("ShowingDirection", RpcTarget.All, view.ViewID);
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
        bulletScript.Id = view.ViewID;
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
        bombsamount[(int)selectedbomb]--;
        UIManager.instance.BombAmount.text ="x "+ bombsamount[(int)selectedbomb].ToString();
        PlayerController FirePOINT;
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Throwing bomb in the room");
            FirePOINT = PhotonNetwork.GetPhotonView(firePoint_ID).transform.GetComponent<PlayerController>();
        }
        else
        {
            FirePOINT = transform.GetComponent<PlayerController>();
        }
        GameObject bomb = Instantiate(bombPrefab[(int)selectedbomb], firePoint.position, Quaternion.identity);
        bomb.GetComponent<Bomb>().playerController = this;
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
        bombsamount[(int)type]++;

    }

    #endregion

    #region Methods for Taking Damage , Death and Soring List

    //Method For Taking damage from player through bullet in offline and online mode
    [PunRPC]
    public void TakeDamageFromHit(int damageAmount, int hitedplayer_id)
    {
        if (PhotonNetwork.InRoom)
        {

            PlayerController Hiterplayer;
            Hiterplayer = PhotonNetwork.GetPhotonView(hitedplayer_id).transform.GetComponent<PlayerController>();// shooter
            currentHealth -= damageAmount;
            UpdateHealthImage();
            if (currentHealth <= 0)
            {
                Hiterplayer.Kill_Count++;
                if (Hiterplayer.view.IsMine)
                {
                    //SaveApperance_KillCount
                    DataShow.Instance.Total_Kill_Count++;
                    PlayfabManager.Instance.SaveApperance_KillCount(DataShow.Instance.Total_Kill_Count);
                    PlayfabManager.Instance.SaveApperance_KD(DataShow.Instance.Total_Kill_Count / DataShow.Instance.Total_Death_Count);
                }

                if (this.view.IsMine)
                {
                    Die();
                }

                if (view.Controller.NickName == PhotonNetwork.GetPhotonView(hitedplayer_id).Controller.NickName)
                {
                    UIManager.instance.killing_text.text = PhotonNetwork.GetPhotonView(hitedplayer_id).Controller.NickName + " Eliminated";

                    UIManager.instance.killing_text.color = Color.white;
                }
                else
                {
                    UIManager.instance.killing_text.text = view.Controller.NickName + " Eliminated by " + PhotonNetwork.GetPhotonView(hitedplayer_id).Controller.NickName;

                    UIManager.instance.killing_text.color = Color.white;
                }

            }
        }

    }

    //Method For Taking damage from player through bomb in offline and online mode
    [PunRPC]
    public void TakeDamage(float damageAmount, int Health_ID)
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("damage tager  " + Health_ID + " damage giver  " + view.ViewID);

            if (PhotonNetwork.GetPhotonView(Health_ID).transform.TryGetComponent<PlayerController>(out PlayerController health))
            {


                
               // Health = PhotonNetwork.GetPhotonView(Health_ID).transform.GetComponent<PlayerController>();
                health.currentHealth -= damageAmount;
                UpdateHealthImage();

                if (health.currentHealth <= 0 && PhotonNetwork.GetPhotonView(Health_ID).IsMine)
                {
                   // if(health.view.ViewID != this.view.ViewID)
                    Kill_Count++;

                    Debug.Log("im dying");
                    health.Die();
                }
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

    public void SoringPlayerBoard()
    {
        // Get the dictionary of players in the current room
        PhotonView[] photonViews = PhotonNetwork.PhotonViews;
        allPlayer.Clear();
        for (int i = 0; i < photonViews.Length; i++)
        {
            if (photonViews[i].gameObject.TryGetComponent<PlayerController>(out PlayerController obj))//all player
            {

                allPlayer.Add(obj);



            }

        }
        // Get the dictionary of players in the current room
        //    Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
        //allPlayer.Clear();
        //foreach (var playerEntry in players)
        //{
        //    Player player = playerEntry.Value;
        //    GameObject playerObject = GetPlayerGameObject(player);
        //    if (playerObject != null)
        //    {
        //        PlayerController playerControllers = playerObject.GetComponent<PlayerController>();
        //        //if (playerControllers != null)
        //        //{
        //        //    Debug.Log($"Nickname: {playerControllers.view.Controller.NickName}, KillCount: {playerControllers.Kill_Count}");
        //        //}
        //        allPlayer.Add(playerControllers);
        //    }
        //}


        // Sort players by Kill_Count in descending order
        allPlayer.Sort((x, y) => y.GetComponent<PlayerController>().Kill_Count.CompareTo(x.GetComponent<PlayerController>().Kill_Count));

        // Update UI with sorted player data
        for (int k = 0; k < allPlayer.Count; k++)
        {
            PlayerController playerController = allPlayer[k].GetComponent<PlayerController>();
            if (playerController != null)
            {
                UIManager.instance.PlayersData[k].GetComponent<TextMeshProUGUI>().text = allPlayer[k].GetComponent<PhotonView>().Owner.NickName;
                UIManager.instance.PlayersData[k].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = playerController.Kill_Count.ToString("00");
            }
        }
    }

    // Method for Checking the health and apply the Death Function To Player
    void Die()
    {
        if (PhotonNetwork.InRoom)
        {

            GameManager.Instance.StartCoroutine("PlayerRespawn", this.gameObject.GetComponent<PlayerController>());

            UIManager.instance.Pause.gameObject.SetActive(true);

            //UIManager.instance.Info.gameObject.SetActive(false);

            UIManager.instance.RespawnTime_Text.gameObject.SetActive(true);

            UIManager.instance.PauseExitButton.gameObject.SetActive(false);

            UIManager.instance.LeaveMatch.gameObject.SetActive(false);

            GameManager.Instance.isRespawning = true;

            SoringPlayerBoard();

            /* 
            * Add All Player Information 
            * Ex.1 Player Name                     Kill
            *      Player1                          10
            *      Player2                          08
            *      Player3                          06
            *      Player4                          04
            *      Player5                          02
            *      Player6                          00
            *      
            *     ------------------------------------
            *     NOTE :- Players Position is Set According to Their Kill Count
            *     
            */

            DataShow.Instance.Total_Death_Count++;
            PlayfabManager.Instance.SaveApperance_TotalDeath(DataShow.Instance.Total_Death_Count);
            PlayfabManager.Instance.SaveApperance_KD(DataShow.Instance.Total_Kill_Count / DataShow.Instance.Total_Death_Count);
            DataShow.Instance.This_Match_Kill_Count = Kill_Count;
            PhotonNetwork.Destroy(this.gameObject);
        }
        else
        {
            UIManager.instance.Score.text = UIManager.instance.playerController.Score_Count.ToString();
            UIManager.instance.Kill.text = UIManager.instance.playerController.Kill_Count.ToString();
            UIManager.instance.High_Score.text = DataShow.Instance.High_Score_Count.ToString();
            if (GameManager.Instance.Lifes != 0)
            {
                if (GameManager.Instance.isDead == 1)
                {
                    Debug.Log(GameManager.Instance.isDead);

                    GameManager.Instance.isDead = 2;
                    Debug.Log(GameManager.Instance.isDead);

                    GameManager.Instance.Lifes -= 1;

                    UIManager.instance.LifeCount.text = "X " + GameManager.Instance.Lifes.ToString();

                    GameManager.Instance.StartCoroutine("PlayerRespawn", this.gameObject.transform.GetComponent<PlayerController>());

                    UIManager.instance.Pause.gameObject.SetActive(true);

                    UIManager.instance.RespawnTime_Text.gameObject.SetActive(true);

                    UIManager.instance.PauseExitButton.gameObject.SetActive(false);

                    UIManager.instance.LeaveMatch.gameObject.SetActive(false);

                    GameManager.Instance.isRespawning = true;
                }
            }
            else
            {
                //SceneManager.LoadScene("Menu");

                UIManager.instance.Pause.gameObject.SetActive(true);

                UIManager.instance.RespawnTime_Text.gameObject.SetActive(true);

                UIManager.instance.PauseExitButton.gameObject.SetActive(false);

                UIManager.instance.LeaveMatch.gameObject.SetActive(false);


                GameManager.Instance.isLifeLineOver = true;
            }
            GameObject Temp = Instantiate(GameManager.Instance.TempPlayer);

            Temp.transform.tag = "Player";

            Destroy(this.gameObject);
        }
    }

    GameObject GetPlayerGameObject(Player player)
    {
        // Get the player's game object by their actor number
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView photonView = obj.GetComponent<PhotonView>();
            if (photonView != null && photonView.Owner == player)
            {
                return obj;
            }
        }
        return null;
    }

    public void onGameOver()
    {
        UIManager.instance.Pause.gameObject.SetActive(true);

        UIManager.instance.RespawnTime_Text.gameObject.SetActive(false);

        UIManager.instance.PauseExitButton.gameObject.SetActive(false);

        UIManager.instance.LeaveMatch.gameObject.SetActive(true);

        UIManager.instance.LeaveMatch.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Menu";

        this.transform.GetComponent<Rigidbody2D>().isKinematic = true;

        SoringPlayerBoard();

        if (allPlayer[0].view.IsMine)
        {
            DataShow.Instance.Win_Matches_Count++;
            PlayfabManager.Instance.SaveApperance_WinMatches(DataShow.Instance.Win_Matches_Count);
        }
    }

    #endregion

    #region Methods for Gun and Bomb Changing in Both Mode (Onlline and Offline)

    // Method for Handle the gun switchiing in Online mode
    [PunRPC]
    public void HandleGunSwitching(int playerID)
    {
        if (reload_co != null)
        {
            StopCoroutine(reload_co);
        }

        UIManager.instance.ReloadImage.fillAmount = 0f;

        UIManager.instance.ReloadButton.interactable = true;


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
        if (PhotonNetwork.InRoom)
        {
            if (view.IsMine && reload_co != null)
            {
                StopCoroutine(reload_co);
            }
        }
        if (reload_co != null && !PhotonNetwork.InRoom)
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

    #region Method for Getting Data

    [PunRPC]
    public void GettingData(int data)
    {
        Kill_Count = data;
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
}
