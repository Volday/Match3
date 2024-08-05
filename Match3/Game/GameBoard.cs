using Match3.Cells;
using Match3.Core;
using Match3.ViewModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Match3.Game.Rocket;

namespace Match3.Game
{
    public class GameBoard : GameObject
    {
        private GameBoardView gameBoardView;

        private Info scoreInfo;

        private Element[,] elements;

        public Action<int>? onEliminate;

        private List<Bomb> detonationQueue;
        private List<Vector2> detonationPositions;

        private enum State { Idle, Moving, Swapping }
        private static State state;

        // Vector2(-1, -1) => not selected
        private Vector2 firstSelection;
        private Vector2 secondSelection;

        private Vector2 gridSize;
        public Vector2 GridSize
        {
            get
            {
                return gridSize;
            }
            set 
            {
                gridSize = value;
                CreateField();
            }
        }

        public override void SetVisibility(bool value)
        {
            gameBoardView.IsVisible = value;
        }

        public override void OnCreate()
        {
            if (gameBoardView == null)
            {
                gameBoardView = new GameBoardView();
            }
            gameBoardView.onCellClicked += OnCellClicked;
            GamePageViewModel.AddView(gameBoardView);

            onEliminate = null;
            GamePageViewModel.onSizeChanged += OnGameViewSizeChanged;

            firstSelection = new Vector2(-1, -1);
            state = State.Idle;
            childrens = new List<GameObject>();
            detonationQueue = new List<Bomb>();
            detonationPositions = new List<Vector2>();
        }

        public override void Update(float deltaTime)
        {
            var stable = CheckIfStable();
            if (!stable) return;

            switch (state)
            {
                case State.Swapping:
                    ResolveGameBoard();
                    break;
                case State.Moving:
                    ResolveGameBoard();
                    break;
                default: break;
            }
        }

        private bool CheckIfStable()
        {
            for (int y = 0; y < elements.GetLength(1); y++)
            {
                for (int x = 0; x < elements.GetLength(0); x++)
                {
                    if (elements[x, y] == null || elements[x, y].moving) return false;
                }
            }
            return true;
        }

        private Vector2 FindElementPosition(Element element)
        {
            for (int y = 0; y < elements.GetLength(1); y++)
            {
                for (int x = 0; x < elements.GetLength(0); x++)
                {
                    if (elements[x, y] == element)
                    {
                        return new Vector2(x, y);   
                    }
                }
            }
            return new Vector2(-1, -1);
        }

