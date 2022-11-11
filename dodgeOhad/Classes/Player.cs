using Windows.UI.Xaml.Controls;

namespace dodgeOhad.Classes
{
    public class Player
    {
        private Canvas _gameBoardCanvas;
        private int _baseMove;
        private Image _playerImage;

        public Player(Image playerImage, Canvas newCanvas)
        {
            PlayerImage = playerImage;
            GameBoardCanvas = newCanvas;
            BaseMove = 15;
        }

        public Image PlayerImage { get => _playerImage; private set => _playerImage = value; }

        public int BaseMove { get => _baseMove; private set => _baseMove = value; }

        public Canvas GameBoardCanvas { get => _gameBoardCanvas; private set => _gameBoardCanvas = value; }

        public void MoveUp()
        {
            if (Canvas.GetTop(PlayerImage) > 0)
            {
                Canvas.SetTop(PlayerImage, Canvas.GetTop(PlayerImage) - BaseMove);
            }
        }

        public void MoveDown()
        {
            if (Canvas.GetTop(PlayerImage) < GameBoardCanvas.ActualHeight - PlayerImage.Height)
            {
                Canvas.SetTop(PlayerImage, Canvas.GetTop(PlayerImage) + BaseMove);
            }
        }

        public void MoveLeft()
        {
            if (Canvas.GetLeft(PlayerImage) > 0)
            {
                Canvas.SetLeft(PlayerImage, Canvas.GetLeft(PlayerImage) - BaseMove);
            }
        }

        public void MoveRight()
        {
            if (Canvas.GetLeft(PlayerImage) < GameBoardCanvas.ActualWidth - PlayerImage.Width)
            {
                Canvas.SetLeft(PlayerImage, Canvas.GetLeft(PlayerImage) + BaseMove);
            }
        }

        public double GetTop()
        {
            return Canvas.GetTop(PlayerImage);
        }

        public double GetLeft()
        {
            return Canvas.GetLeft(PlayerImage);
        }
    }
}
