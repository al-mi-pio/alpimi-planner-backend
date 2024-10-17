using alpimi_planner_backend.API.Configuration;

namespace alpimi_planner_backend.API.Settings
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
            return MinimumPasswordLength;
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
