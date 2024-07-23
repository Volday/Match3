using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Game
{
    internal class TimeInfo : Info
    {
        public DateTime gameStartTime;
        public float gamelangth;

        public bool timeiIsUp = false;

        public Action OnTimeIsUp;

        public override void OnCreate()
        {
            base.OnCreate();

            timeiIsUp = false;
            OnTimeIsUp = null;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (timeiIsUp) return;

            var timePassed = (DateTime.UtcNow - gameStartTime).TotalSeconds;
            var timeleft = ((int)(gamelangth - timePassed)).ToString();
            text = $"Time left: {timeleft}";
            if (timePassed > gamelangth)
            {
                OnTimeIsUp?.Invoke();
                timeiIsUp = true;
            }
        }
    }
}
