using Entitas;

namespace ACTGame.ACTSystems
{
    public class TimerComponentSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> m_Group;

        public TimerComponentSystem(Contexts contexts)
        {
            m_Group = contexts.game.GetGroup(GameMatcher.ACTGameComponentTimer);
        }

        public void Execute()
        {
            foreach (var entity in m_Group.GetEntities())
            {
                var timer = entity.aCTGameComponentTimer;
                timer.curFrame += 1;
            }
        }
    }
}