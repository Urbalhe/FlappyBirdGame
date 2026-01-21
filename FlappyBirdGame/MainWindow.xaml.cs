using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

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

		List<Line> rainDrops = new List<Line>();
		Rectangle fogOverlay = new Rectangle();

		public MainWindow()
		{
			InitializeComponent();
			gameTimer.Interval = TimeSpan.FromMilliseconds(20);
			gameTimer.Tick += GameLoop;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var bgImage = new BitmapImage(new Uri("Images/hatter.gif", UriKind.Relative));
			ImageBehavior.SetAnimatedSource(backgroundGif, bgImage);

			fogOverlay.Width = MyCanvas.ActualWidth;
			fogOverlay.Height = MyCanvas.ActualHeight;
			fogOverlay.Fill = new SolidColorBrush(Color.FromArgb(100, 200, 200, 200));
			fogOverlay.Visibility = Visibility.Hidden;
			Canvas.SetLeft(fogOverlay, 0);
			Canvas.SetTop(fogOverlay, 0);
			Panel.SetZIndex(fogOverlay, 15);
			MyCanvas.Children.Add(fogOverlay);

			UpdateHighScoreText();
		}

		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			MenuGrid.Visibility = Visibility.Hidden;
			MyCanvas.Visibility = Visibility.Visible;
			MyCanvas.Focus();
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

			if (Canvas.GetTop(flappyBird) < 0 ||
				Canvas.GetTop(flappyBird) + flappyBird.Height > MyCanvas.ActualHeight)
			{
				EndGame();
			}

			if (score % 5 == 0 && score != 0) gameSpeed = 5 + score / 5;

			if (score >= 20)
			{
				StartRain();
			}
			if (score >= 30)
			{
				fogOverlay.Visibility = Visibility.Visible;
			}

			AnimateRain();
		}

		private void MovePipe(Image top, Image bottom)
		{
			double left = Canvas.GetLeft(top) - gameSpeed;

			Canvas.SetLeft(top, left);
			Canvas.SetLeft(bottom, left);

			if (!gameOver &&
				left + top.Width < Canvas.GetLeft(flappyBird) &&
				top.Tag?.ToString() != "passed")
			{
				score++;
				scoreText.Content = "Score: " + score;
				top.Tag = "passed";
				UpdateHighScoreText();
			}

			if (left < -top.Width)
			{
				Canvas.SetLeft(top, 800);
				Canvas.SetLeft(bottom, 800);
				top.Tag = null;
			}
		}

		private bool CheckCollision(Image pipe)
		{
			Rect birdRect = new Rect(
				Canvas.GetLeft(flappyBird) + 8,
				Canvas.GetTop(flappyBird) + 8,
				flappyBird.Width - 16,
				flappyBird.Height - 16
			);

			double pipePaddingX = pipe.Width * 0.35;
			double pipePaddingY = 0;

			Rect pipeRect = new Rect(
				Canvas.GetLeft(pipe) + pipePaddingX,
				Canvas.GetTop(pipe) + pipePaddingY,
				pipe.Width - (pipePaddingX * 2),
				pipe.Height
			);

			return birdRect.IntersectsWith(pipeRect);
		}

		private void EndGame()
		{
			if (gameOver) return;

			gameOver = true;
			gameTimer.Stop();
			restartButton.Visibility = Visibility.Visible;

			if (score > highScore) highScore = score;
			UpdateHighScoreText();
		}

		private void RestartButton_Click(object sender, RoutedEventArgs e)
		{
			Canvas.SetTop(flappyBird, 200);
			Canvas.SetLeft(flappyBird, 50);
			birdSpeed = 0;
			birdAngle = 0;
			flappyBird.RenderTransform = new RotateTransform(0, flappyBird.Width / 2, flappyBird.Height / 2);

			Canvas.SetLeft(pipeTop1, 400);
			Canvas.SetLeft(pipeBottom1, 400);
			Canvas.SetLeft(pipeTop2, 650);
			Canvas.SetLeft(pipeBottom2, 650);
			Canvas.SetLeft(pipeTop3, 900);
			Canvas.SetLeft(pipeBottom3, 900);

			score = 0;
			scoreText.Content = "Score: 0";

			pipeTop1.Tag = null;
			pipeTop2.Tag = null;
			pipeTop3.Tag = null;

			gameSpeed = 5;
			rainDrops.Clear();
			fogOverlay.Visibility = Visibility.Hidden;

			List<UIElement> toRemove = new List<UIElement>();
			foreach (UIElement child in MyCanvas.Children)
			{
				if (child != backgroundGif && child != flappyBird && child != pipeTop1 && child != pipeBottom1 &&
					child != pipeTop2 && child != pipeBottom2 && child != pipeTop3 && child != pipeBottom3 &&
					child != scoreText && child != highScoreText && child != restartButton && child != fogOverlay)
				{
					toRemove.Add(child);
				}
			}
			foreach (var child in toRemove)
			{
				MyCanvas.Children.Remove(child);
			}

			gameOver = false;
			restartButton.Visibility = Visibility.Hidden;

			gameTimer.Start();
		}

		private void KeyIsDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space && !gameOver)
			{
				birdSpeed = -7;
				birdAngle = -20;
			}
		}

		private void StartRain()
		{
			if (rainDrops.Count > 0) return;
			for (int i = 0; i < 100; i++)
			{
				Line drop = new Line();
				drop.Stroke = Brushes.LightBlue;
				drop.StrokeThickness = 2;
				drop.X1 = rnd.NextDouble() * MyCanvas.ActualWidth;
				drop.Y1 = rnd.NextDouble() * MyCanvas.ActualHeight;
				drop.X2 = drop.X1;
				drop.Y2 = drop.Y1 + 10;
				Panel.SetZIndex(drop, 12);
				MyCanvas.Children.Add(drop);
				rainDrops.Add(drop);
			}
		}

		private void AnimateRain()
		{
			if (rainDrops.Count == 0) return;
			foreach (var drop in rainDrops)
			{
				drop.Y1 += 5;
				drop.Y2 += 5;
				if (drop.Y1 > MyCanvas.ActualHeight)
				{
					drop.Y1 = 0;
					drop.Y2 = 10;
					drop.X1 = rnd.NextDouble() * MyCanvas.ActualWidth;
					drop.X2 = drop.X1;
				}
			}
		}

		private void UpdateHighScoreText()
		{
			highScoreText.Content = "High Score: " + highScore;
		}
	}
}
