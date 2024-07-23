using Match3.Core;
using Match3.ViewModel;
using System.Diagnostics;
using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Game
{
    public class Element : GameObject
    {
        protected ElementView view;

        public DateTime lastTimeMoved;

        public bool selected;
        public bool moving;
        public bool eliminating;

        public int elementTypeId;
        public Color color;
        public int fontSize;
        public string textIdle;
        public string textSelected;
        public string textFall;

        protected Vector2 target;
        protected Vector2 movingVector;
        protected float speed = 2;
        protected float eliminationTime;

        public override void SetVisibility(bool value)
        {
            view.IsVisible = value;
        }

        public override void OnCreate()
        {
            if (view == null)
            {
                view = new ElementView();
            }
            GamePageViewModel.AddView(view);

            selected = false;
            moving = false;
            eliminating = false;
            elementTypeId = 0;
            textIdle = string.Empty;
            textSelected = string.Empty;
            textFall = string.Empty;
            lastTimeMoved = DateTime.UtcNow;
        }

        public override void Update(float deltaTime)
        {
            if (moving)
            {
                Move(deltaTime);
            }

            if (eliminating)
            {
                eliminationTime -= deltaTime;
                if (eliminationTime <= 0)
                {
                    GameObject.Destroy(this);
                }
            }
        }

        private void Move(float deltaTime)
        {
            var vectorToTarget = target - Position;
            var movingDistance = (movingVector * deltaTime).Length();
            if (vectorToTarget.Length() < movingDistance)
            {
                Position = target;
                moving = false;
                lastTimeMoved = DateTime.UtcNow;
                return;
            }

            var angle = GetSignedAngleBetween(vectorToTarget, movingVector);
            var angleToRotate = 180 * speed * deltaTime;
            if (angleToRotate > Math.Abs(angle)) angleToRotate = angle;
            if (angle < 0) angleToRotate *= -1;
            movingVector = RotateVector(movingVector, angleToRotate);

            Position += movingVector * deltaTime;

            movingVector *= 1 + deltaTime;
        }

        public Vector2 RotateVector(Vector2 v, float degrees)
        {
            float radians = degrees * (float)Math.PI / 180.0f;
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            float newX = v.X * cos - v.Y * sin;
            float newY = v.X * sin + v.Y * cos;

            return new Vector2(newX, newY);
        }

        public static float GetSignedAngleBetween(Vector2 v1, Vector2 v2)
        {
            Vector2 normV1 = Vector2.Normalize(v1);
            Vector2 normV2 = Vector2.Normalize(v2);

            float dotProduct = Vector2.Dot(normV1, normV2);
            dotProduct = Math.Clamp(dotProduct, -1.0f, 1.0f);
            float angleInRadians = (float)Math.Acos(dotProduct);
            float angleInDegrees = angleInRadians * (180.0f / (float)Math.PI);
            float crossProduct = normV1.X * normV2.Y - normV1.Y * normV2.X;

            if (crossProduct > 0)
            {
                return -angleInDegrees;
            }
            else if (crossProduct < 0)
            {
                return angleInDegrees;
            }
            else
            {
                return 0;
            }
        }

        public override void Render()
        {
            GamePageViewModel.SetViewLayoutBounds(view, Position, size);
            view.SetTextColor(color);
            view.SetFontSize(fontSize);

            if (selected)
            {
                view.SetText(textSelected);
            }
            else if (moving)
            {
                view.SetText(textFall);
            }
            else
            {
                view.SetText(textIdle);
            }
        }

        public override void OnParentPositionChanged(Vector2 delta)
        {
            Position += delta;
            target += delta;
        }

        public void Swap(Vector2 target)
        {
            moving = true;
            this.target = target;

            var vectorToRotate = (target - Position);
            movingVector = RotateVector(vectorToRotate, 90) * speed;
        }

        public void Fall(Vector2 target)
        {
            moving = true;
            this.target = target;

            movingVector = RotateVector(new Vector2(0, 100), new Random().Next(-15, 15));
        }

        public virtual void Eliminate()
        {
            moving = true;
            eliminating = true;

            target = Position - new Vector2(0, -1000);
            var angle = new Random().Next(-140, 140);
            if (angle == 0) angle = 140;
            movingVector = RotateVector(new Vector2(0, -100), angle);

            eliminationTime = 5;
        }
    }
}