        private void ResolveGameBoard()
        {
            Dictionary<Vector2, Element> elementsToEliminate = new Dictionary<Vector2, Element>();

            for (int i = 0; i < detonationQueue.Count; i++)
            {
                var position = detonationPositions[i];

                for (int y = (int)position.Y - 1; y <= (int)position.Y + 1; y++)
                {
                    for (int x = (int)position.X - 1; x <= (int)position.X + 1; x++)
                    {
                        if (x >= 0 && y >= 0 && x < elements.GetLength(0) && y < elements.GetLength(1))
                        {
                            var pos = new Vector2(x, y);
                            if (!elementsToEliminate.ContainsKey(pos))
                            {
                                elementsToEliminate.Add(pos, elements[x, y]);
                            }
                        }
                    }
                }

                detonationQueue[i].Detonate();
            }

            detonationQueue.Clear();
            detonationPositions.Clear();

            List<Vector2> horizontalLinesStart = new List<Vector2>();
            List<int> horizontalLinesLength = new List<int>();

            List<Vector2> verticalLinesStart = new List<Vector2>();
            List<int> verticalLinesLength = new List<int>();

            int currentTypeId = -1;
            int streak = 0;

            for (int y = 0; y < elements.GetLength(1); y++)
            {
                for (int x = 0; x < elements.GetLength(0); x++)
                {
                    var element = elements[x, y];
                    if (streak == 0)
                    {
                        currentTypeId = element.elementTypeId;
                        streak = 1;
                    }
                    else if (element.elementTypeId == currentTypeId)
                    {
                        streak += 1;
                        if (x == elements.GetLength(0) - 1 && streak >= 3)
                        {
                            horizontalLinesStart.Add(new Vector2((x - streak) + 1, y));
                            horizontalLinesLength.Add(streak);
                        }
                    }
                    else 
                    {
                        if (streak >= 3)
                        {
                            horizontalLinesStart.Add(new Vector2(x - streak, y));
                            horizontalLinesLength.Add(streak);
                        }
                        currentTypeId = element.elementTypeId;
                        streak = 1;
                    }
                }
                currentTypeId = -1;
                streak = 0;
            }

            for (int x = 0; x < elements.GetLength(0); x++)
            {
                for (int y = 0; y < elements.GetLength(1); y++)
                {
                    var element = elements[x, y];
                    if (streak == 0)
                    {
                        currentTypeId = element.elementTypeId;
                        streak = 1;
                    }
                    else if (element.elementTypeId == currentTypeId)
                    {
                        streak += 1;
                        if (y == elements.GetLength(1) - 1 && streak >= 3)
                        {
                            verticalLinesStart.Add(new Vector2(x, (y - streak) + 1));
                            verticalLinesLength.Add(streak);
                        }
                    }
                    else
                    {
                        if (streak >= 3)
                        {
                            verticalLinesStart.Add(new Vector2(x, y - streak));
                            verticalLinesLength.Add(streak);
                        }
                        currentTypeId = element.elementTypeId;
                        streak = 1;
                    }
                }
                currentTypeId = -1;
                streak = 0;
            }

            List<Vector2> bombPlaces = new List<Vector2>();
            List<int> bombTypeIds = new List<int>();
            List<Vector2> rocketPlaces = new List<Vector2>();
            List<int> rocketTypeIds = new List<int>();

            for (int i = 0; i < horizontalLinesStart.Count; i++)
            {
                bool bomb = false;
                int typeId = 0;
                var place = new Vector2(0, 0);
                var lastTimeMoved = DateTime.MinValue;
                for (int j = 0; j < horizontalLinesLength[i]; j++)
                {
                    var pos = new Vector2(horizontalLinesStart[i].X + j, horizontalLinesStart[i].Y);
                    var element = elements[(int)pos.X, (int)pos.Y];
                    if (!elementsToEliminate.ContainsKey(pos))
                    {
                        elementsToEliminate.Add(pos, element);
                    }

                    if (horizontalLinesLength[i] >= 5)
                    {
                        bomb = true;
                        typeId = element.elementTypeId;
                        if (lastTimeMoved < element.lastTimeMoved)
                        {
                            lastTimeMoved = element.lastTimeMoved;
                            place = pos;
                        }
                    }
                    else if (horizontalLinesLength[i] == 4)
                    {
                        typeId = element.elementTypeId;
                        if (lastTimeMoved < element.lastTimeMoved)
                        {
                            lastTimeMoved = element.lastTimeMoved;
                            place = pos;
                        }
                    }
                }
                if (lastTimeMoved != DateTime.MinValue)
                {
                    if (bomb)
                    {
                        bombPlaces.Add(place);
                        bombTypeIds.Add(typeId);
                    }
                    else
                    {
                        rocketPlaces.Add(place);
                        rocketTypeIds.Add(typeId);
                    }
                }
            }

            for (int i = 0; i < verticalLinesStart.Count; i++)
            {
                bool bomb = false;
                int typeId = 0;
                var place = new Vector2(0, 0);
                var lastTimeMoved = DateTime.MinValue;
                for (int j = 0; j < verticalLinesLength[i]; j++)
                {
                    var pos = new Vector2(verticalLinesStart[i].X, verticalLinesStart[i].Y + j);
                    var element = elements[(int)pos.X, (int)pos.Y];
                    if (!elementsToEliminate.ContainsKey(pos))
                    {
                        elementsToEliminate.Add(pos, element);
                    }

                    if (verticalLinesLength[i] >= 5)
                    {
                        bomb = true;
                        typeId = element.elementTypeId;
                        if (lastTimeMoved < element.lastTimeMoved)
                        {
                            lastTimeMoved = element.lastTimeMoved;
                            place = pos;
                        }
                    }
                    else if (verticalLinesLength[i] == 4)
                    {
                        typeId = element.elementTypeId;
                        if (lastTimeMoved < element.lastTimeMoved)
                        {
                            lastTimeMoved = element.lastTimeMoved;
                            place = pos;
                        }
                    }
                }
                if (lastTimeMoved != DateTime.MinValue)
                {
                    if (bomb)
                    {
                        bombPlaces.Add(place);
                        bombTypeIds.Add(typeId);
                    }
                    else
                    {
                        rocketPlaces.Add(place);
                        rocketTypeIds.Add(typeId);
                    }
                }
            }

            if (elementsToEliminate.Count == 0)
            {
                if (state == State.Swapping)
                {
                    if (firstSelection != new Vector2(-1, -1))
                    {
                        SwapElements(firstSelection, secondSelection);

                        elements[(int)firstSelection.X, (int)firstSelection.Y].selected = false;
                        elements[(int)secondSelection.X, (int)secondSelection.Y].selected = false;

                        firstSelection = new Vector2(-1, -1);
                        secondSelection = new Vector2(-1, -1);
                    }
                }
                state = State.Idle;
                return;
            }

            if (state == State.Swapping)
            {
                elements[(int)firstSelection.X, (int)firstSelection.Y].selected = false;
                elements[(int)secondSelection.X, (int)secondSelection.Y].selected = false;

                firstSelection = new Vector2(-1, -1);
                secondSelection = new Vector2(-1, -1);
            }

            bool rocketsToDetonate = true;
            while (rocketsToDetonate)
            {
                rocketsToDetonate = false;
                var elementsToEliminateCopy = elementsToEliminate.ToList();
                foreach (var element in elementsToEliminateCopy)
                {
                    if (element.Value as Rocket == null) continue;

                    var rocket = element.Value as Rocket;
                    if (rocket.activated) continue;

                    rocketsToDetonate = true;
                    rocket.activated = true;
                    if (rocket.direction == Direction.X)
                    {
                        for (int x = 0; x < elements.GetLength(0); x++)
                        {
                            var pos = new Vector2(x, element.Key.Y);
                            if (!elementsToEliminate.ContainsKey(pos))
                            {
                                elementsToEliminate.Add(pos, elements[(int)pos.X, (int)pos.Y]);
                            }
                        }
                    }
                    if (rocket.direction == Direction.Y)
                    {
                        for (int y = 0; y < elements.GetLength(1); y++)
                        {
                            var pos = new Vector2(element.Key.X, y);
                            if (!elementsToEliminate.ContainsKey(pos))
                            {
                                elementsToEliminate.Add(pos, elements[(int)pos.X, (int)pos.Y]);
                            }
                        }
                    }
                }
            }

            foreach (var element in elementsToEliminate)
            {
                if (element.Value as Bomb != null)
                {
                    (element.Value as Bomb).detonaationPosition = element.Key;
                }
                element.Value.Eliminate();
                childrens.Remove(element.Value);
                elements[(int)element.Key.X, (int)element.Key.Y] = null;
            }

            onEliminate?.Invoke(elementsToEliminate.Count);

            for (var i = 0; i < bombPlaces.Count; i++)
            {
                var bomb = CreateBomb(bombTypeIds[i]);
                bomb.size = size / GridSize;
                bomb.Position = Position + bomb.size * bombPlaces[i];
                elements[(int)bombPlaces[i].X, (int)bombPlaces[i].Y] = bomb;
            }

            for (var i = 0; i < rocketPlaces.Count; i++)
            {
                if (bombPlaces.Contains(rocketPlaces[i])) continue;
                var rocket = CreateRocket(rocketTypeIds[i]);
                rocket.size = size / GridSize;
                rocket.Position = Position + rocket.size * rocketPlaces[i];
                elements[(int)rocketPlaces[i].X, (int)rocketPlaces[i].Y] = rocket;
            }

            Fall();
            FillGaps();
        }

