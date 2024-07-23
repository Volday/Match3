namespace Match3.Game;

public partial class DestroyerView : ContentView
{
	public DestroyerView()
	{
		InitializeComponent();
	}

	public void SetFontSize(int fontSize)
	{ 
		Text.FontSize = fontSize;
	}

    public void SetText(string text)
    {
        Text.Text = text;
    }

    public void SetTextColor(Color color)
    {
        Text.TextColor = color;
    }
}