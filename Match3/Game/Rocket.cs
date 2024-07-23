using Match3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Game
{
    public class Rocket : Element
    {
        public enum Direction { X, Y }
        public Direction direction;
        public bool activated = false;

        public string destroyerForward;
        public string destroyerBackwards;

        public override void OnCreate()
        {
            base.OnCreate();

            activated = false;
        }

        public override void Eliminate()
        {
            eliminating = true;
            eliminationTime = 0;

            var backward = GameObject.GetGameObject<Destroyer>();
            backward.color = color;
            backward.fontSize = fontSize;
            backward.text = destroyerBackwards;
            backward.movingVector = direction == Direction.X ? new Vector2(-1, 0) : new Vector2(0, 1);
            backward.size = size;
            backward.Position = Position;

            var forward = GameObject.GetGameObject<Destroyer>();
            forward.color = color;
            forward.fontSize = fontSize;
            forward.text = destroyerForward;
            forward.movingVector = direction == Direction.X ? new Vector2(1, 0) : new Vector2(0, -1);
            forward.size = size;
            forward.Position = Position;
        }
    }
}