        private void Fall()
        {
            state = State.Moving;
            bool emptySlot = false;
            int lowestY = 0;
            for (int x = 0; x < elements.GetLength(0); x++)
            {
                for (int y = elements.GetLength(1) - 1; y >= 0; y--)
                {
                    var element = elements[x, y];
                    if (element != null && !emptySlot) continue;

                    if (!emptySlot)
                    {
                        emptySlot = true;
                        lowestY = y;
                    }

                    if (element != null)
                    {
                        element.Fall(element.Position - new Vector2(0, (y - lowestY) * element.size.Y));
                        elements[x, lowestY] = element;
                        elements[x, y] = null;
                        y = lowestY;
                        emptySlot = false;
                    }
                }
                emptySlot = false;
            }
        }

        private void FillGaps()
        {
            state = State.Moving;
            for (int x = 0; x < elements.GetLength(0); x++)
            {
                for (int y = elements.GetLength(1) - 1; y >= 0; y--)
                {
                    var element = elements[x, y];
                    if (element != null) continue;

                    var typeId = GameRandom.Random.Next(0, 5);
                    elements[x, y] = CreteElement(typeId);
                    elements[x, y].size = size / GridSize;
                    var newPosition = Position + elements[x, y].size * new Vector2(x, y);
                    elements[x, y].Position = new Vector2(newPosition.X, newPosition.Y - 200);
                    elements[x, y].Fall(newPosition);
                }
            }
        }

