using Match3.Core;
using Match3.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Game
{
    public static class GameManager
    {
        private static GameBoard gameBoard;
        private static int score;
        private static Info scoreInfo;
        private static TimeInfo timeInfo;

        private static bool gameFinished = false;

        public static void StartGame()
        {
            GameLoop.Restart();

            gameFinished = false;

            gameBoard = GameObject.GetGameObject<GameBoard>();
            var gameViewSize = GamePageViewModel.GameViewSize;
            gameBoard.size = new Vector2(500, 500);
            gameBoard.OnGameViewSizeChanged();
            gameBoard.GridSize = new Vector2(8, 8);
            gameBoard.onEliminate += AddScore;

            scoreInfo = GameObject.GetGameObject<Info>();
            score = 0;
            scoreInfo.text = $"Score: {score}";
            scoreInfo.Position = gameBoard.Position + new Vector2(0, gameBoard.size.Y);
            scoreInfo.size = new Vector2(gameBoard.size.X, 50);
            gameBoard.childrens.Add(scoreInfo);

            timeInfo = GameObject.GetGameObject<TimeInfo>();
            timeInfo.gamelangth = 60;
            timeInfo.gameStartTime = DateTime.UtcNow;
            timeInfo.text = $"Time left: {timeInfo.gamelangth}";
            timeInfo.Position = gameBoard.Position - new Vector2(0, 50);
            timeInfo.size = new Vector2(gameBoard.size.X, 50);
            timeInfo.OnTimeIsUp += OnTimeIsUp;
            gameBoard.childrens.Add(timeInfo);
        }

        private static void AddScore(int value)
        {
            if (gameFinished) return;

            score += value;
            scoreInfo.text = $"Score: {score}";
        }

        private static void OnTimeIsUp()
        {
            gameFinished = true;

            var gameOver = GameObject.GetGameObject<GameOver>();
        }
    }
}
