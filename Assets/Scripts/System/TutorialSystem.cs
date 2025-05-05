using Entitas;

public class TutorialSystem : Feature
{
    public TutorialSystem(Contexts contexts) : base("TutorialSystem")
    {
        Add(new DebugMessageSystem(contexts));
    }
}