using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField] private float force = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
        if (!playerRb) return;

        playerRb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }
}