using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  
    public Transform PlayerTransform;

    private void Start()
    {
        PlayerTransform = GameObject.FindWithTag("Player").transform;
    }
    private void Update()
     {
         this.transform.position = new Vector3(PlayerTransform.position.x, PlayerTransform.position.y, -10);
     }
}
