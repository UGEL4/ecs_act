using Entitas;

namespace ACTGame.Component
{
    public class TimerComponent : IComponent
    {
        public long curFrame    = 0;
        public int hertz        = 60;
        public long accumulator = 0;
        public bool isUnitTimer = false;
    }
}