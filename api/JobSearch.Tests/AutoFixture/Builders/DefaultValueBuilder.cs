namespace JobSearch.Tests.AutoFixture.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using global::AutoFixture;
    using global::AutoFixture.Kernel;
    using JobSearch.Tests.Utils;

    public class DefaultValueBuilder : ISpecimenBuilder
    {
        private readonly Dictionary<PropertyInfo, object> _defaultValues = new Dictionary<PropertyInfo, object>();
        private readonly Dictionary<PropertyInfo, Func<object, ISpecimenContext, object>> _defaultBuilders = new Dictionary<PropertyInfo, Func<object, ISpecimenContext, object>>();
        private readonly Dictionary<Type, Func<ISpecimenContext, object, object>> _factories = new Dictionary<Type, Func<ISpecimenContext, object, object>>();

        public object Create(object request, ISpecimenContext context)
        {
            var type = request as Type;
            if (type != null && _factories.ContainsKey(type))
            {
                return _factories[type].Invoke(context, request);
            }

            if (TestDependencyScope.IsInitialized() && type != null && TestDependencyScope.Resolve(type) != null)
            {
                return TestDependencyScope.Resolve(type);
            }

            var property = request as PropertyInfo;
            if (property == null)
            {
                return new NoSpecimen();
            }

            if (_defaultValues.ContainsKey(property))
            {
                var value = _defaultValues[property];
                return value;
            }

            if (_defaultBuilders.ContainsKey(property))
            {
                var builder = _defaultBuilders[property];
                var value = builder(request, context);
                return value;
            }

            return new NoSpecimen();
        }

        public void SetDefaultValue(PropertyInfo property, object value)
        {
            _defaultValues[property] = value;
        }

        public void SetDefaultBuilder(PropertyInfo property, Func<object, ISpecimenContext, object> builder)
        {
            _defaultBuilders[property] = builder;
        }

        public void SetFactory<T>(Func<ISpecimenContext, object, object> factory)
        {
            _factories[typeof(T)] = factory;
        }

        private TypeWrapper<T> For<T>()
        {
            return new TypeWrapper<T>(this);
        }

        private class TypeWrapper<T>
        {
            private readonly DefaultValueBuilder _builder;

            public TypeWrapper(DefaultValueBuilder builder)
            {
                _builder = builder;
            }

            public TypeWrapper<T> SetDefaultValue(Expression<Func<T, object>> expression, object value)
            {
                var propertyInfo = expression.AsPropertyInfo();
                _builder.SetDefaultValue(propertyInfo, value);
                return this;
            }

            public TypeWrapper<T> ConstrainStringSize(Expression<Func<T, object>> expr, int maximumLength)
            {
                var property = expr.AsPropertyInfo();
                _builder.SetDefaultBuilder(
                    property,
                    (p, context) =>
                    {
                        var stringGenerator = new ConstrainedStringGenerator();
                        var constrainedStringRequest = new ConstrainedStringRequest(maximumLength);
                        return stringGenerator.Create(constrainedStringRequest, context);
                    });
                return this;
            }

            public TypeWrapper<T> BuildUsing(Func<ISpecimenContext, object, object> factory)
            {
                _builder.SetFactory<T>(factory);
                return this;
            }
        }
    }
}
