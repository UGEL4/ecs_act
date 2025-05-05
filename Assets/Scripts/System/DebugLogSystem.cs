using System.Collections.Generic;
using Entitas;

public class UnityDebugLogSystem : ReactiveSystem<DebugEntity>
{
    ILogService _logService;
    public UnityDebugLogSystem(Contexts contexts, ILogService logService) : base(contexts.debug)
    {
        _logService = logService;
    }

    protected override void Execute(List<DebugEntity> entities)
    {
        for (int i = 0; i < entities.Count; i++)
        {
            _logService.LogMessage(entities[i].debugLog.message);
        }
    }

    protected override bool Filter(DebugEntity entity)
    {
        return entity.hasDebugLog;
    }

    protected override ICollector<DebugEntity> GetTrigger(IContext<DebugEntity> context)
    {
        return context.CreateCollector(DebugMatcher.DebugLog);
    }
}
