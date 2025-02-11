using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject explosion;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D col2d;
    public Rigidbody2D rigidBody;
    public float bulletLifeTime = 2.5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Rock"))
        {
            rigidBody.linearVelocity = Vector2.zero;
            col2d.enabled = false;
            
            bullet.SetActive(false);
            explosion.SetActive(true);

            animator.Play("Explosion");

            Destroy(gameObject, 0.5f);
        }
    }

    private void OnEnable()
    {
        Invoke(nameof(DestroyBullet), bulletLifeTime);
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
