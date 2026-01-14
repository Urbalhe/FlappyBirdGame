using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;

namespace FlappyBirdGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DispatcherTimer gameTimer = new DispatcherTimer();

        double score;
        int gravity = 8;
        bool gameOver;
        Rect flappyBirdHitBox;

		public MainWindow()
        {
            InitializeComponent();

            gameTimer.Tick += MainEventTimer;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            StartGame();
		}

		private void MainEventTimer(object? sender, EventArgs e)
		{
			throw new NotImplementedException();  //Show potential fixes-ek jött létre
		}

		private void KeyIsDown(object sender, KeyEventArgs e)
		{

		}

		private void KeyIsUp(object sender, KeyEventArgs e)
		{

		}

        private void StartGame()
        {

        }

        private void EndGame()
        {

        }
	}
}