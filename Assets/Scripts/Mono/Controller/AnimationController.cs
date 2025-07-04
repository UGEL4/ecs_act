using UnityEngine;

public class AnimationController : IAnimationController
{
    private Animator animator;

    private MonoCharacter owner;

    public AnimationController(MonoCharacter owner, Animator animator)
    {
        this.owner    = owner;
        this.animator = animator;
    }

    public void PlayAnimation(string animationName)
    {
        animator?.CrossFade(animationName, 0, 0, 0);
        Debug.Log($"PlayAnimation: {animationName}");
    }

    public void OnDestroy()
    {
        animator = null;
        owner    = null;
    }
}