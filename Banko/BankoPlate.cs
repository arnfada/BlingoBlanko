using BlinkoBlanko.Common;

namespace BlinkoBlanko.Banko
{
    //-----------------------------------------------------------------------------------------
    /// <summary>
    /// The BankoPlate class represents a "Banko" card that contains 15 numbers from 1 to 90
    /// distributed in a 3x9 grid, with 5 numbers per row 
    /// </summary>
    public class BankoPlate
    {
        private int randomSeed = 0;

        /// <summary>
        /// The numbers on the plate. First list is the row (9) and the second list is numbers in the column (3)
        /// </summary>
        public List<List<PlateNumber?>> Numbers { get; private set; } = new List<List<PlateNumber?>>();

        /// <summary>
        /// The number of the banko plate
        /// </summary>
        public int PlateNumber { get; private set; }

        /// <summary>
        /// The passcode used to generate the plate numbers
        /// </summary>
        public string PassCode { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="plateNumber">the plate number</param>
        /// <param name="passCode">passcode used to generate the numbers</param>
        public BankoPlate(int plateNumber, string? passCode)
        {
            PlateNumber = plateNumber;
            PassCode = passCode ?? string.Empty;
            randomSeed = PassCodeUtils.ConvertToSeedNumber(passCode) + plateNumber;
            ClearNumbers();
        }

        /// <summary>
        /// Clear the numbers on the plate
        /// </summary>
        private void ClearNumbers()
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

        /// <summary>
        /// Create the plate, generating the numbers using the passcode
        /// </summary>
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

        /// <summary>
        /// Mark all numbers on the card that have been drawn
        /// </summary>
        /// <param name="drawnNumbers">list of drawn numbers</param>
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

