using UnityEngine;

public class BotController : MonoBehaviour
{
    public float moveSpeed = 1.5f; // Movement speed of the bot
    public GameObject bulletPrefab; // Bullet prefab
    public Transform firePoint; // Fire point for bullets
    public float bulletSpeed = 10f; // Bullet Speed

    public Transform player; // Reference to the player's transform
    private Rigidbody2D rb; // Bot's rigidbody
    private float lastShotTime; // Time of last shot
    public int currentHealth; // Current health of the bot

    public Gun[] guns; // Array of guns the bot can use
    private int currentGunIndex; // Index of the currently equipped gun

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        currentHealth = 100; // Initial health of the bot
        lastShotTime = Time.time; // Initialize last shot time

        // Initialize with the first gun
        currentGunIndex = 0;
    }

    void Update()
    {
        MoveTowardsPlayer();
        Shoot();
    }

    void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
        }
        else 
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Shoot()
    {
        if (Time.time > lastShotTime + guns[currentGunIndex].shootCooldown)
        {
            if (player != null)
            {
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                bullet.tag = "Bot_Bullet";
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                bulletScript.gun = guns[currentGunIndex];  // Pass the current gun reference
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                bulletRb.velocity = (player.position - firePoint.position).normalized * bulletSpeed;
            }
            lastShotTime = Time.time;
        }
    }


    public void SwitchGun(int gunIndex)
    {
        if (gunIndex >= 0 && gunIndex < guns.Length)
        {
            currentGunIndex = gunIndex;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Handle bot death logic here (e.g., play death animation, spawn particles)
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player_Bullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                TakeDamage(bullet.gun.damagePerBullet);
            }
        }
    }
}
