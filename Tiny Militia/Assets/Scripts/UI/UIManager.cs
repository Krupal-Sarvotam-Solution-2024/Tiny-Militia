using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public Canvas Pause;

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
        PhotonView view = playerController.view;
        //  playerController.


        if (PhotonNetwork.InRoom)
        {
            view.RPC("HandleGunSwitching", RpcTarget.All, view.ViewID);
        }
        //else
        //{
        //    playerController.HandleGunSwitching();
        //}
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        Pause.gameObject.SetActive(true);
    }

    public void PunchButton()
    {
        PhotonView view = playerController.view;
        playerController.isPunching = true;
        playerController.leftgunboneTransform.parent.transform.GetComponent<PolygonCollider2D>().enabled = true;
        StartCoroutine("PunchingCoroutine", 2f);
        if (PhotonNetwork.InRoom)
        {
            //playerController.TakeDamage(playerController.guns[playerController.currentGunIndex].damagePerBullet * 2,);
        }
        else
        {
            //playerController.TakeDamage(playerController.guns[playerController.currentGunIndex].damagePerBullet * 2,);
        }
    }

    public void BombChange()
    {

    }

    public void LeaveMatchButton()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ContinueMatch()
    {
        Time.timeScale = 1f;
        Pause.gameObject.SetActive(false);
    }

    IEnumerator PunchingCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        playerController.isPunching = false;
        playerController.leftgunboneTransform.parent.transform.GetComponent<PolygonCollider2D>().enabled = false;
    }

}