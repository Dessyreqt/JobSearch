namespace JobSearch.Tests.AutoFixture.Builders
{
    using System.Reflection;
    using global::AutoFixture.Kernel;

    public abstract class PropertyBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var property = request as PropertyInfo;

            if (property == null)
            {
                return new NoSpecimen();
            }

            if (AppliesTo(property))
            {
                return Create(context);
            }

            return new NoSpecimen();
        }

        protected abstract bool AppliesTo(PropertyInfo property);
        protected abstract object Create(ISpecimenContext context);
    }
}
