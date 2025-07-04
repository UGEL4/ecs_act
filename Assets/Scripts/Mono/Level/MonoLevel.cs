using Entitas;
using Entitas.Unity;
using UnityEngine;

public class MonoLevel : MonoBehaviour, IViewController
{
    protected Contexts contexts;
    protected GameEntity entity;

    public Vector3 Position
    {
        get {
            return transform.position;
        }
        set {
            transform.position = value;
        }
    }

    public Vector3 Rotation
    {
        get {
            return transform.rotation.eulerAngles;
        }
        set {
            transform.rotation = Quaternion.Euler(value);
        }
    }

    public Vector3 Scale
    {
        get {
            return transform.localScale;
        }
        set {
            transform.localScale = value;
        }
    }

    public Transform GetLevelRoot()
    {
        return transform;
    }

    public bool Active
    {
        get {
            return gameObject.activeSelf;
        }
        set {
            gameObject.SetActive(value);
        }
    }

    public void InitializeView(Contexts contexts, IEntity entity)
    {
        this.contexts = contexts;
        this.entity   = (GameEntity)entity;
        long levelId  = this.entity.id.value;
        gameObject.Link(this.entity);
        MonoGameObjectInstance.AddLevel(levelId, gameObject);
        OnLevelCreate();
    }

    public void Link(IEntity entity)
    {
        gameObject.Link(entity);
    }

    public void Unlink()
    {
        gameObject.Unlink();
    }

    public void DestroyView()
    {
        Destroy(this);
    }

    void OnDestroy()
    {
        gameObject.Unlink();
        contexts = null;
        entity   = null;
    }

    void OnLevelCreate()
    {
        long worldId = this.entity.level.worldId;
        var worldObj = MonoGameObjectInstance.GetWorld(worldId);
        if (worldObj == null)
        {
            Debug.LogError($"Could not find world with id:{worldId}");
            return;
        }
        var position   = entity.position.value;
        var unityWorld = worldObj.GetComponent<MonoWorld>();
        if (unityWorld != null)
        {
            var levelRootTransform = unityWorld.GetLevelRoot();
            gameObject.transform.SetParent(levelRootTransform, false);
        }
        else
        {
            Debug.LogError($"Could not find wunityWorld with id:{worldId}");
            return;
        }
        gameObject.transform.localPosition = position;
        gameObject.transform.localRotation = entity.rotation.value;
        gameObject.transform.localScale    = entity.scale.value;
    }

    public IEntity GetEntity()
    {
        return entity;
    }
}
