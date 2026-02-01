using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class SkullController : MonoBehaviour {
    public Camera mainCamera;

    [Header("Settings")]
    public MaskProjectile skullPrefab;

    public Transform headSlot;
    public float throwPower = 5f;
    public float recallDuration = 0.3f;
    public float speed = 10f;

    [Header("Juice")]
    public float hitstopDuration = 0.05f;

    public float shakeStrength = 0.2f;

    [Header("Teleport Settings")]
    public float teleportDuration = 0.25f;
    public Ease teleportEase = Ease.OutQuart;
    public Collider2D collider;
    private bool isBodyTeleporting;

    private MaskProjectile activeSkull;
    public bool isHeadless;
    Sequence activeSequence;

    public event Action<bool> OnHeadlessStateChanged;

    public void OnFire(InputAction.CallbackContext context) {
        if (context.performed) {
            if (!isHeadless) Throw();
            else Recall();
        }
    }

    public void OnTeleport(InputAction.CallbackContext context) {
        if (context.performed && isHeadless && activeSkull) {
            if (!activeSkull.isReturning && !isBodyTeleporting) {
                TeleportToHead();
            }
        }
    }

    private void Throw() {
        isHeadless = true;
        OnHeadlessStateChanged?.Invoke(isHeadless);

        activeSkull = Instantiate(skullPrefab, headSlot.position, Quaternion.identity);

        activeSkull.OnCollisionDetected += HandleCollision;
        activeSkull.OnHitEnemy += HandleEnemyHit;
        Vector3 targetPos = transform.position + new Vector3(transform.localScale.x * throwPower, 0, 0);

        transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0), 0.2f);
        activeSequence?.Kill();
        activeSequence = DOTween.Sequence();

        float dir = transform.localScale.x;
        var direction = Vector3.right * dir * throwPower;
        var velocity = direction * speed;
        activeSkull.Launch(velocity);
    }

    private void HandleCollision(Vector3 pos, GameObject obj) {
        activeSequence?.Kill();
        activeSequence = DOTween.Sequence();
        activeSequence
            .Join(mainCamera.transform.DOShakePosition(0.1f, 0.1f))
            .OnComplete(() => {
            activeSequence = null;
        });
    }

    private void HandleEnemyHit(GameObject enemy) {
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
                transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.2f);
                mainCamera.DOShakePosition(0.1f, 0.1f);
                activeSequence = null;
                isHeadless = false;
                OnHeadlessStateChanged?.Invoke(isHeadless);
            });
    }

    private void TeleportToHead() {
        isBodyTeleporting = true;

        var rb = GetComponent<Rigidbody2D>();
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        collider.isTrigger = true;

        activeSequence?.Kill();
        activeSequence = DOTween.Sequence();

        activeSequence
            .Join(transform.DOMove(activeSkull.transform.position, teleportDuration).SetEase(teleportEase))
            .Join(transform.DOPunchScale(new Vector3(-0.1f, 0.1f, 0), teleportDuration))
            .OnComplete(() => {
                rb.gravityScale = originalGravity;
                collider.isTrigger = false;

                Destroy(activeSkull.gameObject);
                activeSkull = null;

                isHeadless = false;
                isBodyTeleporting = false;

                OnHeadlessStateChanged?.Invoke(isHeadless);

                mainCamera.DOShakePosition(0.1f, 0.2f);
                transform.DOPunchScale(new Vector3(0.2f, -0.1f, 0), 0.2f);
            });
    }
}
