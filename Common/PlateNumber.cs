namespace BlinkoBlanko.Common
{
    public class PlateNumber
    {
        public PlateNumber(int number)
        {
            Number = number;
        }
        public int Number { get; set; }
        public bool IsDrawn { get; set; } = false;
    }
}
