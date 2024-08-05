namespace Match3.Game;

public partial class GameOverView : ContentView
{
	public Action? onOkClicked;

	public GameOverView()
	{
		InitializeComponent();
	}

    private void OkClicked(object sender, EventArgs e)
    {
		onOkClicked?.Invoke();
    }
}