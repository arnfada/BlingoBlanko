namespace BlinkoBlanko.Bingo
{
    public class BingoGame
    {
        private Random random;
        public BingoPlates BingoPlates { get; private set; }

        public List<BingoPlateNumber> Numbers { get; private set; }

        private List<int> AvailableNumbers { get; set; }
        public List<int> DrawnNumbers { get; set; }

        public BingoGame()
        {
            BingoPlates = new BingoPlates();
            Numbers = new List<BingoPlateNumber>();
            AvailableNumbers = new List<int>();
            DrawnNumbers = new List<int>();
            random = new Random();
        }

        public void Initialize(string passCode, int? numberOfCards)
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
                Numbers.Add(new BingoPlateNumber(i));
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

        public BingoPlate? GetPlateByNumber(int plateNumber)
        {
            BingoPlate? bingoPlate = BingoPlates.Plates.FirstOrDefault(p => p.PlateNumber == plateNumber);

            if (bingoPlate == null)
            {
                return null;
            }

            bingoPlate.MarkDrawnNumbers(DrawnNumbers);
            return bingoPlate;
        }
    }
}
