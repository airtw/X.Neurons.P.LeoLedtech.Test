using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace X.Neurons.P.LeoLedtech.Test.TotalPower.BasicUtlis
{
    public class ApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiClient(string baseUrl = null, int timeoutSeconds = 30)
        {
            _httpClient = new HttpClient();

            if (!string.IsNullOrEmpty(baseUrl))
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
            }

            _httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

            // JSON 序列化選項
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        #region Headers 管理

        public void AddHeader(string key, string value)
        {
            _httpClient.DefaultRequestHeaders.Add(key, value);
        }

        public void SetAuthorizationHeader(string token, string scheme = "Bearer")
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(scheme, token);
        }

        public void SetContentType(string contentType)
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(contentType));
        }

        #endregion

        #region GET 方法

        // GET - 非同步版本
        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(jsonContent, _jsonOptions);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"GET 請求失敗: {ex.Message}", ex);
            }
        }

        public async Task<string> GetStringAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"GET 請求失敗: {ex.Message}", ex);
            }
        }

        // GET - 同步版本
        public T Get<T>(string endpoint)
        {
            return GetAsync<T>(endpoint).GetAwaiter().GetResult();
        }

        public string GetString(string endpoint)
        {
            return GetStringAsync(endpoint).GetAwaiter().GetResult();
        }

        // 在 HttpClientWrapper 中加入帶超時的方法
        public async Task<T> GetWithTimeoutAsync<T>(string endpoint, TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);
            try
            {
                var response = await _httpClient.GetAsync(endpoint, cts.Token);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(jsonContent, _jsonOptions);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException($"請求超時 ({timeout.TotalSeconds} 秒)");
            }
        }
        #endregion

        #region POST 方法

        // POST - 非同步版本
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(responseJson, _jsonOptions);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"POST 請求失敗: {ex.Message}", ex);
            }
        }

        public async Task<string> PostAsync<TRequest>(string endpoint, TRequest data)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"POST 請求失敗: {ex.Message}", ex);
            }
        }

        // POST - 同步版本
        public TResponse Post<TRequest, TResponse>(string endpoint, TRequest data)
        {
            return PostAsync<TRequest, TResponse>(endpoint, data).GetAwaiter().GetResult();
        }

        public string Post<TRequest>(string endpoint, TRequest data)
        {
            return PostAsync(endpoint, data).GetAwaiter().GetResult();
        }

        #endregion

        #region PUT 方法

        // PUT - 非同步版本
        public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(endpoint, content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(responseJson, _jsonOptions);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"PUT 請求失敗: {ex.Message}", ex);
            }
        }

        public async Task<string> PutAsync<TRequest>(string endpoint, TRequest data)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(endpoint, content);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"PUT 請求失敗: {ex.Message}", ex);
            }
        }

        // PUT - 同步版本
        public TResponse Put<TRequest, TResponse>(string endpoint, TRequest data)
        {
            return PutAsync<TRequest, TResponse>(endpoint, data).GetAwaiter().GetResult();
        }

        public string Put<TRequest>(string endpoint, TRequest data)
        {
            return PutAsync(endpoint, data).GetAwaiter().GetResult();
        }

        #endregion

        #region DELETE 方法

        // DELETE - 非同步版本
        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"DELETE 請求失敗: {ex.Message}", ex);
            }
        }

        public async Task<T> DeleteAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(jsonContent, _jsonOptions);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"DELETE 請求失敗: {ex.Message}", ex);
            }
        }

        // DELETE - 同步版本
        public bool Delete(string endpoint)
        {
            return DeleteAsync(endpoint).GetAwaiter().GetResult();
        }

        public T Delete<T>(string endpoint)
        {
            return DeleteAsync<T>(endpoint).GetAwaiter().GetResult();
        }

        #endregion

        #region 進階方法

        // 上傳檔案
        public async Task<string> UploadFileAsync(string endpoint, byte[] fileData, string fileName, string fieldName = "file")
        {
            try
            {
                using var content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(fileData), fieldName, fileName);

                var response = await _httpClient.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"檔案上傳失敗: {ex.Message}", ex);
            }
        }

        // 下載檔案
        public async Task<byte[]> DownloadFileAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"檔案下載失敗: {ex.Message}", ex);
            }
        }

        // 取得回應狀態碼
        public async Task<(bool Success, int StatusCode, string Content)> GetWithStatusAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();

                return (response.IsSuccessStatusCode, (int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"請求失敗: {ex.Message}", ex);
            }
        }

        #endregion

        #region IDisposable 實作

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        #endregion
    }
}
