using Blazored.LocalStorage;
using BlinkoBlanko.Common;

namespace BlinkoBlanko.Banko
{
    public class BankoGame
    {
        private Random random;
        private string localStorageKey;
        private ILocalStorageService? localStorage;

        public BankoPlates BankoPlates { get; private set; }

        public List<PlateNumber> Numbers { get; private set; }

        public List<int> AvailableNumbers { get; private set; }

        public List<int> DrawnNumbers { get; private set; }

        public List<BankoPlate> WinnersBanko { get; private set; }

        public List<BankoPlate> WinnersTwoRows { get; private set; }

        public List<BankoPlate> WinnersOneRows { get; private set; }

        public string PassCode { get; private set; }

        public BankoGame()
        {
            PassCode = string.Empty;
            BankoPlates = new BankoPlates();
            Numbers = new List<PlateNumber>();
            AvailableNumbers = new List<int>();
            DrawnNumbers = new List<int>();
            random = new Random();
            WinnersBanko = new List<BankoPlate>();
            WinnersTwoRows = new List<BankoPlate>();
            WinnersOneRows = new List<BankoPlate>();
            localStorageKey = string.Empty;
        }

        public async Task Initialize(string? passCode, int? numberOfCards, ILocalStorageService localStorage)
        {
            PassCode = passCode ?? "1234";
            this.localStorage = localStorage;
            BankoPlates.Generate(passCode, numberOfCards);
            Reset();

            localStorageKey = $"DrawnNumbers_{PassCode}";

            if (await localStorage.ContainKeyAsync(localStorageKey))
            {
                List<int> storageNumbers = await localStorage.GetItemAsync<List<int>>(localStorageKey);

                foreach (int number in storageNumbers)
                {
                    DrawNumber(number);
                }
            }
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

        public async Task Draw()
        {
            if (AvailableNumbers.Count == 0)
            {
                return;
            }
            int index = random.Next(AvailableNumbers.Count);
            int drawnNumber = AvailableNumbers[index];

            DrawNumber(drawnNumber);

            if (localStorage != null)
            {
                await localStorage.SetItemAsync<List<int>>(localStorageKey, DrawnNumbers);
            }
        }

        private void DrawNumber(int drawnNumber)
        {
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
