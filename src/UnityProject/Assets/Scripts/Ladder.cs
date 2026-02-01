using UnityEngine;
using UnityEngine.InputSystem;

public class Ladder : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float tileHeight = 1f;
    
    private bool playerInRange = false;
    private Transform playerTransform;
    private Rigidbody2D playerRigidbody;
    private float originalGravityScale;
    
    // 讓其他腳本可以查詢玩家是否在梯子上
    public static bool IsPlayerOnLadder { get; private set; } = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        
        playerInRange = true;
        IsPlayerOnLadder = true;
        playerTransform = other.transform;
        playerRigidbody = other.GetComponent<Rigidbody2D>();
        
        if (playerRigidbody != null)
        {
            // 只保存和禁用重力
            originalGravityScale = playerRigidbody.gravityScale;
            playerRigidbody.gravityScale = 0f;
            
            // 清除垂直速度，但保留水平速度
            playerRigidbody.linearVelocity = new Vector2(playerRigidbody.linearVelocity.x, 0f);
            
            Debug.Log($"[Ladder] 進入梯子 - 重力已禁用 (原始: {originalGravityScale})");
        }
        else
        {
            Debug.LogWarning("[Ladder] Player 沒有 Rigidbody2D！");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        
        playerInRange = false;
        IsPlayerOnLadder = false;
        
        if (playerRigidbody != null)
        {
            // 恢復重力
            playerRigidbody.gravityScale = originalGravityScale;
            
            Debug.Log($"[Ladder] 離開梯子 - 重力已恢復 ({originalGravityScale})");
        }
        
        playerTransform = null;
        playerRigidbody = null;
    }

    private void FixedUpdate()
    {
        // 持續強制保持重力為 0，並清除垂直速度
        if (playerInRange && playerRigidbody != null)
        {
            playerRigidbody.gravityScale = 0f;
            
            // 只清除垂直速度，保留水平速度
            if (playerRigidbody.linearVelocity.y != 0f)
            {
                playerRigidbody.linearVelocity = new Vector2(playerRigidbody.linearVelocity.x, 0f);
            }
        }
    }

    private void Update()
    {
        if (playerInRange && Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            Debug.Log("[Ladder] 按下 F 鍵，往上爬");
            ClimbUp();
        }
        if (playerInRange && Keyboard.current != null && Keyboard.current.vKey.wasPressedThisFrame)
        {
            Debug.Log("[Ladder] 按下 V 鍵，往下爬");
            ClimbDown();
        }
    }

    private void ClimbDown()
    {
        if (playerTransform == null || playerRigidbody == null) return;

        Vector3 targetPosition = playerTransform.position + new Vector3(0f, -tileHeight, 0f);
        playerRigidbody.MovePosition(targetPosition);
    }

    private void ClimbUp()
    {
        if (playerTransform == null || playerRigidbody == null) return;

        Vector3 targetPosition = playerTransform.position + new Vector3(0f, tileHeight, 0f);
        playerRigidbody.MovePosition(targetPosition);
    }
}
