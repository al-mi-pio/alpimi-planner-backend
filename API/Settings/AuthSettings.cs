namespace AlpimiAPI.Settings
{
    public class AuthSettings
    {
        public static int MinimumPasswordLength = 8;
        public static int MaximumPasswordLength = 256;
        public static RequiredCharacterTypes[]? RequiredCharacters =
        [
            RequiredCharacterTypes.SmallLetter,
            RequiredCharacterTypes.BigLetter,
            RequiredCharacterTypes.Digit,
            RequiredCharacterTypes.Symbol
        ];
    }
}
