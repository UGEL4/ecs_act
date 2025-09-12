using Entitas;
using UnityEngine;

namespace Development
{
    public class DevelopmentEditorSceneMain
    {
        Systems _systems;

        public void Start()
        {
            var contexts = Contexts.sharedInstance;
            _systems     = new GameSystems(contexts);
            _systems.Initialize();
            EventSystem.Instance.AddInvoke(InvokeType.GameLoop, typeof(GameLoopInvoke));
            // EventSystem.Instance.AddInvoke(InvokeType.EntityUpdate, typeof(EntityUpdateTimer));
        }

        public void Update()
        {
            _systems.Execute();
            _systems.Cleanup();
        }

        public void OnDestroy()
        {
            _systems = null;
        }
    }
    public class DevelopmentEditorScene : MonoBehaviour
    {
        DevelopmentEditorSceneMain m_Main;
        void Start()
        {
            m_Main = new DevelopmentEditorSceneMain();
            m_Main.Start();
        }

        void Update()
        {

        }

        void OnDestroy()
        {
            m_Main.OnDestroy();
            m_Main = null;
        }
    }
}