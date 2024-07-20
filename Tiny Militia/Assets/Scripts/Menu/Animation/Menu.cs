using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Menu : MonoBehaviour
{
    [SerializeField]private Transform battlebutton,customBattle,Servivalmode;

    [SerializeField] private Transform Playbutton;

    // Start is called before the first frame update
    void Start()
    {
        
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
}
