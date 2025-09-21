namespace AlpimiAPI.Settings
{
    public enum RequiredCharacterTypes
    {
        SmallLetter,
        BigLetter,
        Digit,
        Symbol
    }

    public enum AllowedCharacterTypes
    {
        SmallLetters,
        BigLetters,
        Digits,
        Symbols,
        Spaces
    }

    public static class CharacterSettings
    {
        public static AllowedCharacterTypes[]? AllowedCharacterTypesForCustomURL =
        [
            AllowedCharacterTypes.SmallLetters,
            AllowedCharacterTypes.BigLetters,
            AllowedCharacterTypes.Digits
        ];

        public static AllowedCharacterTypes[]? AllowedCharacterTypesForScheduleName =
        [
            AllowedCharacterTypes.SmallLetters,
            AllowedCharacterTypes.BigLetters,
            AllowedCharacterTypes.Digits,
            AllowedCharacterTypes.Spaces
        ];

        public static AllowedCharacterTypes[]? AllowedCharacterTypesForLogin =
        [
            AllowedCharacterTypes.SmallLetters,
            AllowedCharacterTypes.BigLetters,
            AllowedCharacterTypes.Digits
        ];
        public static AllowedCharacterTypes[]? AllowedCharacterTypesForPassword =
        [
            AllowedCharacterTypes.SmallLetters,
            AllowedCharacterTypes.BigLetters,
            AllowedCharacterTypes.Digits,
            AllowedCharacterTypes.Symbols,
            AllowedCharacterTypes.Spaces
        ];

        public static RequiredCharacterTypes[]? RequiredCharacterTypesForPassword =
        [
            RequiredCharacterTypes.SmallLetter,
            RequiredCharacterTypes.BigLetter,
            RequiredCharacterTypes.Digit,
            RequiredCharacterTypes.Symbol
        ];
    }
}
