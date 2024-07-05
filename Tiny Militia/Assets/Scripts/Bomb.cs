using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float timeToExplode;
    public int damage;
    public GameObject blast;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(waitTillExplode());
    }
    public IEnumerator waitTillExplode()
    {
        yield return new WaitForSeconds(timeToExplode);
        GameObject[] allplayer = GameObject.FindGameObjectsWithTag("Player");

        foreach (var item in allplayer)
        {
            if (Vector3.Distance(item.transform.position, transform.position) < 3)
            {
                item.GetComponent<PlayerController>().currentHealth -= damage;
            }
            else
            {
                Debug.Log(Vector3.Distance(item.transform.position, transform.position));
            }
        }
        blast.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.7f);
       
        Destroy(this.gameObject);
        //exploding animtion

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
