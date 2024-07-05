using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    PlayerController playerController;

    // Variables For Showing Data in Canvas
    public Image boosterLevelImage;
    public Image healthImage;
    public Image GunImage;
    public TextMeshProUGUI AmmoInfo_text;
    public Image ReloadImage;
    public Button ReloadButton;

    private void Awake()
    {
        instance = this;
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    public int GunIndex;
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
        if(cam.orthographicSize < currentgun.magazineSize)
        {
            cam.orthographicSize += 2;

        }
        else
        {
            cam.orthographicSize = 5;
        }
    }

}
