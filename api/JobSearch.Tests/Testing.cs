namespace JobSearch.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Dapper;
    using MediatR;

    public static class Testing
    {
        public static T Resolve<T>()
        {
            return TestDependencyScope.Resolve<T>();
        }

        public static object Resolve(Type type)
        {
            return TestDependencyScope.Resolve(type);
        }

        public static TResponse Send<TResponse>(IRequest<TResponse> request, Action postContainerCreation = null)
        {
            CreateNewContainerAndDbContext();
            postContainerCreation?.Invoke();

            var mediator = Resolve<IMediator>();
            var result = mediator.Send(request).Result;
            return result;
        }

        public static Task SendAsync<TResponse>(IRequest<TResponse> request, Action postContainerCreation = null)
        {
            CreateNewContainerAndDbContext();
            postContainerCreation?.Invoke();

            var mediator = Resolve<IMediator>();
            var result = mediator.Send(request);
            return result;
        }

        public static IEnumerable<TResult> Query<TResult>(string query, object parameters)
        {
            var connection = Resolve<Func<IDbConnection>>()();

            var result = connection.Query<TResult>(query, parameters);

            return result;
        }

        public static TResult QueryFirst<TResult>(string query, object parameters)
        {
            var connection = Resolve<Func<IDbConnection>>()();

            var result = connection.QueryFirst<TResult>(query, parameters);

            return result;
        }

        private static void CreateNewContainerAndDbContext()
        {
            if (TestDependencyScope.IsInitialized())
            {
                TestDependencyScope.End();
                TestDependencyScope.Begin();
            }
        }
    }
}