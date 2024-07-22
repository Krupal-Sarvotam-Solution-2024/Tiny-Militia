using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class Menu : MonoBehaviour
{
    [SerializeField]private Transform battlebutton,customBattle,Servivalmode;

    [SerializeField] private Transform Playbutton;
    [SerializeField] private TMP_InputField Player_name;
    [SerializeField] private GameObject Playername_panel;
    [SerializeField] private TextMeshProUGUI player_nametext;

    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.NickName == "") 
        {

            Playername_panel.SetActive(true);

        }
        Debug.Log(PhotonNetwork.NickName);
    }

    public void playButton()
    {
        Playbutton.DOScale(Vector3.zero,.5f);
        battlebutton.DOMoveY(Playbutton.position.y,1f);
        customBattle.DOMoveY(Playbutton.position.y,1f);
        Servivalmode.DOMoveY(Playbutton.position.y,1f);

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void NamingUser()
    {
        PhotonNetwork.NickName = Player_name.text;
        Playername_panel.SetActive(false);
        player_nametext.text = PhotonNetwork.NickName;

    }
}
