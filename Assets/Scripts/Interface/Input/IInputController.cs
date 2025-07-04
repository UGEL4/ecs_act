using System.Collections.Generic;
using Entitas;

public interface IInputController
{
    public void Initialize(Contexts contexts);
    public List<KeyRecord> GetCommands(long currentFrame);
    public void Destroy();
}