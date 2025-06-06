using System;
using System.Collections.Generic;

public interface IRegister {}

public class RegisterFunc : IRegister
{
    public Action mCallback = null;
}

public class RegisterFunc<T> : IRegister
{
    public Action<T> mCallback = null;
}

public class RegisterFunc<T1, T2> : IRegister
{
    public Action<T1, T2> mCallback = null;
}

public class EventSystem : Singleton<EventSystem>
{
    private Dictionary<Type, Dictionary<int, object>> allInvokes = new();

    public void AddInvoke(int invokeType, Type type)
    {
        object obj     = Activator.CreateInstance(type);
        IInvoke invoke = obj as IInvoke;
        if (allInvokes.ContainsKey(invoke.Type))
        {
            try
            {
                allInvokes[invoke.Type].Add(invokeType, obj);
            }
            catch (Exception e)
            {
                throw new Exception($"action type duplicate: {invoke.Type.Name} {invokeType}", e);
            }
        }
        else
        {
            allInvokes[invoke.Type] = new()
            {
                { invokeType, obj }
            };
        }
    }

    public void Invoke<Args>(int type, Args args) where Args : struct
    {
        if (!this.allInvokes.TryGetValue(typeof(Args), out var invokeHandlers))
        {
            throw new Exception($"Invoke error: {typeof(Args).Name}");
        }
        if (!invokeHandlers.TryGetValue(type, out var invokeHandler))
        {
            throw new Exception($"Invoke error: {typeof(Args).Name} {type}");
        }

        var aInvokeHandler = invokeHandler as AInvokeHandler<Args>;
        if (aInvokeHandler == null)
        {
            throw new Exception($"Invoke error, not AInvokeHandler: {typeof(Args).Name} {type}");
        }

        aInvokeHandler.Handle(args);
    }
}