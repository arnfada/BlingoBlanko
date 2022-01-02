namespace BlinkoBlanko.Bingo
{
    public class BingoPlates
    {
        public List<BingoPlate> Plates { get; private set; } = new List<BingoPlate>();

        public BingoPlates()
        {

        }


        public void Generate(string? passCode = "ABCD", int? numberOfPlates = 50)
        {
            Plates.Clear();

            if (string.IsNullOrEmpty(passCode))
            {
                passCode = "ABCD";
            }
            if (!numberOfPlates.HasValue)
            {
                numberOfPlates = 50;
            }


            int passCodeSeed = 0;
            foreach (char passCodeChar in passCode)
            {
                passCodeSeed += (int)passCodeChar;
            }

            for (int i = 1; i <= numberOfPlates; i++)
            {
                BingoPlate plate = new BingoPlate(i);
                plate.Create(passCodeSeed);
                Plates.Add(plate);
            }
        }
    }
}
