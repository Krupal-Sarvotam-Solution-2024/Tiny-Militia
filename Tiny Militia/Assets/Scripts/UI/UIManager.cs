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
    public TextMeshProUGUI AmmoInfo_text;
    public Image ReloadImage;
    public Button ReloadButton;
    public int GunIndex;
    PlayerController playerController;

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
        StartCoroutine(playerController.Reload(currentgun));
    }

    public void Zoom()
    {
        Camera cam = Camera.main;
        Gun currentgun = playerController.guns[GunIndex];
        if(cam.orthographicSize < currentgun.maxScope)
        {
            cam.orthographicSize += 1;

        }
        else
        {
            cam.orthographicSize = 5;
        }
    }

}
