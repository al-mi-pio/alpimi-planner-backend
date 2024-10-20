namespace alpimi_planner_backend.API.Configuration
{
    public class AuthConfiguration
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
