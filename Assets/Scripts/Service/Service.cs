public class Service
{
    public readonly IViewService viewService;
    public readonly ILevelViewService levelViewService;
    public readonly IWorldViewService worldViewService;
    public readonly IInputService inputService;

    public Service(IViewService viewService, ILevelViewService levelViewService, IWorldViewService worldViewService,
        IInputService inputService)
    {
        this.viewService = viewService;
        this.levelViewService = levelViewService;
        this.worldViewService = worldViewService;
        this.inputService = inputService;
    }
}