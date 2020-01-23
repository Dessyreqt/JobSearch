namespace JobSearch.Tests
{
    using System;
    using System.Data;
    using Fixie;
    using global::AutoFixture;
    using global::AutoFixture.Kernel;
    using JobSearch.Tests.AutoFixture;
    using Respawn;

    public class TestingConvention : Discovery, Execution
    {
        public TestingConvention()
        {
            Classes.Where(x => x.Name.StartsWith("When"));

            Methods.Where(method => method.IsVoid() || method.IsAsync());

            Parameters.Add<AutoFixtureParameterSource>();
        }

        public void Execute(TestClass testClass)
        {
            TestingIoC.Initialize();
            TestDependencyScope.Begin();

            ResetDatabase();

            var fixture = new Fixture();
            AutoFixtureParameterSource.CustomizeAutoFixture(fixture);
            var instance = new SpecimenContext(fixture).Resolve(testClass.Type);

            testClass.RunCases(@case => { @case.Execute(instance); });

            TestDependencyScope.End();

            instance.Dispose();
        }

        private static void ResetDatabase()
        {
            var checkpoint = new Checkpoint
            {
                SchemasToExclude = new[] { "RoundhousE" },
                TablesToIgnore = new[]
                {
                    "sysdiagrams",
                    "ApplicationStatus",
                }
            };

            checkpoint.Reset(TestDependencyScope.Resolve<Func<IDbConnection>>()().ConnectionString).GetAwaiter().GetResult();
        }
    }
}
