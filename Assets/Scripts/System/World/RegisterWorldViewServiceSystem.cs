using Entitas;

public sealed class RegisterWorldViewServiceSystem : IInitializeSystem
{
    private readonly MetaContext metaContext;
    private readonly IWorldViewService service;

    public RegisterWorldViewServiceSystem(Contexts contexts, IWorldViewService viewService)
    {
        metaContext = contexts.meta;
        this.service = viewService;
    }

    public void Initialize()
    {
        metaContext.ReplaceWorldViewService(service);
    }
}