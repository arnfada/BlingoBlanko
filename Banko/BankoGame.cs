using BlinkoBlanko.Common;

namespace BlinkoBlanko.Banko
{
    public class BankoGame
    {
        private Random random;

        public BankoPlates BingoPlates { get; private set; }

        public List<PlateNumber> Numbers { get; private set; }

        public List<int> AvailableNumbers { get; private set; }

        public List<int> DrawnNumbers { get; private set; }

        public BankoGame()
        {
            BingoPlates = new BankoPlates();
            Numbers = new List<PlateNumber>();
            AvailableNumbers = new List<int>();
            DrawnNumbers = new List<int>();
            random = new Random();
        }

        public void Initialize(string? passCode, int? numberOfCards)
        {
            BingoPlates.Generate(passCode, numberOfCards);

            Reset();
        }

        public void Reset()
        {
            random = new Random();

            Numbers.Clear();
            AvailableNumbers.Clear();
            DrawnNumbers.Clear();

            for (int i = 1; i <= 90; i++)
            {
                AvailableNumbers.Add(i);
                Numbers.Add(new PlateNumber(i));
            }
        }

        public void Draw()
        {
            if (AvailableNumbers.Count == 0)
            {
                return;
            }
            int index = random.Next(AvailableNumbers.Count);
            int drawnNumber = AvailableNumbers[index];

            Numbers.First(i => i.Number == drawnNumber).IsDrawn = true;

            AvailableNumbers.Remove(drawnNumber);
            DrawnNumbers.Add(drawnNumber);
        }

        public BankoPlate? GetPlateByNumber(int plateNumber)
        {
            BankoPlate? bingoPlate = BingoPlates.Plates.FirstOrDefault(p => p.PlateNumber == plateNumber);

            if (bingoPlate == null)
            {
                return null;
            }

            bingoPlate.MarkDrawnNumbers(DrawnNumbers);
            return bingoPlate;
        }
    }
}
