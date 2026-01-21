using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;
using WpfAnimatedGif;
using System;
using System.Windows.Media.Imaging;

namespace FlappyBirdGame
{
	public partial class MainWindow : Window
	{
		DispatcherTimer gameTimer = new DispatcherTimer();

		double gravity = 1.5;
		double birdSpeed = 0;
		double gameSpeed = 5;

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


			// GIF betöltése háttérként
			var image = new BitmapImage(new Uri("Images/hatter.gif", UriKind.Relative));
			ImageBehavior.SetAnimatedSource(backgroundGif, image);
		}

		private void GameLoop(object sender, EventArgs e)
		{
			if (gameOver) return;


			Canvas.SetTop(flappyBird, Canvas.GetTop(flappyBird) + birdSpeed);
			birdSpeed += gravity;


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
			MessageBox.Show("GAME OVER!");
		}

		private void RestartButton_Click(object sender, RoutedEventArgs e)
		{

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

			pipeTop1.Tag = null;
			pipeTop2.Tag = null;
			pipeTop3.Tag = null;

			gameOver = false;
			restartButton.Visibility = Visibility.Hidden;
			gameTimer.Start();
		}

		private void KeyIsDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space && !gameOver)
			{
				birdSpeed = -7;
			}
		}







	}
}
