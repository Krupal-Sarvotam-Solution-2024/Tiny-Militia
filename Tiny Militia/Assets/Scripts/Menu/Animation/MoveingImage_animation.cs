using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class MoveingImage_animation : MonoBehaviour
{
    private Vector3 startingpos;
    [SerializeField]float time;
    [SerializeField]float power;
    // Start is called before the first frame update
    void Start()
    {
        startingpos = transform.position;
        transform.DOMoveX(transform.position.x + power, time);
        StartCoroutine(waitforReset());

    }

    IEnumerator waitforReset()
    {
        yield return new WaitForSeconds(time);
        transform.position = new Vector3(startingpos.x,transform.parent.position.y);
        transform.DOMoveX(transform.position.x + power, time);
        StartCoroutine(waitforReset());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
