using Entitas;

public sealed class RegisterLevelViewServiceSystem : IInitializeSystem
{
    private readonly MetaContext metaContext;
    private readonly ILevelViewService service;

    public RegisterLevelViewServiceSystem(Contexts contexts, ILevelViewService viewService)
    {
        metaContext = contexts.meta;
        this.service = viewService;
    }

    public void Initialize()
    {
        metaContext.ReplaceLevelViewService(service);
    }
}