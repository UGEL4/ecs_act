public sealed class ServiceRegisterSystem : Feature
{
    public ServiceRegisterSystem(Contexts contexts, Service service)
    {
        Add(new RegisterViewServiceSystem(contexts, service.viewService));
        Add(new RegisterLevelViewServiceSystem(contexts, service.levelViewService));
        Add(new RegisterWorldViewServiceSystem(contexts, service.worldViewService));
        Add(new RegisterInputServiceSystem(contexts, service.inputService));
    }
}