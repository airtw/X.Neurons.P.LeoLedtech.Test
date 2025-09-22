using System.Reflection;

namespace X.Neurons.P.LeoLedtech.Test.Server.BasicUtils
{
    public class SqlUpdateBuilder
    {
        /// <summary>
        /// 根據傳入物件產生 UPDATE SQL
        /// </summary>
        /// <typeparam name="T">類別型別</typeparam>
        /// <param name="tableName">資料表名稱</param>
        /// <param name="entity">要更新的物件</param>
        /// <param name="idColumn">主鍵欄位名稱 (預設 Id)</param>
        /// <returns>SQL 字串與參數字典</returns>
        public static (string Sql, Dictionary<string, object?> Parameters) BuildUpdateSql<T>(
            string tableName,
            T entity,
            string idColumn = "Id")
        {
            var type = typeof(T);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var updates = new List<string>();
            var parameters = new Dictionary<string, object?>();

            object? idValue = null;

            foreach (var prop in props)
            {
                var name = prop.Name;
                var value = prop.GetValue(entity);

                if (string.Equals(name, idColumn, StringComparison.OrdinalIgnoreCase))
                {
                    idValue = value;
                    parameters[idColumn] = value;
                    continue;
                }

                // 判斷是否要加入更新（排除 null 或 default）
                if (value == null) continue;

                if (prop.PropertyType == typeof(string) && string.IsNullOrEmpty(value.ToString()))
                    continue;

                if (prop.PropertyType.IsValueType && Activator.CreateInstance(prop.PropertyType)?.Equals(value) == true)
                    continue;

                updates.Add($"{name} = @{name}");
                parameters[name] = value;
            }

            if (!updates.Any())
                throw new InvalidOperationException("沒有可更新的欄位");

            if (idValue == null)
                throw new InvalidOperationException($"主鍵 {idColumn} 不可為 null");

            var sql = $"UPDATE {tableName} SET {string.Join(", ", updates)} WHERE {idColumn} = @{idColumn}";
            return (sql, parameters);
        }
    }
}
