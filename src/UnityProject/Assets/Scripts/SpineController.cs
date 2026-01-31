using System;
using Spine.Unity;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpineController : MonoBehaviour {
    public SkullController skullController;
    public SkeletonAnimation skeletonAnimation;
    public string Idle;
    public string MoveEnter;
    public string MoveExit;
    public string MoveLoop;
    public string NoHeadIdle;
    public string NoHeadMoveEnter;
    public string NoHeadMoveExit;
    public string NoHeadMoveLoop;

    bool isMoving;

    void OnEnable() {
        skullController.OnHeadlessStateChanged += OnHeadlessStateChanged;
    }

    void OnDisable() {
        skullController.OnHeadlessStateChanged -= OnHeadlessStateChanged;
    }

    void OnHeadlessStateChanged(bool isHeadless) {
        if (isMoving)
            PlayMoveStart(isHeadless);
        else
            PlayMoveEnd(isHeadless);
    }

    public void OnMove(InputAction.CallbackContext context) {
        // 正常處理移動輸入，不管是否在梯子上
        // 玩家在梯子上也可以左右移動
        var moveInput = context.ReadValue<float>();
        var newIsMoving = Mathf.Abs(moveInput) > 0f;
        if (newIsMoving != isMoving) {
            isMoving = newIsMoving;
            if (isMoving) {
                PlayMoveStart(skullController.isHeadless);
            } else {
                PlayMoveEnd(skullController.isHeadless);
            }
        }
    }

    private void PlayMoveStart(bool isHeadless) {
        if (!isHeadless) {
            skeletonAnimation.AnimationState.SetAnimation(0, MoveEnter, false);
            skeletonAnimation.AnimationState.AddAnimation(0, MoveLoop, true, 0);
        } else {
            skeletonAnimation.AnimationState.SetAnimation(0, NoHeadMoveEnter, false);
            skeletonAnimation.AnimationState.AddAnimation(0, NoHeadMoveLoop, true, 0);
        }
    }

    private void PlayMoveEnd(bool isHeadless) {
        if (!isHeadless) {
            skeletonAnimation.AnimationState.SetAnimation(0, MoveExit, false);
            skeletonAnimation.AnimationState.AddAnimation(0, Idle, true, 0);
        } else {
            skeletonAnimation.AnimationState.SetAnimation(0, NoHeadMoveExit, false);
            skeletonAnimation.AnimationState.AddAnimation(0, NoHeadIdle, true, 0);
        }
    }
}
