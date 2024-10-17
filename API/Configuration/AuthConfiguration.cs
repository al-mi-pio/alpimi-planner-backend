namespace alpimi_planner_backend.API.Configuration
{
    public class AuthConfiguration
    {
        public int MinimumPasswordLength = 8;
        public int MaximumPasswordLength = 256;
        public RequiredCharacterTypes[]? RequiredCharacters =
        [
            RequiredCharacterTypes.SmallLetter,
            RequiredCharacterTypes.BigLetter,
            RequiredCharacterTypes.Digit,
            RequiredCharacterTypes.Symbol
        ];

        public int GetMaximumPasswordLength()
        {
            return MaximumPasswordLength;
        }

        public int GetMinimumPasswordLength()
        {
            return MinimumPasswordLength;
        }

        public RequiredCharacterTypes[]? GetRequiredCharacters()
        {
            return RequiredCharacters;
        }
    }
}
