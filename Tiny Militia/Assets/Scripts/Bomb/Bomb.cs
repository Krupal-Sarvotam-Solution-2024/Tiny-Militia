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

        poisionbomb,

    }
    public bombtype type;
    public float damagetime =.07f;
    public float timeToExplode;

    public float damage;
    public float range;
    public GameObject blast;

    public PlayerController playerController;
    bool readtoExplode;
    bool explotion_Started;
    bool exploded;

    // Start is called before the first frame update

    void Start()
    {
        
        if (type == bombtype.explodebomb || type == bombtype.timebomb)
            StartCoroutine(waitTillExplode());
    }
    

    public IEnumerator waitTillExplode()
    {
        yield return new WaitForSeconds(timeToExplode);

        GameObject[] allplayer = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(allplayer.Length);
        GameObject[] allBot = GameObject.FindGameObjectsWithTag("Bot");
        exploded = true;

        if (PhotonNetwork.InRoom)
        {

            foreach (var item in allplayer)
            {

                float Distance = Vector3.Distance(item.transform.position, transform.position);

                if (Distance < 3)
                {
                    float damageamount =  damage / Distance;

                    if (item.GetComponent<PhotonView>())
                    {
                        playerController.view.RPC("TakeDamage", RpcTarget.All, damageamount, item.GetComponent<PhotonView>().ViewID);// -= damage;
                    }

                }
            }
            //blast.gameObject.SetActive(true);

          
        }
        else
        {
            foreach (var item in allBot)
            {
                float Distance = Vector3.Distance(item.transform.position, transform.position);

                if (Distance < range)
                {
                    float damageCount = damage / Distance;

                    item.GetComponent<BotController>().TakeDamage((int)damageCount);
                }
            }

            foreach (var item in allplayer)
            {
                float Distance = Vector3.Distance(item.transform.position, transform.position);
                if (Distance < range)
                {
                    float damageCount = damage / Distance;

                    item.GetComponent<PlayerController>().TakeDamage((int)damageCount, item.GetComponent<PhotonView>().ViewID);
                }
            }


          
        }
        blast.gameObject.SetActive(true);
        yield return new WaitForSeconds(damagetime);

       
        Destroy(this.gameObject);
        //exploding animtion

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (type == bombtype.timebomb && collision.gameObject.tag != "Player")
        {
            readtoExplode = true;
            Rigidbody2D rb = transform.GetComponent<Rigidbody2D>();
            rb.simulated = false;
            rb.isKinematic = true;

            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.rotation = 0;
            rb.totalForce = Vector2.zero;
            StopAllCoroutines();
            timeToExplode = .1f;
          

        }

        if (collision.gameObject.tag == "Player" && type == bombtype.timebomb)
        {
            if (readtoExplode && !explotion_Started)
            {
              
            }
            else if(!readtoExplode)
            {
               // readtoExplode = true;
            }

        }
        transform.GetComponent<Rigidbody2D>().simulated = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (!readtoExplode || explotion_Started)//explotion stated or insde the player range
        {
            
            if(!readtoExplode)
            {
               
                float Distance = Vector3.Distance(playerController.transform.position, transform.position);
                if(Distance> range)
                {
                    readtoExplode = true;
                }
            }
            return;
        }
        if (type == bombtype.poisionbomb )
        {
            if (exploded)
            {
                GameObject[] allplayer = GameObject.FindGameObjectsWithTag("Player");

                foreach (var item in allplayer)
                {

                    float Distance = Vector3.Distance(item.transform.position, transform.position);

                    //  item.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage, playerController.transform.GetComponent<PhotonView>().ViewID);// -= damage;
                    if (Distance < range)
                    {
                        if (PhotonNetwork.InRoom)
                        {

                            item.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage, playerController.transform.GetComponent<PhotonView>().ViewID);// -= damage;
                        }
                        else
                        {
                            item.GetComponent<PlayerController>().TakeDamage((int)damage, item.GetComponent<PhotonView>().ViewID);

                        }
                    }
                    //if (Distance < range)
                    //{
                    //  //  item.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage / Distance, playerController.transform.GetComponent<PhotonView>().ViewID);// -= damage;
                    //}
                }
            }
        }
        else if (type == bombtype.timebomb)
        {
            Debug.Log("timeBomb");
            GameObject[] allplayer = GameObject.FindGameObjectsWithTag("Player");

            foreach (var item in allplayer)
            {
                float Distance = Vector3.Distance(item.transform.position, transform.position);


                //  item.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage, playerController.transform.GetComponent<PhotonView>().ViewID);// -= damage;
                if (Distance < range)
                {
                    if (PhotonNetwork.InRoom)
                    {
                        explotion_Started = true;
                        ///readtoExplode = false;
                        //StopAllCoroutines();
                        timeToExplode = .01f;
                        StartCoroutine(waitTillExplode());

                    }
                    else
                    {

                        explotion_Started = true;
                        ///readtoExplode = false;
                        //StopAllCoroutines();
                        timeToExplode = .01f;
                        StartCoroutine(waitTillExplode());
                    }
                }

            }
        }
    }
}
