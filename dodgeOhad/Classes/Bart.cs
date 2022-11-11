using Windows.UI.Xaml.Controls;

namespace dodgeOhad.Classes
{
    public class Bart : Player
    {
        private int _amountOfLifes;
        public int AmountOfLifes { get => _amountOfLifes; set => _amountOfLifes = value; }
        public Bart(Image playerImage, Canvas newCanvas, int amountOfLifes) : base(playerImage, newCanvas)
        {
            AmountOfLifes = amountOfLifes;
        }
    }
}
