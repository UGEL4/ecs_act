using Entitas;
using Entitas.Unity;
using UnityEngine;

public class MonoWorld : MonoBehaviour, IViewController
{
    protected Contexts contexts;
    protected GameEntity entity;

    protected Transform levelRoot;
    public Transform GetLevelRoot() => levelRoot;

    public Vector3 Position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    public Vector3 Rotation
    {
        get { return transform.rotation.eulerAngles; }
        set { transform.rotation = Quaternion.Euler(value); }
    }

    public Vector3 Scale
    {
        get { return transform.localScale; }
        set { transform.localScale = value; }
    }

    public bool Active { get { return gameObject.activeSelf; } set { gameObject.SetActive(value); } }
    public void InitializeView(Contexts contexts, IEntity entity)
    {
        var go    = new GameObject("levelRoot");
        levelRoot = go.transform;
        levelRoot.SetParent(gameObject.transform);
        levelRoot.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        levelRoot.localScale = Vector3.one;
        this.contexts        = contexts;
        this.entity          = (GameEntity)entity;
        long worldId         = this.entity.worldId.value;
        Link(this.entity);
        MonoGameObjectInstance.AddWorld(worldId, gameObject);
        OnWorldCreate();
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

    void OnWorldCreate()
    {
        // long worldId = this.entity.level.worldId;
        // var worldObj = MonoGameObjectInstance.GetWorld(worldId);
        // if (worldObj == null)
        // {
        //     Debug.LogError("Could not find world with id:" + worldId);
        //     return;
        // }
        // var position = entity.position.value;
        // gameObject.transform.SetParent(worldObj.transform);
        // gameObject.transform.localPosition = position;
    }

    void OnDestroy()
    {
        Unlink();
        this.contexts = null;
        this.entity   = null;
    }

    public IEntity GetEntity()
    {
        return entity;
    }
}