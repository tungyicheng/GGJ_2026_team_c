using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class SkullController : MonoBehaviour {
    public Camera mainCamera;

    [Header("Settings")]
    public MaskProjectile skullPrefab;

    public Transform headSlot;
    public float throwDistance = 5f;
    public float throwDuration = 0.4f;
    public float recallDuration = 0.3f;
    public float arcHeight = 6f;
    public float speed = 10f;

    [Header("Visuals")]
    public GameObject playerHeadSprite;

    [Header("Juice")]
    public float hitstopDuration = 0.05f;

    public float shakeStrength = 0.2f;

    private MaskProjectile activeSkull;
    private bool isHeadless;
    Sequence? activeSequence;

    public void OnFire(InputAction.CallbackContext context) {
        if (context.performed) {
            if (!isHeadless) Throw();
            else Recall();
        }
    }

    private void Throw() {
        isHeadless = true;
        playerHeadSprite.SetActive(false);

        activeSkull = Instantiate(skullPrefab, headSlot.position, Quaternion.identity);

        activeSkull.OnCollisionDetected += HandleCollision;
        activeSkull.OnHitEnemy += HandleEnemyHit;
        Vector3 targetPos = transform.position + new Vector3(transform.localScale.x * throwDistance, 0, 0);

        activeSequence?.Kill();
        activeSequence = DOTween.Sequence();

        Transform activeSkullTransform = activeSkull.transform;
        float dir = transform.localScale.x;
        var direction = Vector3.right * dir;
        var velocity = direction * speed;
        activeSkull.Launch(velocity);

        // activeSequence
        //     .Join(activeSkullTransform
        //         .DORotate(new Vector3(0, 0, -360 * 2 * dir), throwDuration, RotateMode.FastBeyond360)
        //         .SetEase(Ease.OutCubic)
        //         .SetLoops(-1))
        //     .Join(activeSkullTransform.DOScale(new Vector3(1.4f, 0.8f, 1f), throwDuration / 2));
    }

    private void HandleCollision(Vector3 pos, GameObject obj) {
        activeSequence?.Kill();
        activeSequence = DOTween.Sequence();
        Transform activeSkullTransform = activeSkull.transform;
        activeSequence
            // .Join(activeSkullTransform.DOScale(new Vector3(1f, 1f, 1f), throwDuration / 2))
            .Join(mainCamera.transform.DOShakePosition(0.1f, 0.1f))
            .OnComplete(() => {
            activeSequence = null;
        });
    }

    private void HandleEnemyHit(GameObject enemy) {
        // enemy.GetComponent<EnemyHealth>()?.TakeDamage(1);

        StartCoroutine(HitFeedback());
    }

    private IEnumerator HitFeedback() {
        Time.timeScale = 0f;

        mainCamera.transform.DOShakePosition(0.15f, shakeStrength);

        yield return new WaitForSecondsRealtime(hitstopDuration);

        Time.timeScale = 1f;
    }

    private void Recall() {
        if (activeSkull == null) return;
        if (activeSkull.isReturning) return;
        activeSkull.PrepareForRecall();
        activeSkull.OnHitEnemy -= HandleEnemyHit;
        activeSkull.OnHitEnemy += HandleEnemyHit;
        activeSequence?.Kill();
        activeSequence = DOTween.Sequence();

        var originalScale = activeSkull.transform.localScale;
        var targetScale = new Vector3(originalScale.x * 0.9f, originalScale.y * 0.9f, originalScale.z);
        activeSequence
            .Join(activeSkull.transform.DOScale(targetScale, 0.1f))
            .Join(activeSkull.transform.DOMove(headSlot.position, recallDuration).SetEase(Ease.InBack))
            .OnComplete(() => {
                Destroy(activeSkull.gameObject);
                activeSkull = null;
                playerHeadSprite.SetActive(true);
                transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.2f);
                mainCamera.DOShakePosition(0.1f, 0.1f);
                activeSequence = null;
                isHeadless = false;
            });
    }
}
