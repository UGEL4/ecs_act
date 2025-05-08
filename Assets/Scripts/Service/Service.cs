public class Service
{
    public readonly IViewService viewService;
    public readonly ILevelViewService levelViewService;
    public readonly IWorldViewService worldViewService;

    public Service(IViewService viewService, ILevelViewService levelViewService, IWorldViewService worldViewService)
    {
        this.viewService = viewService;
        this.levelViewService = levelViewService;
        this.worldViewService = worldViewService;
    }
}