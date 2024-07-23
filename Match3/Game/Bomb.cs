using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Game
{
    public class Bomb : Element
    {
        public Action<Bomb, Vector2>? readyToDetonate;
        public float detonationTime;
        public Vector2 detonaationPosition;
        public string detonationText;
        private bool detonation;

        public override void OnCreate()
        {
            base.OnCreate();

            readyToDetonate = null;
            detonationTime = 0.25f;
            detonation = false;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (detonation)
            {
                detonationTime -= deltaTime;
                if (detonationTime <= 0)
                {
                    readyToDetonate?.Invoke(this, detonaationPosition);
                    detonation = false;
                }
            }
        }

        public override void Render()
        {
            base.Render();

            if (eliminating)
            {
                view.SetText(detonationText);
            }
        }

        public override void Eliminate()
        {
            detonation = true;
        }

        public void Detonate()
        { 
            eliminating = true;
            eliminationTime = 0.20f;
            size *= 2;
            fontSize *= 3;
            Position -= size / 4;
        }
    }
}
