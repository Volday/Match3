namespace Match3.Game;

public partial class InfoView : ContentView
{
	public InfoView()
	{
		InitializeComponent();
	}

	public void SetText(string text)
	{
		Text.Text = text;
	}
}