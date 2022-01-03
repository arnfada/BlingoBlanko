using BlinkoBlanko.Common;

namespace BlinkoBlanko.Banko
{
    public class BankoPlate
    {
        private int randomSeed = 0;

        public List<List<PlateNumber?>> Numbers { get; set; } = new List<List<PlateNumber?>>();

        public int PlateNumber { get; set; }

        public BankoPlate(int plateNumber, int seedNumber)
        {
            PlateNumber = plateNumber;
            randomSeed = seedNumber + plateNumber;
            ClearNumbers();
        }

        public void ClearNumbers()
        {
            Numbers.Clear();
            for (int i = 0; i < 3; i++)
            {
                Numbers.Add(new List<PlateNumber?>());
                for (int j = 0; j < 9; j++)
                {
                    Numbers[i].Add(null);
                }
            }
        }
        public void Create()
        {
            Random random = new Random(randomSeed);
            List<List<int>> randomNumbers = new List<List<int>>();

            // Start by adding numbers into each column,
            for (int i = 0; i < 9; i++)
            {
                randomNumbers.Add(new List<int>());

                int number = i * 10 + random.Next(0, 10) + 1;
                randomNumbers[i].Add(number);
            }

            // Populate numbers until 15
            while (randomNumbers.Sum(i => i.Count) < 15)
            {
                int number = random.Next(1, 91);
                int column = (int)(number - 1) / 10;

                if (randomNumbers[column].Count == 3)
                {
                    continue;
                }
                else if (randomNumbers[column].Contains(number))
                {
                    continue;
                }

                randomNumbers[column].Add(number);
            }

            // Populate the columns where there are already 3 numbers
            foreach (var fullList in randomNumbers.Where(i => i.Count == 3))
            {
                int column = randomNumbers.IndexOf(fullList);
                for (int i = 0; i < 3; i++)
                {
                    int number = fullList.Min();
                    Numbers[i][column] = new PlateNumber(number);
                    fullList.Remove(number);

                }
            }

            // Assign numbers to the first two rows randomly
            for (int row = 0; row < 2; row++)
            {
                while (Numbers[row].Count(i => i != null) < 5)
                {

                    int column = random.Next(0, 9);
                    if (Numbers[row][column] != null)
                    {
                        // Cell already has value assigned
                        continue;
                    }
                    if (randomNumbers[column].Count == 0)
                    {
                        // No more values from this column
                        continue;
                    }
                    int number = randomNumbers[column].Min();
                    Numbers[row][column] = new PlateNumber(number);
                    randomNumbers[column].Remove(number);
                }
            }

            // populate the remaining 5 numbers to the last row
            foreach (var remainingList in randomNumbers)
            {
                if (remainingList.Count == 0)
                {
                    continue;
                }
                int column = (remainingList[0] - 1) / 10;
                Numbers[2][column] = new PlateNumber(remainingList[0]);
            }
        }

        internal void MarkDrawnNumbers(List<int> drawnNumbers)
        {
            foreach (var row in Numbers)
            {
                foreach (var number in row)
                {
                    if (number != null)
                    {
                        number.IsDrawn = drawnNumbers.Contains(number.Number);
                    }
                }
            }
        }
    }
}

