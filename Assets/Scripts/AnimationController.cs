using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator playerAnimator;    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    public void PlayerAnimation(string AnimationName){
        playerAnimator.Play(AnimationName);
    }
}
