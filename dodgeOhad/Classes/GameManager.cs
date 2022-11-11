using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace dodgeOhad.Classes
{
    public class GameManager
    {
        private int _simpsonsSpeed;
        private int _bartSpeed;
        private int _giftPicture;
        private Image _gift;
        private bool _isBartHit;
        private List<Simpsons> _simpsons;
        private int _simpsonsTopBoom;
        private int _simpsonsLeftBoom;
        private const double SIMPSONS_SIZE = 60;
        private bool _goLeft;
        private bool _goRight;
        private bool _goUp;
        private bool _goDown;
        private Bart _bart;
        private bool _isGiftOn;
        private Canvas _playgroundCanvas;

        private DispatcherTimer _simpsonsMovementTmr;
        private DispatcherTimer _bartMovementTmr;
        private DispatcherTimer _tmrCollision;
        private DispatcherTimer _returnBartToMainPicture;
        private DispatcherTimer _createGifts;
        private DispatcherTimer _speedGift;
        private DispatcherTimer _removeStarsTmr;
        private DispatcherTimer _speedingSimpsonsTmr;

        private MusicManager _musicManager;

        private List<Image> _collisionImgList;
        private Button _btnLoadGame;
        private Button _btnNewGame;

        public int SimpsonsSpeed { get => _simpsonsSpeed; set => _simpsonsSpeed = value; }
        public int BartSpeed { get => _bartSpeed; set => _bartSpeed = value; }
        public List<Simpsons> Simpsons { get => _simpsons; set => _simpsons = value; }
        public Bart Bart { get => _bart; set => _bart = value; }

        public GameManager(Canvas playgound, int amountOfLifes, Button btnLoadGame, Button btnNewGame, MusicManager musicManager)
        {
            _collisionImgList = new List<Image>();
            _simpsons = new List<Simpsons>();
            _simpsonsSpeed = 140;
            _bartSpeed = 50;
            _musicManager = musicManager;
            _musicManager.PlayBackgroundMusic();
            this._btnLoadGame = btnLoadGame;
            this._btnNewGame = btnNewGame;
            _playgroundCanvas = playgound;
            createBart(amountOfLifes, 500, 700);
            InitializeKeysListener();
            createSimpsons();
            createTimers();
        }

        private void InitializeKeysListener()
        {
            Window.Current.CoreWindow.KeyDown += KeyIsDownEvent;
            Window.Current.CoreWindow.KeyUp += KeyIsUpEvent;
        }

        public GameManager(Canvas canvasPlayingArea, List<PlayerModel> loadedPlayers, Button btnLoadGame, Button btnNewGame, MusicManager musicManager)
        {
            _collisionImgList = new List<Image>();
            _simpsons = new List<Simpsons>();
            BartSpeed = loadedPlayers[0].PlayerSpeed;
            SimpsonsSpeed = loadedPlayers[1].PlayerSpeed;
            _musicManager = musicManager;
            this._btnLoadGame = btnLoadGame;
            this._btnNewGame = btnNewGame;
            _musicManager.PlayBackgroundMusic();
            _playgroundCanvas = canvasPlayingArea;
            createBart(loadedPlayers[0].AmountOfLifes, loadedPlayers[0].PlayerXPosition, loadedPlayers[0].PlayerYPosition);
            InitializeKeysListener();
            createSimpsons(loadedPlayers);
            createTimers();
        }

        private void createBart(int amountOfLifes, double playerXPosition, double playerYPosition)
        {
            Image PlayerImage = new Image();
            PlayerImage.Source = new BitmapImage(new Uri("ms-appx:///pictures/Bart.png"));
            _bart = new Bart(PlayerImage, _playgroundCanvas, amountOfLifes);
            Canvas.SetLeft(PlayerImage, playerXPosition);
            Canvas.SetTop(PlayerImage, playerYPosition);
            PlayerImage.Width = 90;
            PlayerImage.Height = 90;
            _playgroundCanvas.Children.Add(PlayerImage);
        }

        private void createTimers()
        {
            _simpsonsMovementTmr = new DispatcherTimer();
            _bartMovementTmr = new DispatcherTimer();
            _tmrCollision = new DispatcherTimer();
            _returnBartToMainPicture = new DispatcherTimer();
            _createGifts = new DispatcherTimer();
            _speedGift = new DispatcherTimer();
            _removeStarsTmr = new DispatcherTimer();
            _speedingSimpsonsTmr = new DispatcherTimer();

            _bartMovementTmr.Interval = new TimeSpan(0, 0, 0, 0, _bartSpeed);
            _bartMovementTmr.Tick += BartMove;

            _removeStarsTmr.Interval = new TimeSpan(0, 0, 0, 1, 700);
            _removeStarsTmr.Tick += removeStars;

            _simpsonsMovementTmr.Interval = new TimeSpan(0, 0, 0, 0, _simpsonsSpeed);
            _simpsonsMovementTmr.Tick += SimpsonsMove;
            _simpsonsMovementTmr.Tick += SimpsonsCollision;
            _simpsonsMovementTmr.Tick += BartCollision;

            _bartMovementTmr.Start();
            _simpsonsMovementTmr.Start();

            _speedingSimpsonsTmr.Interval = new TimeSpan(0, 0, 0, 4, 500);
            _speedingSimpsonsTmr.Tick += speedingSimpsonsMovement;
            _speedingSimpsonsTmr.Start();

            _createGifts.Interval = new TimeSpan(0, 0, 0, 3, 0);
            _createGifts.Tick += createGift;
            _createGifts.Start();

            _speedGift.Interval = new TimeSpan(0, 0, 0, 4, 0);
            _speedGift.Tick += bringBartToNormalSpeed;
            _speedGift.Start();
        }

        private void createSimpsons(List<PlayerModel> loadedPlayers)
        {
            Random rand = new Random();
            for (int i = 1; i < loadedPlayers.Count; i++)
            {
                Image PlayerImage = new Image();
                PlayerImage.Width = SIMPSONS_SIZE;
                PlayerImage.Height = SIMPSONS_SIZE;
                int character = rand.Next(1, 22);
                PlayerImage.Source = new BitmapImage(new Uri($"ms-appx:///pictures/enemies/{character}.png"));
                _simpsons.Add(new Simpsons(PlayerImage, _playgroundCanvas));
                Canvas.SetLeft(PlayerImage, loadedPlayers[i].PlayerXPosition);
                Canvas.SetTop(PlayerImage, loadedPlayers[i].PlayerYPosition);
                _playgroundCanvas.Children.Add(PlayerImage);
            }
        }

        private void KeyIsDownEvent(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case Windows.System.VirtualKey.Left:
                    _goLeft = true;
                    break;
                case Windows.System.VirtualKey.Right:
                    _goRight = true;
                    break;
                case Windows.System.VirtualKey.Up:
                    _goUp = true;
                    break;
                case Windows.System.VirtualKey.Down:
                    _goDown = true;
                    break;
            }
        }

        private void KeyIsUpEvent(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case Windows.System.VirtualKey.Left:
                    _goLeft = false;
                    break;
                case Windows.System.VirtualKey.Right:
                    _goRight = false;
                    break;
                case Windows.System.VirtualKey.Up:
                    _goUp = false;
                    break;
                case Windows.System.VirtualKey.Down:
                    _goDown = false;
                    break;
            }
        }
        public void BartMove(object sender, object Args)
        {
            if (_goLeft)
            {
                _bart.MoveLeft();
            }
            if (_goRight)
            {
                _bart.MoveRight();
            }
            if (_goUp)
            {
                _bart.MoveUp();
            }
            if (_goDown)
            {
                _bart.MoveDown();
            }
        }

        public void BartMove(CoreWindow Core, KeyEventArgs Args)
        {
            switch (Args.VirtualKey)
            {
                case Windows.System.VirtualKey.Left:
                    _bart.MoveLeft();
                    break;
                case Windows.System.VirtualKey.Right:
                    _bart.MoveRight();
                    break;
                case Windows.System.VirtualKey.Up:
                    _bart.MoveUp();
                    break;
                case Windows.System.VirtualKey.Down:
                    _bart.MoveDown();
                    break;
            }
        }

        private void createSimpsons()
        {
            Random rand = new Random();
            for (int i = 1; i < 11; i++)
            {
                Image PlayerImage = new Image();
                PlayerImage.Width = SIMPSONS_SIZE;
                PlayerImage.Height = SIMPSONS_SIZE;
                int character = rand.Next(0, 21);
                PlayerImage.Source = new BitmapImage(new Uri($"ms-appx:///pictures/enemies/{character}.png"));
                _simpsons.Add(new Simpsons(PlayerImage, _playgroundCanvas));
                Canvas.SetLeft(PlayerImage, rand.Next(-200, 1400));
                Canvas.SetTop(PlayerImage, rand.Next(-200, 250));
                _playgroundCanvas.Children.Add(PlayerImage);
            }
        }

        private void SimpsonsMove(object sender, object Args)
        {
            for (int i = 0; i < _simpsons.Count; i++)
            {
                if (_bart.GetTop() < _simpsons[i].GetTop())
                {
                    _simpsons[i].MoveUp();
                }
                if (_bart.GetTop() > _simpsons[i].GetTop())
                {
                    _simpsons[i].MoveDown();
                }
                if (_bart.GetLeft() < _simpsons[i].GetLeft())
                {
                    _simpsons[i].MoveLeft();
                }
                if (_bart.GetLeft() > _simpsons[i].GetLeft())
                {
                    _simpsons[i].MoveRight();
                }
            }
        }

        private void SimpsonsCollision(object sender, object Args)
        {
            for (int i = 0; i < _simpsons.Count; i++)
            {
                for (int j = i + 1; j < _simpsons.Count; j++)
                {
                    if (_simpsons[i].GetTop() > _simpsons[j].GetTop() - (SIMPSONS_SIZE / 2)
                        && _simpsons[i].GetTop() < _simpsons[j].GetTop() + (SIMPSONS_SIZE / 2)
                        && _simpsons[i].GetLeft() > _simpsons[j].GetLeft() - (SIMPSONS_SIZE / 2)
                        && _simpsons[i].GetLeft() < _simpsons[j].GetLeft() + (SIMPSONS_SIZE / 2))
                    {
                        _simpsonsTopBoom = (int)_simpsons[i].GetTop();
                        _simpsonsLeftBoom = (int)_simpsons[i].GetLeft();
                        _playgroundCanvas.Children.Remove(_simpsons[i].PlayerImage);
                        _simpsons.RemoveAt(i);
                        _musicManager.PlayCollisionSound();
                        createCollisionBoom();
                    }
                }
            }
            if (_simpsons.Count == 1)
            {
                BartWin();
            }
        }

        private void BartCollision(object sender, object Args)
        {
            if (!_isBartHit)
            {
                for (int i = 0; i < _simpsons.Count; i++)
                {
                    if ((_bart.GetTop() > _simpsons[i].GetTop() - SIMPSONS_SIZE && _bart.GetTop() < _simpsons[i].GetTop() + SIMPSONS_SIZE)
                        && _bart.GetLeft() > _simpsons[i].GetLeft() - SIMPSONS_SIZE && _bart.GetLeft() < _simpsons[i].GetLeft() + SIMPSONS_SIZE)
                    {
                        bartGotHit();
                        break;
                    }
                }
            }
        }

        private void bartGotHit()
        {
            _musicManager.PlayBartHitSound();
            TextBlock textBlock = _playgroundCanvas.Children.ElementAt(1) as TextBlock;
            _bart.AmountOfLifes--;
            textBlock.Text = _bart.AmountOfLifes.ToString();
            _isBartHit = true;
            _bart.PlayerImage.Source = new BitmapImage(new Uri("ms-appx:///pictures/BartGotHit.gif"));
            _returnBartToMainPicture.Interval = new TimeSpan(0, 0, 0, 1, 0);
            _returnBartToMainPicture.Tick += bartFirstPicture;
            _returnBartToMainPicture.Start();
            if (_bart.AmountOfLifes == 0)
            {
                bartIsDead();
            }
        }

        private void bartFirstPicture(object sender, object e)
        {
            _bart.PlayerImage.Source = new BitmapImage(new Uri("ms-appx:///pictures/Bart.png"));
            _returnBartToMainPicture.Stop();
            _isBartHit = false;
        }

        private void createCollisionBoom()
        {
            Image CollisionImage = new Image();
            CollisionImage.Source = new BitmapImage(new Uri("ms-appx:///pictures/Boom4.gif"));
            CollisionImage.Height = 100;
            CollisionImage.Width = 100;
            Canvas.SetTop(CollisionImage, _simpsonsTopBoom);
            Canvas.SetLeft(CollisionImage, _simpsonsLeftBoom);
            _playgroundCanvas.Children.Add(CollisionImage);
            _collisionImgList.Add(CollisionImage);
            _tmrCollision.Interval = new TimeSpan(0, 0, 0, 0, 1200);
            _tmrCollision.Tick += OnTickHandlerExplosion;
            _tmrCollision.Start();
        }
        private void OnTickHandlerExplosion(object sender, object e)
        {
            foreach (var item in _collisionImgList)
            {
                _playgroundCanvas.Children.Remove(item);
            }
            _tmrCollision.Stop();
        }

        private void bartIsDead()
        {
            removePlayers();
            _musicManager.StopBgMusic();
            _musicManager.PlayGameOverMusic();
            _simpsonsMovementTmr.Stop();
            Window.Current.CoreWindow.KeyDown -= KeyIsDownEvent;
            Window.Current.CoreWindow.KeyUp -= KeyIsUpEvent;
            _bartMovementTmr.Stop();
            _playgroundCanvas.Children.Remove(_bart.PlayerImage);
            generateGameOvernWindow();
        }

        private void generateGameOvernWindow()
        {
            Image bartCry = new Image();
            bartCry.Source = new BitmapImage(new Uri("ms-appx:///pictures/BartCry2.gif"));
            bartCry.Width = 700;
            bartCry.Height = 700;
            Canvas.SetLeft(bartCry, 750);
            Canvas.SetTop(bartCry, 150);
            _playgroundCanvas.Children.Add(bartCry);

            Image gameOver = new Image();
            gameOver.Source = new BitmapImage(new Uri("ms-appx:///pictures/GameOver.gif"));
            gameOver.Width = 1200;
            gameOver.Height = 300;
            Canvas.SetLeft(gameOver, 505);
            Canvas.SetTop(gameOver, 20);
            _playgroundCanvas.Children.Add(gameOver);
            addButtons();
        }
        
        private void BartWin()
        {
            removePlayers();
            _musicManager.StopBgMusic();
            _musicManager.PlayBartWinMusic();
            _simpsonsMovementTmr.Stop();
            Window.Current.CoreWindow.KeyDown -= KeyIsDownEvent;
            Window.Current.CoreWindow.KeyUp -= KeyIsUpEvent;
            _bartMovementTmr.Stop();
            generateYouWinWindow();
        }

        private void generateYouWinWindow()
        {
            Image bartWin = new Image();
            bartWin.Source = new BitmapImage(new Uri("ms-appx:///pictures/bartWin.gif"));
            bartWin.Width = 700;
            bartWin.Height = 700;
            Canvas.SetLeft(bartWin, 750);
            Canvas.SetTop(bartWin, 150);
            _playgroundCanvas.Children.Add(bartWin);

            Image rightCoins = new Image();
            rightCoins.Source = new BitmapImage(new Uri("ms-appx:///pictures/Coins.gif"));
            rightCoins.Width = 200;
            rightCoins.Height = 700;
            Canvas.SetLeft(rightCoins, 1470);
            Canvas.SetTop(rightCoins, 120);
            _playgroundCanvas.Children.Add(rightCoins);

            Image leftCoins = new Image();
            leftCoins.Source = new BitmapImage(new Uri("ms-appx:///pictures/Coins.gif"));
            leftCoins.Width = 200;
            leftCoins.Height = 700;
            Canvas.SetLeft(leftCoins, 530);
            Canvas.SetTop(leftCoins, 120);
            _playgroundCanvas.Children.Add(leftCoins);

            Image bigWin = new Image();
            bigWin.Source = new BitmapImage(new Uri("ms-appx:///pictures/BigWin.gif"));
            bigWin.Width = 700;
            bigWin.Height = 200;
            Canvas.SetLeft(bigWin, 750);
            Canvas.SetTop(bigWin, 50);
            _playgroundCanvas.Children.Add(bigWin);
            addButtons();
        }

        private void removePlayers()
        {
            _createGifts.Stop();
            _playgroundCanvas.Children.Remove(_bart.PlayerImage);
            for (int i = 0; i < _simpsons.Count; i++)
            {
                _playgroundCanvas.Children.Remove(_simpsons[i].PlayerImage);
            }
            if (_isGiftOn)
            {
                _playgroundCanvas.Children.Remove(_gift);
            }
        }

        private void createGift(object sender, object Args)
        {
            Random rand = new Random();
            int currentWidnowHeight = (int)_playgroundCanvas.ActualHeight;
            int currentWidnowWidth = (int)_playgroundCanvas.ActualWidth;
            _gift = new Image();
            _giftPicture = rand.Next(1, 3);
            _gift.Source = new BitmapImage(new Uri($"ms-appx:///pictures/gifts/gift{_giftPicture}.gif"));
            _gift.Width = 65;
            _gift.Height = 65;
            Canvas.SetLeft(_gift, rand.Next(100, currentWidnowWidth - 70));
            Canvas.SetTop(_gift, rand.Next(100, currentWidnowHeight - 70));
            _playgroundCanvas.Children.Add(_gift);
            _isGiftOn = true;
            _bartMovementTmr.Tick += bartTakeGift;
            _createGifts.Stop();
        }

        private void bartTakeGift(object sender, object Args)
        {
            TextBlock textBlock = _playgroundCanvas.Children.ElementAt(1) as TextBlock;

            if ((_bart.GetTop() > Canvas.GetTop(_gift) - _gift.Height && _bart.GetTop() < Canvas.GetTop(_gift) + _gift.Height)
                && _bart.GetLeft() > Canvas.GetLeft(_gift) - _gift.Width && _bart.GetLeft() < Canvas.GetLeft(_gift) + _gift.Width && _isGiftOn)
            {
                createMagicStars();

                _isGiftOn = false;
                _createGifts.Start();

                if (_giftPicture == 1)
                {
                    _bart.AmountOfLifes++;
                    textBlock.Text = _bart.AmountOfLifes.ToString();
                    _musicManager.BartLaugh();
                }
                else
                {
                    _bartSpeed = 5;
                    _musicManager.PlayBartMan();
                    _bartMovementTmr.Interval = new TimeSpan(0, 0, 0, 0, _bartSpeed);
                    _speedGift.Start();
                }
            }
        }

        private void bringBartToNormalSpeed(object sender, object Args)
        {
            _bartSpeed = 50;
            _bartMovementTmr.Interval = new TimeSpan(0, 0, 0, 0, _bartSpeed);
            _speedGift.Stop();
        }

        private void speedingSimpsonsMovement(object sender, object Args)
        {
            _simpsonsSpeed -= 25;
            _simpsonsMovementTmr.Interval = new TimeSpan(0, 0, 0, 0, _simpsonsSpeed);
        }

        private void createMagicStars()
        {
            _gift.Width = 300;
            _gift.Height = 300;
            Canvas.SetTop(_gift, Canvas.GetTop(_gift) - 110);
            Canvas.SetLeft(_gift, Canvas.GetLeft(_gift) - 110);
            _gift.Source = new BitmapImage(new Uri("ms-appx:///pictures/Magic.gif"));
            _removeStarsTmr.Start();
        }

        private void removeStars(object sender, object Args)
        {
            _playgroundCanvas.Children.Remove(_gift);
            _removeStarsTmr.Stop();
        }

        public void Pause()
        {
            _bartMovementTmr.Stop();
            _simpsonsMovementTmr.Stop();
            _musicManager.PauseBgMusic();
            if (!_isGiftOn)
            {
                _createGifts.Stop();
            }
        }

        public void Resume()
        {
            _bartMovementTmr.Start();
            _simpsonsMovementTmr.Start();
            _musicManager.ResmueBgMusic();
            if (!_isGiftOn)
            {
                _createGifts.Start();
            }
        }

        private void addButtons()
        {
            Image homerImage = new Image();
            homerImage.Source = new BitmapImage(new Uri("ms-appx:///pictures/HomerButton.png"));
            homerImage.Width = 200;
            homerImage.Height = 400;
            Canvas.SetLeft(homerImage, 170);
            Canvas.SetTop(homerImage, 500);
            _playgroundCanvas.Children.Add(homerImage);

            Image lisaImage = new Image();
            lisaImage.Source = new BitmapImage(new Uri("ms-appx:///pictures/LisarButton.png"));
            lisaImage.Width = 160;
            lisaImage.Height = 320;
            Canvas.SetLeft(lisaImage, 75);
            Canvas.SetTop(lisaImage, 230);
            _playgroundCanvas.Children.Add(lisaImage);

            Canvas.SetLeft(_btnNewGame, 230);
            Canvas.SetTop(_btnNewGame, 250);
            Canvas.SetLeft(_btnLoadGame, 30);
            Canvas.SetTop(_btnLoadGame, 700);
            _playgroundCanvas.Children.Add(_btnNewGame);
            _playgroundCanvas.Children.Add(_btnLoadGame);
        }
    }
}
