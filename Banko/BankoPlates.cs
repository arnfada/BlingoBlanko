namespace BlinkoBlanko.Banko
{
    /// <summary>
    /// The BankoPlates class manages generating multiple BankoPlates
    /// </summary>
    public class BankoPlates
    {
        /// <summary>
        /// The Plates
        /// </summary>
        public List<BankoPlate> Plates { get; private set; } = new List<BankoPlate>();

        /// <summary>
        /// Constructor
        /// </summary>
        public BankoPlates()
        {

        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Generate banko plates
        /// </summary>
        /// <param name="passCode">the pass-code for randomness</param>
        /// <param name="numberOfPlates">number of plates to generate</param>
        public void Generate(string? passCode, int? numberOfPlates)
        {
            // Limit the amount of plates to 1000.
            if (numberOfPlates.HasValue && numberOfPlates > 1000)
            {
                numberOfPlates = 1000;
            }

            Plates.Clear();
            for (int i = 1; i <= (numberOfPlates ?? 50); i++)
            {
                BankoPlate plate = new BankoPlate(i, passCode);
                plate.Create();
                Plates.Add(plate);
            }
        }

        /// <summary>
        /// Reset drawn numbers on plates
        /// </summary>
        internal void ResetDrawnNumbers()
        {
            foreach (var plate in Plates)
            {
                plate.ResetDrawnNumbers();
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Parse a number string to only generate banko plates with specific numbers
        /// </summary>
        /// <param name="passCode">the pass-code for randomness</param>
        /// <param name="numberString">The number string containing numbers or range numbers. 
        /// Supports comma separated numbers and range of numbers like A-B</param>
        public void ParseNumbers(string? passCode, string? numberString)
        {
            if (string.IsNullOrWhiteSpace(numberString))
            {
                return;
            }

            List<int> cardNumbers = new List<int>();

            string[] commaSplits = numberString.Split(',');
            foreach (string commaSplit in commaSplits)
            {
                if (commaSplit.Contains("-"))
                {
                    //Is range of number ex. 5-12
                    string[] rangeSplit = commaSplit.Split("-");
                    if (rangeSplit.Length != 2 || !int.TryParse(rangeSplit[0], out int lowNum) || !int.TryParse(rangeSplit[1], out int highNum))
                    {
                        continue;
                    }

                    for (int i = lowNum; i <= highNum; i++)
                    {
                        cardNumbers.Add(i);
                    }
                }
                else
                {
                    if (int.TryParse(commaSplit, out int number))
                    {
                        cardNumbers.Add(number);
                    }
                }
            }

            // Generate plates
            Plates.Clear();
            foreach (int number in cardNumbers)
            {
                BankoPlate plate = new BankoPlate(number, passCode);
                plate.Create();
                Plates.Add(plate);
            }
        }
    }
}
