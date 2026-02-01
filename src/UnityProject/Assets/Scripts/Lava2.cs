using UnityEngine;

/// <summary>
/// 岩漿碰撞：玩家碰到後傳送回 (0, 0)
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Lava2 : MonoBehaviour
{
    [Header("傳送目標座標")]
    public Vector2 respawnPosition = new Vector2(215f, 32f);

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 確保只影響 Player
        if (!other.CompareTag("Player")) return;

        // 直接傳送玩家
        other.transform.position = respawnPosition;

        // 如果玩家有 Rigidbody2D，建議清空速度（避免殘留動量）
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}
