using BlinkoBlanko.Common;

namespace BlinkoBlanko.Banko
{
    public class BankoGame
    {
        private Random random;

        public BankoPlates BankoPlates { get; private set; }

        public List<PlateNumber> Numbers { get; private set; }

        public List<int> AvailableNumbers { get; private set; }

        public List<int> DrawnNumbers { get; private set; }

        public List<BankoPlate> WinnersBanko { get; private set; }

        public List<BankoPlate> WinnersTwoRows { get; private set; }

        public List<BankoPlate> WinnersOneRows { get; private set; }

        public BankoGame()
        {
            BankoPlates = new BankoPlates();
            Numbers = new List<PlateNumber>();
            AvailableNumbers = new List<int>();
            DrawnNumbers = new List<int>();
            random = new Random();
            WinnersBanko = new List<BankoPlate>();
            WinnersTwoRows = new List<BankoPlate>();
            WinnersOneRows = new List<BankoPlate>();
        }

        public void Initialize(string? passCode, int? numberOfCards)
        {
            BankoPlates.Generate(passCode, numberOfCards);

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

            WinnersBanko.Clear();
            WinnersTwoRows.Clear();
            WinnersOneRows.Clear();

            BankoPlates.ResetDrawnNumbers();
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

            // Mark drawn number on every card and check if they have winnings
            foreach (var plate in BankoPlates.Plates)
            {
                bool numberMatched = plate.MarkDrawnNumber(drawnNumber);
                if (!numberMatched)
                {
                    continue;
                }

                if (plate.HasBanko)
                {
                    if (!WinnersBanko.Contains(plate))
                    {
                        WinnersBanko.Add(plate);
                        WinnersTwoRows.Remove(plate);
                    }
                }
                else if (plate.HasTwoRows)
                {
                    if (!WinnersTwoRows.Contains(plate))
                    {
                        WinnersTwoRows.Add(plate);
                        WinnersOneRows.Remove(plate);
                    }
                }
                else if (plate.HasOneRow)
                {
                    if (!WinnersOneRows.Contains(plate))
                    {
                        WinnersOneRows.Add(plate);
                    }
                }
            }
        }

        public BankoPlate? GetPlateByNumber(int plateNumber)
        {
            BankoPlate? bingoPlate = BankoPlates.Plates.FirstOrDefault(p => p.PlateNumber == plateNumber);

            if (bingoPlate == null)
            {
                return null;
            }
            return bingoPlate;
        }
    }
}