        private void SwapElements(Vector2 first, Vector2 second)
        {
            state = State.Swapping;

            var firstElement = elements[(int)first.X, (int)first.Y];
            var secondElement = elements[(int)second.X, (int)second.Y];

            firstElement.Swap(secondElement.Position);
            secondElement.Swap(firstElement.Position);

            elements[(int)first.X, (int)first.Y] = secondElement;
            elements[(int)second.X, (int)second.Y] = firstElement;
        }

        public override void Render()
        {
            GamePageViewModel.SetViewLayoutBounds(gameBoardView, Position, size);
        }

        public void OnGameViewSizeChanged()
        {
            var gameViewSize = GamePageViewModel.GameViewSize;
            Position = gameViewSize / 2 - size / 2;
        }

        private void CreateField()
        {
            gameBoardView.CreateField(GridSize);
            FillBoardWithElements();
        }

        private void FillBoardWithElements()
        {
            elements = new Element[(int)GridSize.X, (int)GridSize.Y];

            for (int y = 0; y < elements.GetLength(1); y++)
            {
                for (int x = 0; x < elements.GetLength(0); x++)
                {
                    var cellPosition = new Vector2(x, y);
                    var element = GetFittingElement(cellPosition);
                    element.size = size / GridSize;
                    element.Position = Position + element.size * cellPosition;
                    elements[x, y] = element;
                }
            }
        }

