using Entitas;

public sealed class RegisterViewServiceSystem : IInitializeSystem
{
    private readonly MetaContext metaContext;
    private readonly IViewService viewService;

    public RegisterViewServiceSystem(Contexts contexts, IViewService viewService)
    {
        metaContext = contexts.meta;
        this.viewService = viewService;
    }

    public void Initialize()
    {
        metaContext.ReplaceViewService(viewService);
    }
}