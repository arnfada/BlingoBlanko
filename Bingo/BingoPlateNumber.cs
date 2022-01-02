namespace BlinkoBlanko.Bingo
{
    public class BingoPlateNumber
    {
        public BingoPlateNumber(int number)
        {
            Number = number;
        }
        public int Number { get; set; }
        public bool IsDrawn { get; set; } = false;
    }
}
