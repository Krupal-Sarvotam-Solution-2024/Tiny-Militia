using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Menu : MonoBehaviour
{
    public Transform battlebutton;
   

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void playButton()
    {
        battlebutton.DOMoveY(battlebutton.position.y+125,1f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
