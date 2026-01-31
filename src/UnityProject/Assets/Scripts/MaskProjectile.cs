using UnityEngine;
using System;

public class MaskProjectile : MonoBehaviour {
    public event Action<Vector3, GameObject> OnCollisionDetected;
    public event Action<GameObject> OnHitEnemy;

    private Rigidbody2D rb;
    private Collider2D col;
    private bool hasHit = false;
    public bool isReturning = false;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) return;

        if (!isReturning && !hasHit) {
            hasHit = true;
            OnCollisionDetected?.Invoke(transform.position, other.gameObject);
            Launch();
            rb.AddForce(Vector2.up * 2f, ForceMode2D.Impulse);
        }

        if (other.CompareTag("Enemy")) {
            OnHitEnemy?.Invoke(other.gameObject);
        }
    }

    public void Launch(Vector3 velocity = default) {
        rb.bodyType = RigidbodyType2D.Dynamic;
        col.isTrigger = false;
        rb.linearVelocity = velocity;
        rb.angularVelocity = 0f;
    }

    public void PrepareForRecall() {
        isReturning = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        col.isTrigger = true;
        hasHit = false;
    }
}
