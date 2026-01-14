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

			// Csövek mozgatása
			MovePipe(pipeTop1, 2);
			MovePipe(pipeBottom1, 2);
			MovePipe(pipeTop2, 2);
			MovePipe(pipeBottom2, 2);

			// Ütközés ellenőrzés: CSAK ha tényleg átfed a cső kép
			if (CheckCollision(pipeTop1) || CheckCollision(pipeBottom1) ||
				CheckCollision(pipeTop2) || CheckCollision(pipeBottom2))
			{
				EndGame();
			}
		}

		private void MovePipe(Image pipe, double speed)
		{
			double left = Canvas.GetLeft(pipe) - speed;
			if (left < -pipe.Width) left = 800; // újra a jobb oldalra
			Canvas.SetLeft(pipe, left);
		}

		private bool CheckCollision(Image pipe)
		{
			// Madár és cső Image téglalap átfedés
			Rect birdRect = new Rect(Canvas.GetLeft(flappyBird), Canvas.GetTop(flappyBird), flappyBird.Width, flappyBird.Height);
			Rect pipeRect = new Rect(Canvas.GetLeft(pipe), Canvas.GetTop(pipe), pipe.Width, pipe.Height);

			return birdRect.IntersectsWith(pipeRect);
		}

		private void EndGame()
		{
			if (gameOver) return;
			gameOver = true;
			gameTimer.Stop();
			MessageBox.Show("GAME OVER!");
		}

		private void KeyIsDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space && !gameOver) birdSpeed = -7;
		}
	}
}
