using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Gun gun;


    private void Start()
    {
        Destroy(this.gameObject, 2f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        Destroy(gameObject);
    }
}
