using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Gun gun;



    private void Start()
    {
        Destroy(this.gameObject, 2f);
        transform.Rotate(0, 0, 90);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
