using System;
using System.Diagnostics;
using Unity.Mathematics;

namespace ACTGame
{
    public class ActGame
    {
        private Stopwatch m_GameTimer;
        private long m_LastTime;
        private long m_Accumulator;
        private int m_Hertz = 60;

        private GameMainLogic m_GameMainLogic;

        public void Init()
        {
            m_GameMainLogic = new GameMainLogic();
            m_GameMainLogic.Start();

            //////////////////////////////
            m_GameTimer = new Stopwatch();
            m_GameTimer.Restart();
            m_LastTime = m_GameTimer.ElapsedTicks;
            m_Accumulator = 0;
        }

        public void Update()
        {
            long DT  = TimeSpan.TicksPerSecond / m_Hertz;
            long now = m_GameTimer.ElapsedTicks;
            m_Accumulator += math.clamp(now - m_LastTime, 0, DT);
            m_LastTime = now;

            if (m_Hertz == 0)
            {
                return;
            }

            while (m_Accumulator >= DT)
            {
                m_Accumulator = 0;

                m_GameMainLogic.Update();
            }
        }

        public void Destroy()
        {
            m_GameTimer = null;
        }
    }
}