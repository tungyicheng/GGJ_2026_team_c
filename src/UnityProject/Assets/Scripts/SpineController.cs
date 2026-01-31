using Spine.Unity;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpineController : MonoBehaviour {
    public SkeletonAnimation skeletonAnimation;
    public string Idle;
    public string MoveEnter;
    public string MoveExit;
    public string MoveLoop;

    bool isMoving;

    public void OnMove(InputAction.CallbackContext context) {
        var moveInput = context.ReadValue<float>();
        var newIsMoving = Mathf.Abs(moveInput) > 0f;
        if (newIsMoving != isMoving) {
            isMoving = newIsMoving;
            if (isMoving) {
                PlayMoveStart();
            } else {
                PlayMoveEnd();
            }
        }
    }


    private void PlayMoveStart()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, MoveEnter, false);
        skeletonAnimation.AnimationState.AddAnimation(0, MoveLoop, true, 0);
    }

    private void PlayMoveEnd()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, MoveExit, false);
        skeletonAnimation.AnimationState.AddAnimation(0, Idle, true, 0);
    }
}
