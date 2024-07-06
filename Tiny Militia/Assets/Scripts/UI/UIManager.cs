using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    // Variables For Showing Data in Canvas
    public Image boosterLevelImage;
    public Image healthImage;
    public Image GunImage;
    public Image ReloadImage;
    public Image CurrentGunImage;
    public TextMeshProUGUI AmmoInfo_text;
    public TextMeshProUGUI ScopeText;
    public Button ReloadButton;
    public int GunIndex;
    public PlayerController playerController;

    private void Awake()
    {
        instance = this;

    }
    private void Start()
    {
        playerController = GameManager.Instance.PlayerManager;

    }
    public void ReloadGun()
    {
        Gun currentgun;
        currentgun = playerController.guns[GunIndex];
        if (currentgun.currentAmmoInMagazine < currentgun.magazineSize)
        {
            StartCoroutine(playerController.Reload(currentgun));
        }
    }

    public void Zoom()
    {
        Camera cam = Camera.main;
        Gun currentgun = playerController.guns[GunIndex];
        if (cam.orthographicSize < currentgun.maxScope)
        {
            cam.orthographicSize += 1;

        }
        else
        {
            cam.orthographicSize = 5;
        }
        ScopeText.text = (cam.orthographicSize - 4).ToString() + "x";
    }


    public void ThrowBomb()
    {
        PhotonView view = playerController.view;
        //playerController.ThrowBomb();

        if (PhotonNetwork.InRoom)
        {
            view.RPC("ThrowBomb", RpcTarget.All, view.ViewID);
        }
        else
        {
            playerController.ThrowBomb(view.ViewID);
        }
    }

    public void GunChange()
    {
        playerController.isSwitching = true;
        playerController.HandleGunSwitching();
    }
}