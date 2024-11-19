using alpimi_planner_backend.API.Locales;
using Microsoft.Extensions.Localization;
using Moq;

namespace AlpimiTest.TestSetup
{
    public static class ResourceSetup
    {
        private static readonly Mock<IStringLocalizer<Errors>> _str = new();
        private static readonly Dictionary<string, string> localizedErrorsWithArgs = new Dictionary<
            string,
            string
        >
        {
            { "invalidLoginOrPassword", "Invalid login or password" },
            { "invalidPassword", "Invalid password" },
        };
        private static readonly Dictionary<
            string,
            Func<object[], LocalizedString>
        > localizedErrorsWithoutArgs = new Dictionary<string, Func<object[], LocalizedString>>
        {
            {
                "badParameter",
                args => new LocalizedString(
                    "badParameter",
                    string.Format("{0} parameter is invalid", args[0])
                )
            },
            {
                "longPassword",
                args => new LocalizedString(
                    "longPassword",
                    string.Format("Password cannot be longer than {0} characters", args[0])
                )
            },
            {
                "shortPassword",
                args => new LocalizedString(
                    "shortPassword",
                    string.Format("Password cannot be shorter than {0} characters", args[0])
                )
            },
            {
                "alreadyExists",
                args => new LocalizedString(
                    "alreadyExists",
                    string.Format("There is already a {0} with the name {1}", args[0], args[1])
                )
            },
            {
                "passwordMustContain",
                args => new LocalizedString(
                    "passwordMustContain",
                    string.Format(
                        "Password must contain at least one of the following: {0}",
                        args[0]
                    )
                )
            }
        };

        public static Mock<IStringLocalizer<Errors>> Setup()
        {
            _str.Setup(localizer => localizer[It.IsAny<string>(), It.IsAny<object[]>()])
                .Returns(
                    (string key, object[] args) =>
                    {
                        return localizedErrorsWithoutArgs[key](args);
                    }
                );
            _str.Setup(localizer => localizer[It.IsAny<string>()])
                .Returns(
                    (string key) =>
                    {
                        return new LocalizedString(key, localizedErrorsWithArgs[key]);
                    }
                );

            return _str;
        }
    }
}
