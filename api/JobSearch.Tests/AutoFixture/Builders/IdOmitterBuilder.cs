namespace JobSearch.Tests.AutoFixture.Builders
{
    using System;
    using System.Reflection;
    using global::AutoFixture.Kernel;

    public class IdOmitterBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var property = request as PropertyInfo;

            if (property == null)
            {
                return new NoSpecimen();
            }

            if (IsId(property))
            {
                return Activator.CreateInstance(property.PropertyType);
            }

            return new NoSpecimen();
        }

        private static bool IsId(PropertyInfo property)
        {
            return property.Name.EndsWith("Id") && (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?));
        }
    }
}
