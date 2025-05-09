using Entitas;

public class RegisterInputServiceSystem : IInitializeSystem
{
    public InputContext inputContext;
    public IInputService inputService;

    public RegisterInputServiceSystem(Contexts contexts, IInputService inputService)
    {
        inputContext      = contexts.input;
        this.inputService = inputService;
    }

    public void Initialize()
    {
        inputContext.ReplaceInputService(inputService);
    }
}