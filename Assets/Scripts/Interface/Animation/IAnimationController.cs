public interface IAnimationController
{
    public void PlayAnimation(string animationName, float normalizedTransitionDuration, int layer, float normalizedTimeOffset);
    public void OnDestroy();
}