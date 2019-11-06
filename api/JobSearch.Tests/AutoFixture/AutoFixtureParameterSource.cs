namespace JobSearch.Tests.AutoFixture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Fixie;
    using global::AutoFixture;
    using global::AutoFixture.Kernel;
    using JobSearch.Tests.AutoFixture.Builders;

    public class AutoFixtureParameterSource : ParameterSource
    {
        public static void CustomizeAutoFixture(Fixture fixture)
        {
            var propertyBuilders = typeof(PropertyBuilder).Assembly.GetTypes().Where(t => !t.IsAbstract && typeof(ISpecimenBuilder).IsAssignableFrom(t))
                .Select(Activator.CreateInstance).Cast<ISpecimenBuilder>();

            foreach (var propertyBuilder in propertyBuilders)
            {
                fixture.Customizations.Add(propertyBuilder);
            }

            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        public IEnumerable<object[]> GetParameters(MethodInfo method)
        {
            var fixture = new Fixture();
            CustomizeAutoFixture(fixture);

            var specimenContext = new SpecimenContext(fixture);
            var parameterTypes = method.GetParameters().Select(x => x.ParameterType);
            var arguments = parameterTypes.Select(specimenContext.Resolve).ToArray();

            return new[] { arguments };
        }
    }
}
