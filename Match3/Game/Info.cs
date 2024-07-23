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
    public class Info : GameObject
    {
        private InfoView view;
        
        public string text;

        public override void OnCreate()
        {
            if (view == null)
            {
                view = new InfoView();
            }
            GamePageViewModel.AddView(view);

            text = "";
        }

        public override void Render()
        {
            GamePageViewModel.SetViewLayoutBounds(view, Position, size);
            view.SetText(text);
        }

        public override void OnParentPositionChanged(Vector2 delta)
        {
            Position += delta;
        }

        public override void SetVisibility(bool value)
        {
            view.IsVisible = value;
        }
    }
}
