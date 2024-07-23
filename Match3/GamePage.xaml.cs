using Match3.ViewModel;
using System.Numerics;

namespace Match3;

public partial class GamePage : ContentPage
{
    private List<IView> views = new List<IView>();

    public GamePage()
    {
        InitializeComponent();
        BindingContext = new GamePageViewModel(this);
    }

    public Vector2 GetSize()
    {
        return new Vector2((float)GameView.Width, (float)GameView.Height);
    }

    public void AddView(IView view)
    {
        if (!views.Contains(view))
        {
            views.Add(view);
            GameView.Add(view);
        }
    }

    public void SetViewLayoutBounds(IView view, Vector2 position, Vector2 size)
    {
        GameView.SetLayoutBounds(view, new Rect(position.X, position.Y, size.X, size.Y));
    }
}