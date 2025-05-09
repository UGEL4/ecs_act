using System.Collections.Generic;
using Entitas;

public interface IInputController
{
    public void Initialize(Contexts contexts, IEntity entity);
    public List<KeyRecord> GetCommands(long currentFrame);
}