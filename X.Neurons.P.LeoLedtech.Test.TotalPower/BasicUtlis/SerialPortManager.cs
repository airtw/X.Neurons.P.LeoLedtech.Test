using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;


namespace X.Neurons.P.LeoLedtech.Test.TotalPower.BasicUtlis
{
    public class SerialPortManager : IDisposable
    {
        private SerialPort serialPort;
        private System.Windows.Forms.Timer scannerTimer;
        private System.Windows.Forms.Timer connectionMonitorTimer;
        private StringBuilder dataBuffer;
        private bool disposed = false;
        private DateTime lastSuccessfulCommunication = DateTime.Now;

        // 事件定義
        public event Action<string> DataReceived;
        public event Action<string> MessageReceived;
        public event Action<string, bool> StatusChanged;
        public event Action<Exception> ErrorOccurred;
        public event Action ConnectionLost;

        // 屬性
        public bool IsOpen => serialPort?.IsOpen ?? false;
        public bool IsScanning => scannerTimer?.Enabled ?? false;
        public bool IsMonitoring => connectionMonitorTimer?.Enabled ?? false;
        public string PortName { get; private set; }
        public int BaudRate { get; private set; }

        // 連線監控設定
        public int ConnectionCheckInterval { get; set; } = 1000; // 每秒檢查一次
        public bool IsCommunicationTimeout { get; set; } = false;
        public int CommunicationTimeout { get; set; } = 10000; // 10秒無通訊視為斷線

        // 建構子
        public SerialPortManager()
        {
            dataBuffer = new StringBuilder();
        }

