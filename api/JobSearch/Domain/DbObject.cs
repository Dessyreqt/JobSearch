namespace JobSearch.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using Dapper;

    public abstract class DbObject
    {
    }

    public abstract class DbIdObject : DbObject
    {
        public int Id { get; set; }
    }

    public class IdColumnAttribute : Attribute
    {
        public IdColumnAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public class TableNameAttribute : Attribute
    {
        public TableNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public class IgnoreOnInsertAttribute : Attribute
    {
    }

    public static class DbObjectExtensions
    {
        public static List<T> GetList<T>(this IDbConnection connection, string whereClause = null, object param = null)
        {
            var type = typeof(T);

            if (!type.IsSubclassOf(typeof(DbObject)))
            {
                throw new ArgumentException("This method only works for classes that inherit from DbObject");
            }

            string selectClause;

            if (type.IsSubclassOf(typeof(DbIdObject)))
            {
                var idColumnName = GetIdColumnName(type);
                selectClause = $"{idColumnName} AS Id, *";
            }
            else
            {
                selectClause = "*";
            }

            var tableName = GetTableName(type);

            if (whereClause == null)
            {
                var selectQuery = $"SELECT {selectClause} FROM [{tableName}]";
                var response = connection.Query<T>(selectQuery).ToList();

                return response;
            }
            else
            {
                var selectQuery = $"SELECT {selectClause} FROM [{tableName}] WHERE {whereClause}";
                var response = connection.Query<T>(selectQuery, param).ToList();

                return response;
            }
        }

        public static T GetById<T>(this IDbConnection connection, int id)
        {
            var type = typeof(T);

            if (!type.IsSubclassOf(typeof(DbIdObject)))
            {
                throw new ArgumentException("This method only works for classes that inherit from DbIdObject");
            }

            var tableName = GetTableName(type);
            var idColumnName = GetIdColumnName(type);
            var selectQuery = $"SELECT {idColumnName} AS Id, * FROM [{tableName}] WHERE [{idColumnName}] = @Id";

            var response = connection.QueryFirstOrDefault<T>(selectQuery, new { Id = id });

            return response;
        }

        public static void Save<T>(this IDbConnection connection, T obj)
        {
            if (!obj.GetType().IsSubclassOf(typeof(DbObject)))
            {
                throw new ArgumentException("This method only works for classes that inherit from DbObject");
            }

            var dbIdObj = obj as DbIdObject;

            if (dbIdObj != null)
            {
                if (dbIdObj.Id == default(int))
                {
                    connection.Insert(obj);
                }
                else
                {
                    connection.Update(obj);
                }
            }
            else
            {
                connection.Insert(obj);
            }
        }

        public static void Insert<T>(this IDbConnection connection, T obj)
        {
            var type = obj.GetType();

            if (!type.IsSubclassOf(typeof(DbObject)))
            {
                throw new ArgumentException("This method only works for classes that inherit from DbObject");
            }

            string idColumnName = null;

            if (type.IsSubclassOf(typeof(DbIdObject)))
            {
                idColumnName = GetIdColumnName(type);
            }

            var tableName = GetTableName(type);
            var properties = GetInsertProperties(type);
            var outputString = GetOutputString(properties, idColumnName);
            var insertQuery =
                $"INSERT INTO [{tableName}] ({string.Join(", ", properties.Insert.Select(x => $"[{x.Name}]"))}) {outputString}VALUES ({string.Join(", ", properties.Insert.Select(x => $"@{x.Name}"))}); ";

            var insertedObj = connection.QueryFirstOrDefault<T>(insertQuery, obj);

            UpdateObjectProperties(obj, insertedObj, properties, idColumnName != null);
        }

        public static void Update<T>(this IDbConnection connection, T obj)
        {
            var type = obj.GetType();

            if (!obj.GetType().IsSubclassOf(typeof(DbIdObject)))
            {
                throw new ArgumentException("This method only works for classes that inherit from DbIdObject");
            }

            var tableName = GetTableName(type);
            var idColumnName = GetIdColumnName(type);
            var properties = type.GetProperties().Where(property => property.Name != "Id").ToList();
            var updateQuery = $"UPDATE [{tableName}] SET {string.Join(", ", properties.Select(x => $"[{x.Name}] = @{x.Name}"))} WHERE [{idColumnName}] = @Id";

            var rowsAffected = connection.Execute(updateQuery, obj);

            if (rowsAffected == 0)
            {
                connection.Insert(obj);
            }
        }

        public static void Delete(this IDbConnection connection, DbIdObject obj)
        {
            var type = obj.GetType();

            if (!obj.GetType().IsSubclassOf(typeof(DbIdObject)))
            {
                throw new ArgumentException("This method only works for classes that inherit from DbIdObject");
            }

            var tableName = GetTableName(type);
            var idColumnName = GetIdColumnName(type);
            var deleteQuery = $"DELETE FROM [{tableName}] WHERE [{idColumnName}] = @Id";

            connection.Execute(deleteQuery, obj);
            obj.Id = 0;
        }

        private static void UpdateObjectProperties<T>(T obj, T insertedObj, InsertProperties properties, bool idColumn)
        {
            if (insertedObj == null)
            {
                return;
            }

            var type = obj.GetType();

            if (idColumn)
            {
                var newId = type.GetProperty("Id").GetValue(insertedObj, null);
                type.GetProperty("Id").SetValue(obj, newId);
            }

            foreach (var property in properties.Ignore)
            {
                var newValue = type.GetProperty(property.Name).GetValue(insertedObj, null);
                type.GetProperty(property.Name).SetValue(obj, newValue);
            }
        }

        private static string GetOutputString(InsertProperties properties, string idColumnName = null)
        {
            var propertyStrings = new List<string>();

            if (idColumnName != null)
            {
                propertyStrings.Add($"INSERTED.[{idColumnName}] AS [Id]");
            }

            foreach (var property in properties.Ignore)
            {
                propertyStrings.Add($"INSERTED.[{property.Name}]");
            }

            if (propertyStrings.Count > 0)
            {
                return $"OUTPUT {string.Join(", ", propertyStrings)} ";
            }

            return string.Empty;
        }

        private static string GetIdColumnName(Type type)
        {
            var idColumnName = $"{type.Name}Id";

            var idColumnAttributes = type.GetCustomAttributes(typeof(IdColumnAttribute), false);

            if (idColumnAttributes.Length != 0)
            {
                var idColumnAttribute = idColumnAttributes[0] as IdColumnAttribute;

                idColumnName = idColumnAttribute?.Name ?? idColumnName;
            }

            return idColumnName;
        }

        private static string GetTableName(Type type)
        {
            var tableName = $"{type.Name}";

            var tableNameAttributes = type.GetCustomAttributes(typeof(TableNameAttribute), false);

            if (tableNameAttributes.Length != 0)
            {
                var tableNameAttribute = tableNameAttributes[0] as TableNameAttribute;

                tableName = tableNameAttribute?.Name ?? tableName;
            }

            return tableName;
        }

        private static InsertProperties GetInsertProperties(Type type)
        {
            var properties = type.GetProperties();
            var insertProperties = new InsertProperties();

            foreach (var property in properties)
            {
                var propertyAttributes = property.GetCustomAttributes(typeof(IgnoreOnInsertAttribute), false);

                if (type.IsSubclassOf(typeof(DbIdObject)) && property.Name == "Id")
                {
                    continue;
                }

                if (propertyAttributes.Length == 0)
                {
                    insertProperties.Insert.Add(property);
                }
                else
                {
                    insertProperties.Ignore.Add(property);
                }
            }

            return insertProperties;
        }

        private class InsertProperties
        {
            public List<PropertyInfo> Insert { get; } = new List<PropertyInfo>();
            public List<PropertyInfo> Ignore { get; } = new List<PropertyInfo>();
        }
    }
}
