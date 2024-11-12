using DotNetEnv;
using Xunit;

namespace AlpimiTest.TestSetup
{
    using DotNetEnv;
    using Xunit;

    public class EnvSetupFixture
    {
        static EnvSetupFixture()
        {
            DotNetEnv.Env.Load(
                Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".env")
            );
        }
    }

    [CollectionDefinition("Sequential Tests")]
    public class SequentialTestCollection : ICollectionFixture<EnvSetupFixture> { }

    [CollectionDefinition("Global setup collection")]
    public class GlobalSetupCollection : ICollectionFixture<EnvSetupFixture> { }
}
