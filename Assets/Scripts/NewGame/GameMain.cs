using UnityEngine;

namespace ACTGame
{
    public class GameMain : MonoBehaviour
    {
        private ActGame m_Game;
        void Awake()
        {

        }

        void Start()
        {
            m_Game = new ActGame();
            m_Game.Init();
        }

        void Update()
        {
            m_Game.Update();
        }

        void OnDestroy()
        {
            m_Game.Destroy();
            m_Game = null;
        }


    }
    
}