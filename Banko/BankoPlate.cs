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
        public List<List<PlateNumber?>> Numbers { get; private set; } = new List<List<PlateNumber?>>(3);

        /// <summary>
        /// The number of the banko plate
        /// </summary>
        public int PlateNumber { get; private set; }

        /// <summary>
        /// The passcode used to generate the plate numbers
        /// </summary>
        public string PassCode { get; private set; }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Get if the plate has BANKO (ie. all 15 numbers)
        /// </summary>
        public bool HasBanko => DrawnRowCount[0] == 5 && DrawnRowCount[1] == 5 && DrawnRowCount[2] == 5;

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Get if the plate has at least two full rows
        /// </summary>
        public bool HasTwoRows => (DrawnRowCount[0] == 5 && DrawnRowCount[1] == 5) || (DrawnRowCount[0] == 5 && DrawnRowCount[2] == 5) || (DrawnRowCount[1] == 5 && DrawnRowCount[2] == 5);

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Get if the plate has at least one full row
        /// </summary>
        public bool HasOneRow => DrawnRowCount[0] == 5 || DrawnRowCount[1] == 5 || DrawnRowCount[2] == 5;

        private int[] DrawnRowCount { get; set; } = { 0, 0, 0 };


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

            int[] rowCount = { 0, 0, 0 };

            // Populate the columns where there are already 3 numbers
            foreach (var fullList in randomNumbers.Where(i => i.Count == 3))
            {
                int column = randomNumbers.IndexOf(fullList);
                for (int i = 0; i < 3; i++)
                {
                    int number = fullList.Min();
                    Numbers[i][column] = new PlateNumber(number);
                    fullList.Remove(number);
                    rowCount[i]++;
                }
            }

            // Place numbers in the smallest row first
            foreach (var numberList in randomNumbers)
            {
                int column = randomNumbers.IndexOf(numberList);
                while (numberList.Count > 0)
                {
                    int row = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        if (rowCount[i] < rowCount[row])
                        {
                            row = i;
                        }
                    }

                    int number = numberList.Min();

                    if (numberList.Count > 1)
                    {
                        if (row == 2)
                        {
                            // Bigger number must be in third row
                            number = numberList.Max();
                        }
                        else if (row == 0)
                        {
                            // Smaller number must be in first row
                        }
                        else if (rowCount[0] < rowCount[2])
                        {
                            // Third row is larger than first. Lets place the larger number in middle row causing the smaller number going in first row
                            number = numberList.Max();
                        }
                        else
                        {
                            // First row is larger than firs, Lets pace the smaller number in the middle row causing the larger number going in the last row
                        }
                    }

                    numberList.Remove(number);
                    Numbers[row][column] = new PlateNumber(number);
                    rowCount[row]++;
                }
            }

            // Validation disabled due to increased performance cost. Enable if algorithm changes
            // ValidateCard();
        }

        /// <summary>
        /// Validate that the card is according to rules
        /// </summary>
        /// <exception cref="FormatException">Exception stating that the card is not valid</exception>
        private void ValidateCard()
        {
            // Must have 15 numbers
            if (Numbers.Sum(row => row.Count(cell => cell != null)) != 15)
            {
                throw new FormatException($"card {PlateNumber} does not have 15 numbers");
            }
            // Each row mush have 5 numbers
            foreach (var row in Numbers)
            {
                if (row.Count(cell => cell != null) != 5)
                {
                    throw new FormatException($"Row in card {PlateNumber} does not have 5 numbers");
                }
            }
            // Each column must be ordered correctly
            for (int col = 0; col < 9; col++)
            {
                int lastNumber = 0;
                for (int row = 0; row < 2; row++)
                {
                    if (Numbers[row][col] == null)
                    {
                        continue;
                    }


                    if (lastNumber != 0 && Numbers[row][col]?.Number < lastNumber)
                    {
                        throw new FormatException($"Row in card {PlateNumber} does not have 5 numbers");
                    }
                    lastNumber = Numbers[row][col]?.Number ?? -1;
                }
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Mark a number on the card
        /// </summary>
        /// <param name="drawnNumber">drawn number</param>
        /// <returns>bool if a number is found on card</returns>
        internal bool MarkDrawnNumber(int drawnNumber)
        {
            for (int row = 0; row < Numbers.Count; row++)
            {
                foreach (var number in Numbers[row])
                {
                    if (number?.Number == drawnNumber)
                    {
                        number.IsDrawn = true;
                        DrawnRowCount[row]++;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Reset all drawn numbers
        /// </summary>
        internal void ResetDrawnNumbers()
        {
            DrawnRowCount = new int[3] { 0, 0, 0 };
            for (int row = 0; row < Numbers.Count; row++)
            {
                foreach (var number in Numbers[row])
                {
                    if (number == null)
                    {
                        continue;
                    }
                    number.IsDrawn = false;
                }
            }
        }
    }
}

