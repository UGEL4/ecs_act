using Entitas;

public class InputSystem : IExecuteSystem
{
    IInputService _inputService;
    Contexts _contexts;

    public InputSystem(Contexts contexts, IInputService inputService)
    {
        _inputService = inputService;
        _contexts = contexts;
    }

    public void Execute()
    {
        var e = _contexts.input.CreateEntity();
        e.AddInput(_inputService.Axis, _inputService.Rotate);
    }
}