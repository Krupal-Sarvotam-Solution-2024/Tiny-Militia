using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bomb : MonoBehaviour
{
    public enum bombtype 
    {
        explodebomb,
        timebomb,
        poisenmomb,
    
    }
    public bombtype type;
    public float timeToExplode;
    public float damage;
    public GameObject blast;
    PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        if(type == bombtype.explodebomb || type == bombtype.timebomb)
        StartCoroutine(waitTillExplode());
    }
  
    [PunRPC]
    public IEnumerator waitTillExplode()
    {
        yield return new WaitForSeconds(timeToExplode);
        GameObject[] allplayer = GameObject.FindGameObjectsWithTag("Player");

        foreach (var item in allplayer)
        {
            float Distance = Vector3.Distance(item.transform.position, transform.position);
            if (Distance < 3)
            {
                item.GetComponent<PhotonView>().RPC("TakeDamage",RpcTarget.All,damage / Distance ,playerController.transform.GetComponent<PhotonView>().ViewID);// -= damage;
            }
        }
        blast.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.7f);
       
        Destroy(this.gameObject);
        //exploding animtion

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       Rigidbody2D rb = transform.GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        rb.totalForce =Vector2.zero;

    }
    // Update is called once per frame
    void Update()
    {
        if(type == bombtype.poisenmomb)
        {
            GameObject[] allplayer = GameObject.FindGameObjectsWithTag("Player");

            foreach (var item in allplayer)
            {
                float Distance = Vector3.Distance(item.transform.position, transform.position);
                if (Distance < 3)
                {
                    item.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage / Distance, playerController.transform.GetComponent<PhotonView>().ViewID);// -= damage;
                }
            }
        }else if(type == bombtype.timebomb)
        {
            GameObject[] allplayer = GameObject.FindGameObjectsWithTag("Player");

            foreach (var item in allplayer)
            {
                float Distance = Vector3.Distance(item.transform.position, transform.position);
                if (Distance < 1)
                {
                    item.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage, playerController.transform.GetComponent<PhotonView>().ViewID);// -= damage;
                }
            }
        }  
    }
}
