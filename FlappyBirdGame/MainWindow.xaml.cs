using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;

namespace FlappyBirdGame
{
	public partial class MainWindow : Window
	{
		DispatcherTimer gameTimer = new DispatcherTimer();
		double gravity = 1.5;
		double birdSpeed = 0;
		double gameSpeed = 5; // gyorsabb játék
		int score = 0;
		bool gameOver = false;

		public MainWindow()
		{
			InitializeComponent();
			gameTimer.Interval = TimeSpan.FromMilliseconds(20);
			gameTimer.Tick += GameLoop;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			MyCanvas.Focus();
			gameTimer.Start();
		}

		private void GameLoop(object sender, EventArgs e)
		{
			if (gameOver) return;

			// Madár mozgás
			Canvas.SetTop(flappyBird, Canvas.GetTop(flappyBird) + birdSpeed);
			birdSpeed += gravity;

			// Csövek mozgatása és pontozás
			MovePipe(pipeTop1, pipeBottom1);
			MovePipe(pipeTop2, pipeBottom2);
			MovePipe(pipeTop3, pipeBottom3);

			// Ütközés ellenőrzés
			if (CheckCollision(pipeTop1) || CheckCollision(pipeBottom1) ||
				CheckCollision(pipeTop2) || CheckCollision(pipeBottom2) ||
				CheckCollision(pipeTop3) || CheckCollision(pipeBottom3))
			{
				EndGame();
			}
		}

		private void MovePipe(Image top, Image bottom)
		{
			double left = Canvas.GetLeft(top) - gameSpeed;
			Canvas.SetLeft(top, left);
			Canvas.SetLeft(bottom, left);

			// Pontszám növelése, ha a madár elhalad a cső mellett
			if (!gameOver && left + top.Width < Canvas.GetLeft(flappyBird) && top.Tag?.ToString() != "passed")
			{
				score++;
				scoreText.Content = "Score: " + score;
				top.Tag = "passed";
			}

			// Ha a cső kiment a képernyőről, visszaállítjuk
			if (left < -top.Width)
			{
				Canvas.SetLeft(top, 800);
				Canvas.SetLeft(bottom, 800);

				top.Tag = null; // újra pontozható
			}
		}

		private bool CheckCollision(Image pipe)
		{
			Rect birdRect = new Rect(Canvas.GetLeft(flappyBird), Canvas.GetTop(flappyBird), flappyBird.Width, flappyBird.Height);
			Rect pipeRect = new Rect(Canvas.GetLeft(pipe), Canvas.GetTop(pipe), pipe.Width, pipe.Height);

			return birdRect.IntersectsWith(pipeRect);
		}

		private void EndGame()
		{
			if (gameOver) return;
			gameOver = true;
			gameTimer.Stop();
			restartButton.Visibility = Visibility.Visible;
			MessageBox.Show("GAME OVER!");
		}

		private void RestartButton_Click(object sender, RoutedEventArgs e)
		{
			// Játék újraindítása
			Canvas.SetTop(flappyBird, 200);
			Canvas.SetLeft(flappyBird, 50);
			birdSpeed = 0;

			Canvas.SetLeft(pipeTop1, 400);
			Canvas.SetLeft(pipeBottom1, 400);
			Canvas.SetLeft(pipeTop2, 650);
			Canvas.SetLeft(pipeBottom2, 650);
			Canvas.SetLeft(pipeTop3, 900);
			Canvas.SetLeft(pipeBottom3, 900);

			score = 0;
			scoreText.Content = "Score: 0";

			gameOver = false;
			restartButton.Visibility = Visibility.Hidden;

			pipeTop1.Tag = null;
			pipeTop2.Tag = null;
			pipeTop3.Tag = null;

			gameTimer.Start();
		}

		private void KeyIsDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space && !gameOver)
				birdSpeed = -7;
		}
	}
}
