using dodgeOhad.Classes;
using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Image = Windows.UI.Xaml.Controls.Image;

namespace dodgeOhad
{
    public sealed partial class MainPage : Page
    {
        private GameManager _gameManager;
        private MusicManager _musicManager;
        private bool PauseGame = false;
        private int _amountOfLifes;

        public MainPage()
        {
            this.InitializeComponent();
            _musicManager = new MusicManager();
        }

        public void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _musicManager.PlayIntroMusic();
        }

        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            CanvasPlayingArea.Children.Clear();
            InitializeNewGame();
            _gameManager = new GameManager(CanvasPlayingArea, _amountOfLifes, btnLoadGame, btnNewGame, _musicManager);
        }

        private void InitializeNewGame()
        {
            _musicManager.StopEnindgMusic();
            changeImageBackground();
            addBart_LivesImageToCorner();
            addLivesNumberToCorner();

            CanvasPlayingArea.Children.Remove(btnNewGame);
            CanvasPlayingArea.Children.Remove(btnLoadGame);

            _musicManager.StopIntroMusic();
        }

        private async void btnLoadGame_Click(object sender, RoutedEventArgs e)
        {
            _musicManager.StopEnindgMusic();
            CanvasPlayingArea.Children.Clear();
            List<PlayerModel> loadedPlayers = await FileController.LoadFromFile();
            if (loadedPlayers.Count == 1)
            {
                MessageDialog messageDialog = new MessageDialog("There was an error loading the game");
                await messageDialog.ShowAsync();
            }

            if (loadedPlayers is null)
            {
                MessageDialog messageDialog = new MessageDialog("Error- There is no saved game");
                await messageDialog.ShowAsync();
            }

            InitializeNewGame();
            _gameManager = new GameManager(CanvasPlayingArea, loadedPlayers, btnLoadGame, btnNewGame, _musicManager);
        }

        private void addBart_LivesImageToCorner()
        {
            Image bartImage = new Image();
            bartImage.Source = new BitmapImage(new Uri("ms-appx:///pictures/Bart-lives.gif"));

            bartImage.Width = 80;
            bartImage.Height = 80;

            Canvas.SetLeft(bartImage, 0);
            Canvas.SetTop(bartImage, 20);
            CanvasPlayingArea.Children.Add(bartImage);
        }

        private void addLivesNumberToCorner()
        {
            TextBlock txtLifesLeft = new TextBlock();
            txtLifesLeft.Text = "3";
            txtLifesLeft.Name = "txtLifesLeft";
            txtLifesLeft.FontSize = 40;
            txtLifesLeft.Foreground = new SolidColorBrush(Colors.Yellow);

            Canvas.SetLeft(txtLifesLeft, 40);
            Canvas.SetTop(txtLifesLeft, 110);

            CanvasPlayingArea.Children.Add(txtLifesLeft);
            _amountOfLifes = int.Parse(txtLifesLeft.Text);
        }

        private void changeImageBackground()
        {
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///pictures/backgrounds/Background.jpg"));
            CanvasPlayingArea.Background = imageBrush;
        }

        private List<PlayerModel> getAllPlayers(int bartSpeed, int simpsonsSpeed)
        {
            List<PlayerModel> allPlayers = new List<PlayerModel>();
            allPlayers.Add(new PlayerModel
            {
                PlayerXPosition = Canvas.GetLeft(_gameManager.Bart.PlayerImage),
                PlayerYPosition = Canvas.GetTop(_gameManager.Bart.PlayerImage),
                AmountOfLifes = _gameManager.Bart.AmountOfLifes,
                PlayerSpeed = bartSpeed
            });

            for (int i = 0; i < _gameManager.Simpsons.Count; i++)
            {
                allPlayers.Add(new PlayerModel
                {
                    PlayerXPosition = Canvas.GetLeft(_gameManager.Simpsons[i].PlayerImage),
                    PlayerYPosition = Canvas.GetTop(_gameManager.Simpsons[i].PlayerImage),
                    PlayerSpeed = simpsonsSpeed
                });
            }

            return allPlayers;
        }

        private void pauseGameButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (!PauseGame)
            {
                _gameManager.Pause();
                PauseGame = true;
            }
            else
            {
                _gameManager.Resume();
                PauseGame = false;
            }
        }

        private void saveGameButton_Clicked(object sender, RoutedEventArgs e)
        {
            List<PlayerModel> allPlayers = getAllPlayers(_gameManager.BartSpeed, _gameManager.SimpsonsSpeed);
            FileController.SaveToFile(allPlayers);
        }
    }
}
