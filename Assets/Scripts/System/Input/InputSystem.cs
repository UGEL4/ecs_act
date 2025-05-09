using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class InputSystem : IExecuteSystem, IInitializeSystem
{
    //private IInputService _inputService;
    private Contexts _contexts;

    private IGroup<GameEntity> gameEntities;

    public InputSystem(Contexts contexts)
    {
        _contexts    = contexts;
        gameEntities = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.InputController, GameMatcher.InputToCommand));
    }
    
    public void Initialize()
    {
        //_inputService = _contexts.input.inputService.instance;
    }

    public void Execute()
    {
        //var e = _contexts.input.CreateEntity();
        //e.AddInput(_inputService.Axis, _inputService.Rotate);
        var entities = gameEntities.GetEntities();
        foreach (var entity in entities)
        {
            var commandComp = entity.inputToCommand;
            //List<KeyRecord> newCommands = entity.inputController.instance.GetCommands()
        }
    }
}