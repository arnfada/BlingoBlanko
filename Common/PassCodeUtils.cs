namespace BlinkoBlanko.Common
{
    public static class PassCodeUtils
    {
        public static int ConvertToSeedNumber(string? passCode)
        {
            if (string.IsNullOrEmpty(passCode))
            {
                passCode = "ABCD";
            }

            int passCodeSeed = 0;
            foreach (char passCodeChar in passCode)
            {
                passCodeSeed += (int)passCodeChar;
            }
            return passCodeSeed;
        }
    }
}
