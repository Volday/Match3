using Match3.Core;
using Microsoft.Maui.Controls;
using System.Numerics;

namespace Match3.Cells;

public partial class GameBoardView : ContentView
{
    private Dictionary<Button, Vector2> cellPositions = new Dictionary<Button, Vector2>();
    public Action<Vector2>? onCellClicked;

	public GameBoardView()
	{
		InitializeComponent();
    }

    public void CreateField(Vector2 gridSize)
    {
        Board.Children.Clear();
        Board.RowDefinitions.Clear();
        Board.ColumnDefinitions.Clear();
        cellPositions.Clear();

        for (int i = 0; i < gridSize.Y; i++)
        {
            Board.RowDefinitions.Add(new RowDefinition());
        }

        for (int i = 0; i < gridSize.X; i++)
        {
            Board.ColumnDefinitions.Add(new ColumnDefinition());
        }

        Style boardCellStyle = null;
        if (App.Current.Resources.TryGetValue("BoardCell", out var boardCell))
        {
            boardCellStyle = (Style)boardCell;
        }

        for (int y = 0; y < gridSize.Y; y++)
        {
            for (int x = 0; x < gridSize.X; x++)
            {
                var btn = new Button();
                btn.Style = boardCellStyle;
                Board.Add(btn);
                Board.SetColumn(btn, x);
                Board.SetRow(btn, y);
                cellPositions.Add(btn, new Vector2(x, y));
                btn.Clicked += OnCellClicked;
            }
        }
    }

    private void OnCellClicked(object? sender, EventArgs e)
    {
        var cellPosition = new Vector2(0, 0);
        if (sender as Button == null) return;
        if (!cellPositions.ContainsKey((Button)sender)) return;
        cellPosition = cellPositions[(Button)sender];
        onCellClicked?.Invoke(cellPosition);
    }
}