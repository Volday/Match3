using System.Numerics;

namespace Match3.Game;

public partial class ElementView : ContentView
{
	public ElementView()
	{
		InitializeComponent();
	}

	public void SetText(string text)
	{ 
		ElementText.Text = text;
	}

    public void SetTextColor(Color color)
    {
        ElementText.TextColor = color;
    }

	public void SetFontSize(int size)
	{
        ElementText.FontSize = size;
    }
}