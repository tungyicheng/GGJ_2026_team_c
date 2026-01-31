using UnityEngine;
using UnityEngine.InputSystem;

public class Water : MonoBehaviour
{
    [Header("基本設定")]
    [SerializeField] private string playerTag = "Player";
    
    [Header("水中物理效果")]
    [SerializeField] private float sinkSpeed = 0.5f; // 下沉速度
    [SerializeField] private float swimUpForce = 5f; // 向上游的力度
    [SerializeField] private float waterDrag = 2f; // 水的阻力
    [SerializeField] private float maxVerticalSpeed = 3f; // 最大垂直速度限制
    
    private bool playerInWater = false;
    private Transform playerTransform;
    private Rigidbody2D playerRigidbody;
    private float originalGravityScale;
    private float originalDrag;
    
    // 讓其他腳本可以查詢玩家是否在水中
    public static bool IsPlayerInWater { get; private set; } = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        
        playerInWater = true;
        IsPlayerInWater = true;
        playerTransform = other.transform;
        playerRigidbody = other.GetComponent<Rigidbody2D>();
        
        if (playerRigidbody != null)
        {
            // 保存原始設定
            originalGravityScale = playerRigidbody.gravityScale;
            originalDrag = playerRigidbody.linearDamping;
            
            // 設定水中物理
            playerRigidbody.gravityScale = 0f; // 禁用重力
            playerRigidbody.linearDamping = waterDrag; // 增加阻力
            
            Debug.Log($"[Water] 進入水中 - 已套用水中物理效果");
        }
        else
        {
            Debug.LogWarning("[Water] Player 沒有 Rigidbody2D！");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        
        playerInWater = false;
        IsPlayerInWater = false;
        
        if (playerRigidbody != null)
        {
            // 恢復原始物理設定
            playerRigidbody.gravityScale = originalGravityScale;
            playerRigidbody.linearDamping = originalDrag;
            
            Debug.Log($"[Water] 離開水中 - 已恢復正常物理");
        }
        
        playerTransform = null;
        playerRigidbody = null;
    }

    private void FixedUpdate()
    {
        if (!playerInWater || playerRigidbody == null) return;

        // 持續保持重力為 0
        playerRigidbody.gravityScale = 0f;
        
        // 緩慢下沉效果
        ApplySinking();
        
        // 限制垂直速度
        LimitVerticalSpeed();
    }

    private void Update()
    {
        if (!playerInWater) return;

        // 按空格鍵向上游
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("[Water] 按下空格鍵，向上游");
            SwimUp();
        }
    }

    /// <summary>
    /// 緩慢下沉效果
    /// </summary>
    private void ApplySinking()
    {
        if (playerRigidbody == null) return;

        // 施加向下的力，模擬緩慢下沉
        Vector2 sinkForce = new Vector2(0f, -sinkSpeed);
        playerRigidbody.AddForce(sinkForce, ForceMode2D.Force);
    }

    /// <summary>
    /// 向上游動
    /// </summary>
    private void SwimUp()
    {
        if (playerRigidbody == null) return;

        // 施加向上的瞬間力
        playerRigidbody.AddForce(Vector2.up * swimUpForce, ForceMode2D.Impulse);
    }

    /// <summary>
    /// 限制垂直速度，避免過快
    /// </summary>
    private void LimitVerticalSpeed()
    {
        if (playerRigidbody == null) return;

        Vector2 velocity = playerRigidbody.linearVelocity;
        
        // 限制垂直速度
        if (Mathf.Abs(velocity.y) > maxVerticalSpeed)
        {
            velocity.y = Mathf.Sign(velocity.y) * maxVerticalSpeed;
            playerRigidbody.linearVelocity = velocity;
        }
    }

    // 在編輯器中繪製水域範圍（方便調試）
    private void OnDrawGizmosSelected()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
            Gizmos.DrawCube(col.bounds.center, col.bounds.size);
        }
    }
}