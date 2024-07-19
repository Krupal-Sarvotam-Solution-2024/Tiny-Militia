using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunsData : MonoBehaviour
{
    public Gun currentData;

    SpriteRenderer gunImage;


    private void Start()
    {
        gunImage = GetComponent<SpriteRenderer>();
        gunImage.sprite = currentData.GunSprite;
    }
}