        //Element that will not match in its cell
        private Element GetFittingElement(Vector2 cellPosition)
        {
            List<int> possibleId = new List<int>() { 0, 1, 2, 3, 4 };

            if (cellPosition.X > 1)
            {
                var first = elements[(int)cellPosition.X - 1, (int)cellPosition.Y];
                var second = elements[(int)cellPosition.X - 2, (int)cellPosition.Y];
                if (first != null && second != null && first.elementTypeId == second.elementTypeId)
                {
                    possibleId.Remove(first.elementTypeId);
                }
            }

            if (cellPosition.X < GridSize.X - 2)
            {
                var first = elements[(int)cellPosition.X + 1, (int)cellPosition.Y];
                var second = elements[(int)cellPosition.X + 2, (int)cellPosition.Y];
                if (first != null && second != null && first.elementTypeId == second.elementTypeId)
                {
                    possibleId.Remove(first.elementTypeId);
                }
            }

            if (cellPosition.X < GridSize.X - 1 && cellPosition.X > 0)
            {
                var first = elements[(int)cellPosition.X + 1, (int)cellPosition.Y];
                var second = elements[(int)cellPosition.X - 1, (int)cellPosition.Y];
                if (first != null && second != null && first.elementTypeId == second.elementTypeId)
                {
                    possibleId.Remove(first.elementTypeId);
                }
            }

            if (cellPosition.Y > 1)
            {
                var first = elements[(int)cellPosition.X, (int)cellPosition.Y - 1];
                var second = elements[(int)cellPosition.X, (int)cellPosition.Y - 2];
                if (first != null && second != null && first.elementTypeId == second.elementTypeId)
                {
                    possibleId.Remove(first.elementTypeId);
                }
            }

            if (cellPosition.Y < GridSize.Y - 2)
            {
                var first = elements[(int)cellPosition.X, (int)cellPosition.Y + 1];
                var second = elements[(int)cellPosition.X, (int)cellPosition.Y + 2];
                if (first != null && second != null && first.elementTypeId == second.elementTypeId)
                {
                    possibleId.Remove(first.elementTypeId);
                }
            }

            if (cellPosition.Y < GridSize.Y - 1 && cellPosition.Y > 0)
            {
                var first = elements[(int)cellPosition.X, (int)cellPosition.Y + 1];
                var second = elements[(int)cellPosition.X, (int)cellPosition.Y - 1];
                if (first != null && second != null && first.elementTypeId == second.elementTypeId)
                {
                    possibleId.Remove(first.elementTypeId);
                }
            }

            var typeId = possibleId[GameRandom.Random.Next(0, possibleId.Count)];
            return CreteElement(typeId);
        }

        //To Remove
        private Element CreteElement(int elementTypeId)
        {
            var element = ElementFactory.CreteElement(elementTypeId);
            childrens.Add(element);
            return element;
        }

        private Bomb CreateBomb(int elementTypeId)
        {
            var bomb = ElementFactory.CreateBomb(elementTypeId);
            childrens.Add(bomb);
            bomb.readyToDetonate += OnReadyToDetonate;
            return bomb;
        }

        private Rocket CreateRocket(int elementTypeId)
        {
            var direction = (GameRandom.Random.Next(0, 2) % 2 == 0) ? Direction.X : Direction.Y;
            var rocket = ElementFactory.CreateRocket(elementTypeId, direction);
            childrens.Add(rocket);
            return rocket;
        }

        private void OnReadyToDetonate(Bomb bomb, Vector2 pos)
        {
            detonationQueue.Add(bomb);
            detonationPositions.Add(pos);
        }

        private void OnCellClicked(Vector2 cellPosition)
        {
            if (state != State.Idle) return;

            if (firstSelection == new Vector2(-1, -1))
            {
                firstSelection = cellPosition;
                elements[(int)firstSelection.X, (int)firstSelection.Y].selected = true;
            }
            else
            {
                if (cellPosition == firstSelection + new Vector2(1, 0) ||
                    cellPosition == firstSelection + new Vector2(0, 1) ||
                    cellPosition == firstSelection + new Vector2(-1, 0) ||
                    cellPosition == firstSelection + new Vector2(0, -1))
                {
                    elements[(int)cellPosition.X, (int)cellPosition.Y].selected = true;
                    secondSelection = cellPosition;
                    SwapElements(firstSelection, secondSelection);
                }
                else
                {
                    elements[(int)firstSelection.X, (int)firstSelection.Y].selected = false;
                    firstSelection = new Vector2(-1, -1);
                }
            }
        }

        public override void OnDestroy()
        {
            gameBoardView.onCellClicked -= OnCellClicked;
            GamePageViewModel.onSizeChanged -= OnGameViewSizeChanged;
        }
    }
}
