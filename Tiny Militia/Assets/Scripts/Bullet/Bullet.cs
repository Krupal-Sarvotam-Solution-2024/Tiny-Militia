using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Gun gun;
    public GameObject bloodshott;


    private void Start()
    {
        Destroy(this.gameObject, 2f);
        transform.Rotate(0, 0, 90);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = Instantiate(bloodshott,collision.collider.transform.position,Quaternion.EulerAngles(0,0,transform.rotation.z+180));
        Destroy(GetComponent<Rigidbody2D>());
        Destroy(obj, .5f);
        Destroy(this.gameObject, .5f);
        
    }
}
