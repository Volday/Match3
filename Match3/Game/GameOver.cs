using Match3.Core;
using Match3.ViewModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Match3.Game
{
    public class GameOver : GameObject
    {
        private GameOverView view;

        public override void OnCreate()
        {
            if (view == null)
            {
                view = new GameOverView();
            }
            view.onOkClicked += LoadMainMenu;
            GamePageViewModel.AddView(view);
            Position = new Vector2(0, 0);
            size = GamePageViewModel.GameViewSize;
        }

        public override void Update(float deltaTime)
        {
            size = GamePageViewModel.GameViewSize;
        }

        public override void Render()
        {
            GamePageViewModel.SetViewLayoutBounds(view, Position, size);
        }

        public override void OnDestroy()
        {
            view.onOkClicked -= LoadMainMenu;
        }

        public override void SetVisibility(bool value)
        {
            view.IsVisible = value;
        }

        public void LoadMainMenu()
        {
            LoadMainMenuAsync();
        }

        public async void LoadMainMenuAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
