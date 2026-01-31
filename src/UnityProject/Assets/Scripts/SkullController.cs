using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class SkullController : MonoBehaviour {
    [Header("Settings")]
    public GameObject skullPrefab;

    public Transform headSlot;
    public float throwDistance = 5f;
    public float throwDuration = 0.4f;
    public float recallDuration = 0.3f;

    [Header("Visuals")]
    public GameObject playerHeadSprite;

    private GameObject activeSkull;
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

        Vector3 targetPos = headSlot.position + new Vector3(transform.localScale.x * throwDistance, 0, 0);

        activeSequence?.Kill();
        activeSequence = DOTween.Sequence();

        Transform activeSkullTransform = activeSkull.transform;
        float dir = transform.localScale.x;

        float arcHeight = 0.6f;

        activeSequence
            .Join(activeSkullTransform.DOMoveX(targetPos.x, throwDuration).SetEase(Ease.OutCubic))
            .Join(activeSkullTransform.DOMoveY(activeSkullTransform.position.y + arcHeight, throwDuration / 2)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutQuad))
            .Join(activeSkullTransform
                .DORotate(new Vector3(0, 0, -360 * 2 * dir), throwDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic))
            .Join(activeSkullTransform.DOScale(new Vector3(1.4f, 0.6f, 1f), throwDuration / 2)
                .SetLoops(2, LoopType.Yoyo))
            .OnComplete(() => {
                activeSkullTransform.DOShakePosition(0.15f, 0.2f);
                activeSequence = null;
            });
    }

    private void Recall() {
        activeSequence?.Kill();
        activeSequence = DOTween.Sequence();

        activeSequence
            .Append(activeSkull.transform.DOScale(new Vector3(0.8f, 1.2f, 1f), 0.1f))
            .Append(activeSkull.transform.DOMove(headSlot.position, recallDuration).SetEase(Ease.InBack))
            .OnComplete(() => {
                Destroy(activeSkull);
                activeSkull = null;
                playerHeadSprite.SetActive(true);
                transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.2f);
                Camera.main.DOShakePosition(0.1f, 0.1f);
                activeSequence = null;
                isHeadless = false;
            });
    }
}
