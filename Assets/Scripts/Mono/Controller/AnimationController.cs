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

    public void PlayAnimation(string animationName, float normalizedTransitionDuration, int layer, float normalizedTimeOffset)
    {
        /*
            假设我们有从A->B进行混合过渡，
            A状态持续1s，B状态持续2s， 调用函数 ：
            CrossFade(animationName, 0.2f, layer, 0.05f, 0.1f);
            我们来逐层分析计算，
            0.2f表示我们的混合会使用20%的动画时间来混合
            0.05f表示这个混合从动画播放了5%的地方开始取，也就是混合5%到25%的部分
            这个时候我们会得到一个时间为(25% * 2) = 0.5s的混合动画，这个动画会在动画过渡的时候播放
            0.1f也就是表示，当我们播放这个0.5s的混合动画，会从（10% * 0.5s）= 0.05s的地方开始播放。
        */
        animator?.CrossFade(animationName, normalizedTransitionDuration, layer, normalizedTimeOffset);
        Debug.Log($"PlayAnimation: {animationName}, {normalizedTransitionDuration}, {normalizedTimeOffset}");
    }

    public void OnDestroy()
    {
        animator = null;
        owner    = null;
    }
}