        /// <summary>
        /// 初始化串列埠設定
        /// </summary>
        /// <param name="portName">COM 埠名稱 (例: COM3)</param>
        /// <param name="baudRate">波特率 (預設: 9600)</param>
        /// <param name="dataBits">資料位元 (預設: 8)</param>
        /// <param name="parity">同位元檢查 (預設: None)</param>
        /// <param name="stopBits">停止位元 (預設: One)</param>
        /// <param name="scanInterval">掃描間隔毫秒 (預設: 100)</param>
        public void Initialize(string portName, int baudRate = 9600, int dataBits = 8,
                              Parity parity = Parity.None, StopBits stopBits = StopBits.One,
                              int scanInterval = 100, bool isDTR = false)
        {
            try
            {
                // 清理舊的資源
                Cleanup();

                PortName = portName;
                BaudRate = baudRate;

                // 建立串列埠
                serialPort = new SerialPort();
                serialPort.PortName = portName;
                serialPort.BaudRate = baudRate;
                serialPort.DataBits = dataBits;
                serialPort.Parity = parity;
                serialPort.StopBits = stopBits;
                serialPort.Handshake = Handshake.None;

                if (isDTR)
                {
                    serialPort.DtrEnable = true;
                }

                // 設定超時
                serialPort.ReadTimeout = 1000;
                serialPort.WriteTimeout = 1000;

                // 設定緩衝區
                serialPort.ReadBufferSize = 4096;
                serialPort.WriteBufferSize = 4096;

                // 註冊事件
                serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.ErrorReceived += SerialPort_ErrorReceived;

                // 建立掃描 Timer
                scannerTimer = new System.Windows.Forms.Timer();
                scannerTimer.Interval = scanInterval;
                scannerTimer.Tick += ScannerTimer_Tick;

                // 建立連線監控 Timer
                connectionMonitorTimer = new System.Windows.Forms.Timer();
                connectionMonitorTimer.Interval = ConnectionCheckInterval;
                connectionMonitorTimer.Tick += ConnectionMonitorTimer_Tick;

                OnStatusChanged($"串列埠 {portName} 初始化完成", false);
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                throw new InvalidOperationException($"串列埠初始化失敗: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 開啟串列埠連接
        /// </summary>
        public bool Open()
        {
            try
            {
                if (serialPort == null)
                {
                    throw new InvalidOperationException("請先呼叫 Initialize 方法");
                }

                if (!serialPort.IsOpen)
                {
                    serialPort.Open();

                    // 清除緩衝區
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                    dataBuffer.Clear();

                    OnStatusChanged($"串列埠 {PortName} 已開啟", false);
                    return true;
                }

                return true;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                OnStatusChanged($"開啟串列埠失敗: {ex.Message}", true);
                return false;
            }
        }

        /// <summary>
        /// 關閉串列埠連接
        /// </summary>
        public void Close()
        {
            try
            {
                StopScanning();

                if (serialPort?.IsOpen == true)
                {
                    serialPort.Close();
                    OnStatusChanged($"串列埠 {PortName} 已關閉", false);
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                OnStatusChanged($"關閉串列埠錯誤: {ex.Message}", true);
            }
        }

        /// <summary>
        /// 開始掃描（啟動 Timer）
        /// </summary>
        public bool StartScanning()
        {
            try
            {
                if (Open())
                {
                    scannerTimer.Start();

                    // 啟動連線監控
                    StartConnectionMonitoring();

                    OnStatusChanged("開始資料掃描", false);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                return false;
            }
        }

        /// <summary>
        /// 停止掃描
        /// </summary>
        public void StopScanning()
        {
            try
            {
                if (scannerTimer?.Enabled == true)
                {
                    scannerTimer.Stop();
                    OnStatusChanged("停止資料掃描", false);
                }

                StopConnectionMonitoring();
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }

        /// <summary>
        /// 啟動連線監控
        /// </summary>
        public void StartConnectionMonitoring()
        {
            if (connectionMonitorTimer != null && !connectionMonitorTimer.Enabled)
            {
                lastSuccessfulCommunication = DateTime.Now;
                connectionMonitorTimer.Interval = ConnectionCheckInterval;
                connectionMonitorTimer.Start();
                OnStatusChanged("連線監控已啟動", false);
            }
        }

        /// <summary>
        /// 停止連線監控
        /// </summary>
        public void StopConnectionMonitoring()
        {
            if (connectionMonitorTimer?.Enabled == true)
            {
                connectionMonitorTimer.Stop();
                OnStatusChanged("連線監控已停止", false);
            }
        }

        /// <summary>
        /// 檢查連線狀態
        /// </summary>
        public bool CheckConnection()
        {
            try
            {
                if (serialPort?.IsOpen != true)
                {
                    return false;
                }

                // 檢查 COM 埠是否仍然存在
                string[] availablePorts = SerialPort.GetPortNames();
                if (!availablePorts.Contains(PortName))
                {
                    OnStatusChanged($"COM 埠 {PortName} 已不存在", true);
                    return false;
                }

                // 檢查是否長時間無通訊
                if (IsCommunicationTimeout)
                {
                    if (DateTime.Now - lastSuccessfulCommunication > TimeSpan.FromMilliseconds(CommunicationTimeout))
                    {
                        OnStatusChanged("通訊超時，可能已斷線", true);
                        return false;
                    }
                }
          

                return true;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                return false;
            }
        }

        /// <summary>
        /// 發送資料
        /// </summary>
        public bool SendData(string data)
        {
            try
            {
                if (serialPort?.IsOpen == true)
                {
                    serialPort.Write(data);
                    OnStatusChanged($"發送: {data}", false);
                    return true;
                }
                else
                {
                    OnStatusChanged("串列埠未開啟，無法發送資料", true);
                    return false;
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                OnStatusChanged($"發送資料失敗: {ex.Message}", true);
                return false;
            }
        }

        /// <summary>
        /// 發送資料並加上換行符
        /// </summary>
        public bool SendLine(string data)
        {
            return SendData(data + "\r\n");
        }

        /// <summary>
        /// 手動讀取一次資料
        /// </summary>
        public string ReadData()
        {
            try
            {
                if (serialPort?.IsOpen == true && serialPort.BytesToRead > 0)
                {
                    return serialPort.ReadExisting();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// 取得可用的 COM 埠列表
        /// </summary>
        public static string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }

        // 私有方法

        private void ScannerTimer_Tick(object sender, EventArgs e)
        {
            ReadSerialData();
        }

        private void ConnectionMonitorTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!CheckConnection())
                {
                    OnStatusChanged("偵測到連線中斷", true);
                    OnConnectionLost();

                    // 停止監控，避免重複觸發
                    StopConnectionMonitoring();
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }

        private void ReadSerialData()
        {
            if (serialPort?.IsOpen != true)
                return;

            try
            {
                if (serialPort.BytesToRead > 0)
                {
                    string data = serialPort.ReadExisting();

                    if (!string.IsNullOrEmpty(data))
                    {
                        ProcessReceivedData(data);
                    }
                }
            }
            catch (TimeoutException)
            {
                // 讀取超時，正常情況
            }
            catch (IOException ex)
            {
                OnErrorOccurred(ex);
                OnStatusChanged($"IO 錯誤: {ex.Message}", true);
                Close();
            }
            catch (InvalidOperationException ex)
            {
                OnErrorOccurred(ex);
                OnStatusChanged($"串列埠操作錯誤: {ex.Message}", true);
                Close();
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }

        private void ProcessReceivedData(string data)
        {
            // 更新最後成功通訊時間
            lastSuccessfulCommunication = DateTime.Now;

            // 觸發原始資料事件
            OnDataReceived(data);

            // 將資料加入緩衝區
            dataBuffer.Append(data);

            // 處理完整訊息（以換行符分隔）
            ProcessCompleteMessages();
        }

        private void ProcessCompleteMessages()
        {
            string buffer = dataBuffer.ToString();
            string[] messages = buffer.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (buffer.EndsWith("\r") || buffer.EndsWith("\n"))
            {
                // 處理所有完整訊息
                foreach (string message in messages)
                {
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        OnMessageReceived(message.Trim());
                    }
                }
                dataBuffer.Clear();
            }
            else if (messages.Length > 1)
            {
                // 處理除了最後一個不完整訊息外的所有訊息
                for (int i = 0; i < messages.Length - 1; i++)
                {
                    if (!string.IsNullOrWhiteSpace(messages[i]))
                    {
                        OnMessageReceived(messages[i].Trim());
                    }
                }
                // 保留最後一個不完整的訊息
                dataBuffer.Clear();
                dataBuffer.Append(messages[messages.Length - 1]);
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // 這個事件在背景執行緒中觸發
            try
            {
                SerialPort sp = (SerialPort)sender;
                string data = sp.ReadExisting();

                if (!string.IsNullOrEmpty(data))
                {
                    ProcessReceivedData(data);
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }

        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            string errorMessage = $"串列埠錯誤: {e.EventType}";
            OnStatusChanged(errorMessage, true);
        }

        // 事件觸發方法
        protected virtual void OnDataReceived(string data)
        {
            DataReceived?.Invoke(data);
        }

        protected virtual void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(message);
        }

        protected virtual void OnStatusChanged(string message, bool isError)
        {
            StatusChanged?.Invoke(message, isError);
        }

        protected virtual void OnErrorOccurred(Exception ex)
        {
            ErrorOccurred?.Invoke(ex);
        }

        protected virtual void OnConnectionLost()
        {
            ConnectionLost?.Invoke();
        }

        // 清理資源
        private void Cleanup()
        {
            try
            {
                scannerTimer?.Stop();
                scannerTimer?.Dispose();
                scannerTimer = null;

                connectionMonitorTimer?.Stop();
                connectionMonitorTimer?.Dispose();
                connectionMonitorTimer = null;

                if (serialPort?.IsOpen == true)
                {
                    serialPort.Close();
                }
                serialPort?.Dispose();
                serialPort = null;

                dataBuffer?.Clear();
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }

        // IDisposable 實作
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Cleanup();
                }
                disposed = true;
            }
        }

        ~SerialPortManager()
        {
            Dispose(false);
        }
    }
}
