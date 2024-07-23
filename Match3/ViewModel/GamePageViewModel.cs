using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Match3.ViewModel
{
    public partial class GamePageViewModel : ObservableObject
    {
        public static GamePageViewModel? instance;
        private GamePage gameView;

        public static Action? onSizeChanged;

        public static Vector2 GameViewSize
        {
            get 
            {
                if (instance != null)
                {
                    return instance.gameView.GetSize();
                }
                return new Vector2(0, 0);
            }
        }

        public GamePageViewModel(GamePage gamePage)
        {
            this.gameView = gamePage;
            if (instance == null)
            {
                instance = this;
            }

            gameView.SizeChanged += OnSizeChanged;
        }

        public static void AddView(IView view)
        { 
            instance?.gameView.AddView(view);
        }

        public static void SetViewLayoutBounds(IView view, Vector2 position, Vector2 size)
        { 
            instance?.gameView.SetViewLayoutBounds(view, position, size);
        }

        private static void OnSizeChanged(object? sender, EventArgs e)
        {
            onSizeChanged?.Invoke();
        }
    }
}
