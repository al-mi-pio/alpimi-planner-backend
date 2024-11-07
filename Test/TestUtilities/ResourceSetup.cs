using alpimi_planner_backend.API.Locales;
using Microsoft.Extensions.Localization;
using Moq;

namespace AlpimiTest.TestUtilities
{
    public static class ResourceSetup
    {
        private static readonly Mock<IStringLocalizer<Errors>> _str = new();

        public static Mock<IStringLocalizer<Errors>> Setup()
        {
            var localizedErrorsWithArgs = new Dictionary<string, string>
            {
                { "invalidLoginOrPassword", "Invalid login or password" },
                { "invalidPassword", "Invalid password" },
            };

            var localizedErrorsWithoutArgs = new Dictionary<string, Func<object[], LocalizedString>>
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

            _str.Setup(localizer => localizer[It.IsAny<string>(), It.IsAny<object[]>()])
                .Returns(
                    (string key, object[] args) =>
                    {
                        if (localizedErrorsWithoutArgs.ContainsKey(key))
                        {
                            return localizedErrorsWithoutArgs[key](args);
                        }

                        return new LocalizedString(key, "huh");
                    }
                );
            _str.Setup(localizer => localizer[It.IsAny<string>()])
                .Returns(
                    (string key) =>
                    {
                        return localizedErrorsWithArgs.ContainsKey(key)
                            ? new LocalizedString(key, localizedErrorsWithArgs[key])
                            : new LocalizedString(key, "huh");
                    }
                );

            return _str;
        }
    }
}
