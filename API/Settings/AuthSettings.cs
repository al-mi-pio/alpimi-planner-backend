namespace AlpimiAPI.Settings
{
    public static class AuthSettings
    {
        public const int MinimumPasswordLength = 8;
        public const int MaximumPasswordLength = 256;
        public static RequiredCharacterTypes[]? RequiredCharacters =
        [
            RequiredCharacterTypes.SmallLetter,
            RequiredCharacterTypes.BigLetter,
            RequiredCharacterTypes.Digit,
            RequiredCharacterTypes.Symbol
        ];
    }
}
