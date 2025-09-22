using MySqlConnector;
using System.Data;

namespace X.Neurons.P.LeoLedtech.Test.Server.BasicUtils
{
    public static class MariaDbHelper
    {
        /// <summary>
        /// 建立連線實體（不主動 Open，交由呼叫端 using/await using 控制生命週期）
        /// </summary>
        public static MySqlConnection CreateConnection(string connectionString)
            => new MySqlConnection(connectionString);

        /// <summary>
        /// 取得多筆查詢結果（強型別映射；欄位名需與屬性名對得上，不分大小寫）
        /// </summary>
        public static async Task<List<T>> QueryAsync<T>(
            string connectionString,
            string sql,
            object? parameters = null,
            CancellationToken ct = default
        ) where T : new()
        {
            await using var conn = CreateConnection(connectionString);
            await conn.OpenAsync(ct);
            await using var cmd = BuildCommand(conn, sql, parameters);

            var list = new List<T>();
            await using var reader = await cmd.ExecuteReaderAsync(ct);

            var props = typeof(T).GetProperties();
            var ordinals = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < reader.FieldCount; i++)
                ordinals[reader.GetName(i)] = i;

            while (await reader.ReadAsync(ct))
            {
                var item = new T();
                foreach (var p in props)
                {
                    if (!ordinals.TryGetValue(p.Name, out int idx)) continue;
                    var val = await reader.IsDBNullAsync(idx, ct) ? null : reader.GetValue(idx);
                    if (val == null) continue;

                    // 嘗試轉型（處理可空型別）
                    var targetType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                    p.SetValue(item, Convert.ChangeType(val, targetType));
                }
                list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// 取得單筆（找不到回傳 default）
        /// </summary>
        public static async Task<T?> QuerySingleOrDefaultAsync<T>(
            string connectionString,
            string sql,
            object? parameters = null,
            CancellationToken ct = default
        ) where T : new()
        {
            var list = await QueryAsync<T>(connectionString, sql, parameters, ct);
            return list.Count > 0 ? list[0] : default;
        }

        /// <summary>
        /// 以 DataTable 取得查詢結果（方便綁定 Grid）
        /// </summary>
        public static async Task<DataTable> QueryDataTableAsync(
            string connectionString,
            string sql,
            object? parameters = null,
            CancellationToken ct = default
        )
        {
            await using var conn = CreateConnection(connectionString);
            await conn.OpenAsync(ct);
            await using var cmd = BuildCommand(conn, sql, parameters);
            await using var reader = await cmd.ExecuteReaderAsync(ct);

            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

        /// <summary>
        /// 新增：回傳自動編號（LAST_INSERT_ID），若無自動編號則回傳受影響列數
        /// </summary>
        public static async Task<long> InsertAsync(
            string connectionString,
            string insertSql,
            object parameters,
            CancellationToken ct = default
        )
        {
            await using var conn = MariaDbHelper.CreateConnection(connectionString);
            await conn.OpenAsync(ct);
            await using var cmd = BuildCommand(conn, insertSql, parameters);

            await cmd.ExecuteNonQueryAsync(ct);
            return (long)cmd.LastInsertedId; // ★ 直接拿剛剛那次 INSERT 的自動編號
        }

        /// <summary>
        /// 編輯/刪除/一般非查詢命令：回傳受影響列數
        /// </summary>
        public static async Task<int> ExecuteAsync(
            string connectionString,
            string sql,
            object? parameters = null,
            CancellationToken ct = default
        )
        {
            await using var conn = CreateConnection(connectionString);
            await conn.OpenAsync(ct);
            await using var cmd = BuildCommand(conn, sql, parameters);
            return await cmd.ExecuteNonQueryAsync(ct);
        }

        /// <summary>
        /// 交易範例：將多個 SQL 包在同一個 Transaction 中
        /// </summary>
        public static async Task ExecuteInTransactionAsync(
            string connectionString,
            Func<MySqlConnection, MySqlTransaction, Task> work,
            IsolationLevel iso = IsolationLevel.ReadCommitted,
            CancellationToken ct = default
        )
        {
            await using var conn = CreateConnection(connectionString);
            await conn.OpenAsync(ct);
            await using var tx = await conn.BeginTransactionAsync(iso, ct);

            try
            {
                await work(conn, (MySqlTransaction)tx);
                await tx.CommitAsync(ct);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }

        // --- Helpers ---

        private static MySqlCommand BuildCommand(
            MySqlConnection conn,
            string sql,
            object? parameters
        )
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;

            if (parameters != null)
            {
                foreach (var (name, value) in ToNameValues(parameters))
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = name.StartsWith("@") ? name : "@" + name;
                    p.Value = value ?? DBNull.Value;
                    cmd.Parameters.Add(p);
                }
            }
            return cmd;
        }

        private static IEnumerable<(string Name, object? Value)> ToNameValues(object parameters)
        {
            if (parameters is IEnumerable<KeyValuePair<string, object?>> dict)
            {
                foreach (var kv in dict) yield return (kv.Key, kv.Value);
                yield break;
            }

            var props = parameters.GetType().GetProperties();
            foreach (var p in props)
            {
                yield return (p.Name, p.GetValue(parameters));
            }
        }
    }
}
