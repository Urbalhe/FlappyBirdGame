using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace FlappyBirdGame
{
	public partial class MainWindow : Window
	{
		DispatcherTimer gameTimer = new DispatcherTimer();
		Random rnd = new Random();

		double gravity = 1.5;
		double birdSpeed = 0;
		double gameSpeed = 5;
		double birdAngle = 0;

		int score = 0;
		int highScore = 0;
		bool gameOver = false;

		double pipeSpacing = 250;

		List<Line> rainDrops = new List<Line>();
		Rectangle fogOverlay = new Rectangle();

		string selectedBird = "Images/flappyBird.png";

		public MainWindow()
		{
			InitializeComponent();
			gameTimer.Interval = TimeSpan.FromMilliseconds(20);
			gameTimer.Tick += GameLoop;
		}

		private void Bird1Button_Click(object sender, RoutedEventArgs e)
		{
			selectedBird = "Images/flappyBird.png";
		}

		private void Bird2Button_Click(object sender, RoutedEventArgs e)
		{
			selectedBird = "Images/madar.png";
		}

		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			StartGame();
		}

		private void StartGame()
		{
			flappyBird.Source = new BitmapImage(new Uri(selectedBird, UriKind.Relative));

			MenuGrid.Visibility = Visibility.Hidden;
			MyCanvas.Visibility = Visibility.Visible;
			gameOver = false;

			StopRain();
			fogOverlay.Visibility = Visibility.Hidden;
			gameSpeed = 5;

			Canvas.SetTop(flappyBird, 200);
			Canvas.SetLeft(flappyBird, 50);
			birdSpeed = 0;
			birdAngle = 0;
			flappyBird.RenderTransform = new RotateTransform(0, flappyBird.Width / 2, flappyBird.Height / 2);

			Canvas.SetLeft(pipeTop1, 500);
			Canvas.SetLeft(pipeBottom1, 500);
			Canvas.SetLeft(pipeTop2, 500 + pipeSpacing);
			Canvas.SetLeft(pipeBottom2, 500 + pipeSpacing);
			Canvas.SetLeft(pipeTop3, 500 + pipeSpacing * 2);
			Canvas.SetLeft(pipeBottom3, 500 + pipeSpacing * 2);

			score = 0;
			scoreText.Content = "Pont: 0";

			pipeTop1.Tag = null;
			pipeTop2.Tag = null;
			pipeTop3.Tag = null;

			restartButton.Visibility = Visibility.Hidden;
			BackToMenuButton.Visibility = Visibility.Hidden;

			gameTimer.Start();
		}

		private void GameLoop(object sender, EventArgs e)
		{
			if (gameOver) return;

			Canvas.SetTop(flappyBird, Canvas.GetTop(flappyBird) + birdSpeed);
			birdSpeed += gravity;

			birdAngle += (birdSpeed > 0 ? 1 : -1);
			if (birdAngle > 25) birdAngle = 25;
			if (birdAngle < -25) birdAngle = -25;
			flappyBird.RenderTransform = new RotateTransform(birdAngle, flappyBird.Width / 2, flappyBird.Height / 2);

			MovePipe(pipeTop1, pipeBottom1);
			MovePipe(pipeTop2, pipeBottom2);
			MovePipe(pipeTop3, pipeBottom3);

			if (CheckCollision(pipeTop1) || CheckCollision(pipeBottom1) ||
				CheckCollision(pipeTop2) || CheckCollision(pipeBottom2) ||
				CheckCollision(pipeTop3) || CheckCollision(pipeBottom3))
			{
				EndGame();
			}

			if (Canvas.GetTop(flappyBird) < 0 || Canvas.GetTop(flappyBird) + flappyBird.Height > MyCanvas.ActualHeight)
			{
				EndGame();
			}

			gameSpeed = 5 + score / 5;
		}

		private void MovePipe(Image top, Image bottom)
		{
			double left = Canvas.GetLeft(top) - gameSpeed;
			Canvas.SetLeft(top, left);
			Canvas.SetLeft(bottom, left);

			if (!gameOver && left + top.Width < Canvas.GetLeft(flappyBird) && top.Tag?.ToString() != "passed")
			{
				score++;
				scoreText.Content = "Pont: " + score;
				top.Tag = "passed";
				UpdateHighScoreText();
			}

			if (left < -top.Width)
			{
				double maxX = Math.Max(Canvas.GetLeft(pipeTop1), Math.Max(Canvas.GetLeft(pipeTop2), Canvas.GetLeft(pipeTop3)));
				Canvas.SetLeft(top, maxX + pipeSpacing);
				Canvas.SetLeft(bottom, maxX + pipeSpacing);
				top.Tag = null;
			}
		}

		private bool CheckCollision(Image pipe)
		{
			Rect birdRect = new Rect(Canvas.GetLeft(flappyBird) + 8, Canvas.GetTop(flappyBird) + 8, flappyBird.Width - 16, flappyBird.Height - 16);
			double pad = pipe.Width * 0.35;
			Rect pipeRect = new Rect(Canvas.GetLeft(pipe) + pad, Canvas.GetTop(pipe), pipe.Width - pad * 2, pipe.Height);
			return birdRect.IntersectsWith(pipeRect);
		}

		private void EndGame()
		{
			if (gameOver) return;
			gameOver = true;
			gameTimer.Stop();
			restartButton.Visibility = Visibility.Visible;
			BackToMenuButton.Visibility = Visibility.Visible;
			if (score > highScore) highScore = score;
			UpdateHighScoreText();
		}

		private void RestartButton_Click(object sender, RoutedEventArgs e)
		{
			StartGame();
		}

		private void BackToMenuButton_Click(object sender, RoutedEventArgs e)
		{
			gameTimer.Stop();
			MyCanvas.Visibility = Visibility.Hidden;
			MenuGrid.Visibility = Visibility.Visible;
		}

		private void KeyIsDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space && !gameOver)
			{
				birdSpeed = -7;
				birdAngle = -20;
			}
		}

		private void UpdateHighScoreText()
		{
			highScoreText.Content = "Rekord: " + highScore;
		}

		private void StopRain() { foreach (var drop in rainDrops) MyCanvas.Children.Remove(drop); rainDrops.Clear(); }
	}
}
