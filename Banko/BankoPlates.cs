using BlinkoBlanko.Common;

namespace BlinkoBlanko.Banko
{
    public class BankoPlates
    {
        public List<BankoPlate> Plates { get; private set; } = new List<BankoPlate>();

        public BankoPlates()
        {

        }


        public void Generate(string? passCode, int? numberOfPlates)
        {
            Plates.Clear();

            for (int i = 1; i <= (numberOfPlates ?? 50); i++)
            {
                BankoPlate plate = new BankoPlate(i, PassCodeUtils.ConvertToSeedNumber(passCode));
                plate.Create();
                Plates.Add(plate);
            }
        }
    }
}
