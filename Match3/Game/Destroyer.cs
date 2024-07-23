using Match3.Core;
using Match3.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Game
{
    public class Destroyer : GameObject
    {
        private DestroyerView view;

        public Color color;
        public int fontSize;
        public string text;
        public Vector2 movingVector;
        private float speed = 1000;

        public override void OnCreate()
        {
            if (view == null)
            {
                view = new DestroyerView();
            }
            GamePageViewModel.AddView(view);
        }

        public override void Update(float deltaTime)
        {
            Position += movingVector * speed * deltaTime;
        }

        public override void Render()
        {
            GamePageViewModel.SetViewLayoutBounds(view, Position, size);
            view.SetTextColor(color);
            view.SetFontSize(fontSize);
            view.SetText(text);
        }
    }
}
