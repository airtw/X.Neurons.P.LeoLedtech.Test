using DevExpress.Office.Services;
using DevExpress.Spreadsheet;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Net.Http;
using X.Neurons.P.LeoLedtech.Test.TotalPower.BasicUtlis;
using X.Neurons.P.LeoLedtech.Test.TotalPower.Models;

namespace X.Neurons.P.LeoLedtech.Test.TotalPower
{
    public partial class Main : DevExpress.XtraEditors.XtraForm
    {
        private bool _isRequestInProgress = false;
        private DateTime _lastSuccessTime = DateTime.MinValue;

        private SerialPortManager scannerSerialManager;
        private SerialPortManager jigSerialManager;

        private Models.Test.TestStep ProductionTestStep;

        private bool isFail = true;
        public Main()
        {
            InitializeComponent();
            #region 初始化狀態欄樣式
            bsiServerStatus.ItemAppearance.Normal.ForeColor = Color.LightGreen;
            bsiServerStatus.ItemAppearance.Normal.Options.UseForeColor = true;
            bsiServerStatus.ItemAppearance.Normal.Font = new Font("Microsoft JhengHei UI", 12, FontStyle.Bold);

            bsiSystemTime.ItemAppearance.Normal.Options.UseForeColor = true;
            bsiSystemTime.ItemAppearance.Normal.Font = new Font("Microsoft JhengHei UI", 12, FontStyle.Bold);

            bsiScannerStatus.ItemAppearance.Normal.Options.UseForeColor = true;
            bsiScannerStatus.ItemAppearance.Normal.Font = new Font("Microsoft JhengHei UI", 12, FontStyle.Bold);

            bsiJigStatus.ItemAppearance.Normal.Options.UseForeColor = true;
            bsiJigStatus.ItemAppearance.Normal.Font = new Font("Microsoft JhengHei UI", 12, FontStyle.Bold);
            #endregion
            GlobalSettings.ServerApiClient = new ApiClient($"http://{AppSetting.Default.serverIP}:12500");

        }
        private void Main_Load(object sender, EventArgs e)
        {
            #region 初始化介面顯示
            ch1_Current_Value.Text = "0.0";
            ch2_Current_Value.Text = "0.0";
            ch3_Current_Value.Text = "0.0";
            ch4_Current_Value.Text = "0.0";

            #endregion
            InitAllChart();
            InitTestStepGrid();
            InitTestLogGrid();
            //GetTestStep(); //獲取測試資訊

            InitScannerSerialPort();
            InitJigSerialPort();
        }

        private void btn_about_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            About logForm = new About();
            logForm.ShowDialog();
        }

        private void btn_system_setting_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SystemConfig systemConfigForm = new SystemConfig();
            systemConfigForm.ShowDialog();
        }

        #region 伺服器心跳檢測
        private async void serverTimer_Tick(object sender, EventArgs e)
        {
            if (_isRequestInProgress) return;

            _isRequestInProgress = true;

            try
            {
                // 設定 5 秒超時
                var timeout = TimeSpan.FromSeconds(5);
                var t = await GlobalSettings.ServerApiClient.GetWithTimeoutAsync<Response_Heartbeat>($"/Heartbeat/{AppSetting.Default.clientID}", timeout);

                if (t?.Status == true)
                {
                    bsiSystemTime.Caption = $"{t.Date} {t.Time}";
                    bsiServerStatus.Caption = "伺服器連線正常";
                    bsiServerStatus.ItemAppearance.Normal.ForeColor = Color.LightGreen;
                    _lastSuccessTime = DateTime.Now;
                    GlobalSettings.IsServerConnect = true;

                }
                else
                {
                    UpdateDisconnectedStatus("伺服器回應異常");
                }
            }
            catch (TimeoutException)
            {
                UpdateDisconnectedStatus("連線超時");
            }
            catch (HttpRequestException ex)
            {
                UpdateDisconnectedStatus($"連線失敗: {ex.Message}");
            }
            finally
            {
                _isRequestInProgress = false;
            }
        }
        private void UpdateDisconnectedStatus(string message)
        {
            bsiSystemTime.Caption = "無法取得伺服器時間";
            bsiServerStatus.Caption = $"{message}";
            bsiServerStatus.ItemAppearance.Normal.ForeColor = Color.LightPink;

            if (DateTime.Now - _lastSuccessTime > TimeSpan.FromMinutes(5))
            {
                // 可以在這裡調整 Timer 間隔或執行其他邏輯
            }
        }
        #endregion

        #region 圖表(Chart)
        private void InitAllChart()
        {
            InitChart(ch1_chart);
            InitChart(ch2_chart);
            InitChart(ch3_chart);
            InitChart(ch4_chart);
        }
        /// <summary>
        /// 初始化圖表
        /// </summary>
        private void InitChart(ChartControl chart)
        {
            chart.Series.Clear();

            // 建立 Step Line Series
            var series = new Series("IV Curve", ViewType.StepLine)
            {
                ArgumentScaleType = ScaleType.Qualitative,
                SeriesPointsSorting = DevExpress.XtraCharts.SortingMode.None,
            };

            var view = (StepLineSeriesView)series.View;
            view.MarkerVisibility = DefaultBoolean.True;
            view.LineStyle.Thickness = 2;

            chart.Series.Add(series);

            // 取得 XY 圖表
            var dia = (XYDiagram)chart.Diagram;

            // 關閉座標軸標題與標籤
            //dia.AxisX.Label.Visibility = DefaultBoolean.False;
            dia.AxisX.Title.Visibility = DefaultBoolean.False;
            dia.AxisX.Tickmarks.Visible = false;
            dia.AxisX.GridLines.Visible = false;

            //dia.AxisY.Label.Visibility = DefaultBoolean.False;
            dia.AxisY.Title.Visibility = DefaultBoolean.False;
            dia.AxisY.Tickmarks.Visible = false;
            dia.AxisY.GridLines.Visible = false;

            chart.Legend.Visibility = DefaultBoolean.False;

            // 背景 / 邊框
            chart.BorderOptions.Visibility = DefaultBoolean.False;
            chart.BackColor = Color.White;

            // Tooltip 與十字準星
            chart.ToolTipEnabled = DefaultBoolean.True;
            series.ToolTipPointPattern = "電流: {A:F2} mA\n電壓: {V:F2} V";
            chart.CrosshairEnabled = DefaultBoolean.True;
            series.CrosshairLabelPattern = "電流: {A:F2} mA\n電壓: {V:F2} V";

            // 縮放捲動
            dia.EnableAxisXScrolling = true;
            dia.EnableAxisXZooming = true;
            dia.EnableAxisYScrolling = true;
            dia.EnableAxisYZooming = true;
        }
        /// <summary>
        /// 動態添加資料點
        /// </summary>
        private void AddDataPoint(ChartControl chart, double current, double voltage)
        {
            Series series = chart.Series[0];  // 取第一個序列
            series.Points.Add(new SeriesPoint(current, voltage));
        }
        /// <summary>
        /// 清除資料點
        /// </summary>
        private void ClearDataPoints(ChartControl chart)
        {
            if (chart.Series.Count > 0)
            {
                Series series = chart.Series[0];
                series.Points.Clear();  // 清掉所有資料點
            }
        }
        #endregion

        #region 測試步驟
        private void InitTestStepGrid()
        {

            // 資料來源
            var logList = new BindingList<TestStepEntry>();
            gcTestStep.DataSource = logList;

            // 只讀 + 自動列高（換行需要）
            gvTestStep.OptionsBehavior.Editable = false;
            gvTestStep.OptionsView.RowAutoHeight = true;
            gvTestStep.OptionsView.ColumnAutoWidth = true;
            gvTestStep.OptionsView.ShowGroupPanel = false;

            // 取得要換行的欄位（請確認屬性名稱跟你的資料模型一致）
            var colMsg = gvTestStep.Columns["TestContent"];   // 若你的欄位其實叫 Message，請改成 "Message"
            if (colMsg != null)
            {
                // 建議用 MemoEdit 支援多行
                var memo = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                memo.ReadOnly = true; // viewer
                colMsg.ColumnEdit = memo;

                //（可選）Appearance 的換行也開著
                colMsg.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            }

            // 簡單條件格式化（等級=Error → 紅色）
            StyleFormatCondition cond = new StyleFormatCondition
            {
                Appearance = { BackColor = Color.LightCoral, ForeColor = Color.White },
                Condition = FormatConditionEnum.Equal,
                Value1 = "Error",
                Column = gvTestStep.Columns["Level"]
            };
            gvTestStep.FormatConditions.Add(cond);
        }
        private void AddStep(string id, string testContent)
        {
            var logList = (BindingList<TestStepEntry>)gcTestStep.DataSource;
            logList.Add(new TestStepEntry
            {
                ID = id,
                TestContent = testContent
            });
        }
        private async void GetTestStep()
        {
            var t = await GlobalSettings.ServerApiClient.GetAsync<Models.Test.TestStep>($"/TestStep/{AppSetting.Default.clientID}");
            ProductionTestStep = t;
            ClearTestStep();
            foreach (var cable in t.Cable)
            {
                switch (cable.ID)
                {
                    case 1:
                        ch1_CableColor.SvgImage = svgImageCollection1[cable.CableColor];
                        break;
                    case 2:
                        ch2_CableColor.SvgImage = svgImageCollection1[cable.CableColor];
                        break;
                    case 3:
                        ch3_CableColor.SvgImage = svgImageCollection1[cable.CableColor];
                        break;
                    case 4:
                        ch4_CableColor.SvgImage = svgImageCollection1[cable.CableColor];
                        break;
                }
            }
            foreach (var step in t.Step)
            {
                var content = string.Empty;
                content += $"電源供應器 輸出電壓:{step.Voltage}v\r\n";
                foreach (var ch in step.Channel)
                {
                    content += $"通道:{ch.ID} ";
                    if (ch.HH != 0)
                    {
                        content += $"高高限:{ch.HH}mA ";
                    }
                    if (ch.H != 0)
                    {
                        content += $"高限:{ch.H}mA ";
                    }
                    if (ch.L != 0)
                    {
                        content += $"低限:{ch.L}mA ";
                    }
                    if (ch.LL != 0)
                    {
                        content += $"低低限:{ch.LL}mA ";
                    }
                    if (!ch.Equals(step.Channel.Last()))
                    {
                        content += "\r\n";
                    }
                }
                AddStep(step.ID.ToString(), content);
            }
        }
        private void ClearTestStep()
        {
            if (gcTestStep.DataSource is BindingList<TestStepEntry> logList)
            {
                logList.Clear();
            }
        }
        #endregion

        #region 測試紀錄
        private void InitTestLogGrid()
        {

            // 資料來源
            var logList = new BindingList<TestLogEntry>();
            gcTestLog.DataSource = logList;

            // 只讀 + 自動列高（換行需要）
            gvTestLog.OptionsBehavior.Editable = false;
            gvTestLog.OptionsView.RowAutoHeight = true;
            gvTestLog.OptionsView.ColumnAutoWidth = true;
            gvTestLog.OptionsView.ShowGroupPanel = false;

            // 取得要換行的欄位（請確認屬性名稱跟你的資料模型一致）
            var colMsg = gvTestLog.Columns["Content"];
            if (colMsg != null)
            {
                // 建議用 MemoEdit 支援多行
                var memo = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                memo.ReadOnly = true; // viewer
                colMsg.ColumnEdit = memo;

                //（可選）Appearance 的換行也開著
                colMsg.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            }

            // 簡單條件格式化（等級=Error → 紅色）
            StyleFormatCondition cond = new StyleFormatCondition
            {
                Appearance = { BackColor = Color.LightCoral, ForeColor = Color.White },
                Condition = FormatConditionEnum.Equal,
                Value1 = "Error",
                Column = gvTestStep.Columns["Level"]
            };
            gvTestLog.FormatConditions.Add(cond);
        }
        private void AddTestLog(string dateTime, string id, string content)
        {
            var logList = (BindingList<TestLogEntry>)gcTestLog.DataSource;
            logList.Add(new TestLogEntry
            {
                DateTime = dateTime,
                ID = id,
                Content = content
            });
        }
        private void ClearTestLog()
        {
            if (gcTestLog.DataSource is BindingList<TestLogEntry> logList)
            {
                logList.Clear();
            }
        }
        #endregion

        #region 掃描機
        private void InitScannerSerialPort()
        {
            // 建立管理器
            scannerSerialManager = new SerialPortManager();
            // 設定檢查間隔（可選）
            scannerSerialManager.ConnectionCheckInterval = 1000; // 每秒檢查一次
            scannerSerialManager.IsCommunicationTimeout = false; //是否啟動無通訊視為斷線
            scannerSerialManager.CommunicationTimeout = 10000;   // 10秒無通訊視為斷線

            // 註冊事件
            scannerSerialManager.MessageReceived += (message) =>
            {
                Debug.Print($"收到訊息: {message}");
                UpdateProductNumber(message);

            };

            scannerSerialManager.StatusChanged += (status, isError) =>
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        bsiScannerStatus.Caption = status;
                        bsiScannerStatus.ItemAppearance.Normal.ForeColor = isError ? Color.Red : Color.LightGreen;
                    }));
                }
                else
                {
                    bsiScannerStatus.Caption = status;
                    bsiScannerStatus.ItemAppearance.Normal.ForeColor = isError ? Color.Red : Color.LightGreen;
                }
            };

            // 連線中斷事件
            scannerSerialManager.ConnectionLost += () =>
            {
                Debug.Print("連線已中斷！");
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        bsiScannerStatus.Caption = "掃描器連線中斷";
                        bsiScannerStatus.ItemAppearance.Normal.ForeColor = Color.Red;
                    }));
                }
            };
            // 連線中斷事件 - 簡單重連
            scannerSerialManager.ConnectionLost += async () =>
            {
                Debug.Print("連線已中斷！");
                // 持續重試直到成功
                await ReconnectScannerAsync();
            };

            scannerSerialManager.Initialize(
                portName: AppSetting.Default.scanner_comport_value,
                baudRate: int.Parse(AppSetting.Default.scanner_baudrate_value),
                dataBits: int.Parse(AppSetting.Default.scanner_databits_value),
                parity: AppSetting.Default.scanner_parity_value == "ODD" ? Parity.Odd : AppSetting.Default.scanner_parity_value == "EVEN" ? Parity.Even : Parity.None,
                stopBits: AppSetting.Default.scanner_stopbits_value == "2" ? StopBits.Two : StopBits.One);
            var isOpen = scannerSerialManager.StartScanning();

            //bsiScannerStatus.Caption = "掃描器已連線";
            //bsiScannerStatus.ItemAppearance.Normal.ForeColor = Color.LightGreen;

            // 初始化連線
            TryScannerInitializeConnection();

        }

        private async void TryScannerInitializeConnection()
        {
            try
            {
                scannerSerialManager.Initialize(
                    portName: AppSetting.Default.scanner_comport_value,
                    baudRate: int.Parse(AppSetting.Default.scanner_baudrate_value),
                    dataBits: int.Parse(AppSetting.Default.scanner_databits_value),
                    parity: AppSetting.Default.scanner_parity_value == "ODD" ? Parity.Odd :
                           AppSetting.Default.scanner_parity_value == "EVEN" ? Parity.Even : Parity.None,
                    stopBits: AppSetting.Default.scanner_stopbits_value == "2" ? StopBits.Two : StopBits.One);

                var isOpen = scannerSerialManager.StartScanning();

                if (isOpen)
                {
                    // 連線成功
                    bsiScannerStatus.Caption = "掃描器連線正常";
                    bsiScannerStatus.ItemAppearance.Normal.ForeColor = Color.LightGreen;
                }
                else
                {
                    // 初始連線失敗，開始重試
                    bsiScannerStatus.Caption = "掃描器連線中斷";
                    bsiScannerStatus.ItemAppearance.Normal.ForeColor = Color.Red;

                    Debug.Print("初始連線失敗，開始自動重連");
                    await ReconnectScannerAsync();
                }
            }
            catch (Exception ex)
            {
                // 初始化錯誤，也開始重試
                Debug.Print($"初始化失敗: {ex.Message}");
                bsiScannerStatus.Caption = $"初始化失敗: {ex.Message}";
                bsiScannerStatus.ItemAppearance.Normal.ForeColor = Color.Red;

                await ReconnectScannerAsync();
            }
        }

        private async Task<bool> ReconnectScannerAsync()
        {
            int attemptCount = 0;

            while (true) // 無限重試
            {
                attemptCount++;

                UpdateScannerStatus($"掃描器連線中斷", false);
                Debug.Print($"嘗試重新連線... (第 {attemptCount} 次)");

                // 等待 2 秒後重試
                await Task.Delay(2000);

                try
                {
                    scannerSerialManager.StopScanning();
                    scannerSerialManager.Close();

                    // 重新初始化
                    scannerSerialManager.Initialize(
                        portName: AppSetting.Default.scanner_comport_value,
                        baudRate: int.Parse(AppSetting.Default.scanner_baudrate_value),
                        dataBits: int.Parse(AppSetting.Default.scanner_databits_value),
                        parity: AppSetting.Default.scanner_parity_value == "ODD" ? Parity.Odd :
                               AppSetting.Default.scanner_parity_value == "EVEN" ? Parity.Even : Parity.None,
                        stopBits: AppSetting.Default.scanner_stopbits_value == "2" ? StopBits.Two : StopBits.One);

                    if (scannerSerialManager.StartScanning())
                    {
                        UpdateScannerStatus("掃描器連線正常", true);
                        Debug.Print($"重新連線成功！(共嘗試 {attemptCount} 次)");
                        return true; // 成功
                    }
                    else
                    {
                        Debug.Print($"第 {attemptCount} 次重連失敗：無法開始掃描");
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print($"第 {attemptCount} 次重連錯誤: {ex.Message}");
                }

                // 如果重試次數過多，可以增加延遲時間
                if (attemptCount % 10 == 0)
                {
                    Debug.Print($"已重試 {attemptCount} 次，暫停 10 秒...");
                    await Task.Delay(10000); // 每 10 次失敗後暫停 10 秒
                }
            }
        }

        private void UpdateScannerStatus(string message, bool isConnected)
        {
            // 確保在 UI 執行緒中執行
            if (InvokeRequired)
            {
                Invoke(new Action<string, bool>(UpdateScannerStatus), message, isConnected);
                return;
            }

            bsiScannerStatus.Caption = message;
            bsiScannerStatus.ItemAppearance.Normal.ForeColor = isConnected ? Color.LightGreen : Color.LightPink;

            Debug.Print($"掃描器狀態: {message}");
        }
        private void UpdateProductNumber(string number)
        {
            // 確保在 UI 執行緒中執行
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateProductNumber), number);
                return;
            }
            GetTestStep(); //獲取測試資訊
            workOder_Number_Value.EditValue = number;
            //bsiScannerStatus.ItemAppearance.Normal.ForeColor = isConnected ? Color.LightGreen : Color.LightPink;

            //Debug.Print($"掃描器狀態: {message}");
        }
        #endregion

        #region 治具
        private void InitJigSerialPort()
        {
            // 建立管理器
            jigSerialManager = new SerialPortManager();
            // 設定檢查間隔（可選）
            jigSerialManager.ConnectionCheckInterval = 1000; // 每秒檢查一次
            jigSerialManager.IsCommunicationTimeout = false; //是否啟動無通訊視為斷線
            jigSerialManager.CommunicationTimeout = 10000;   // 10秒無通訊視為斷線

            // 註冊事件
            jigSerialManager.MessageReceived += async (message) =>
            {

                var parsedMessage = JigUtlis.ParseMessage(message);
                if (parsedMessage != null)
                {
                    //Debug.Print($"版本: {parsedMessage.Version}");
                    //Debug.Print($"按鈕狀態: {parsedMessage.Button}");

                    foreach (var channel in parsedMessage.Channels)
                    {
                        Debug.Print($"通道 {channel.ID}: Vbus={channel.Vbus}, Vload={channel.Vload}, I={channel.I}, P={channel.P}");
                    }
                    GlobalSettings.Jig = parsedMessage;

                    // 如果需要執行 UI 相關操作，切換到 UI 執行緒
                    if (parsedMessage.Button)
                    {
                        // 切換到 UI 執行緒執行
                        if (InvokeRequired)
                        {
                            Invoke(new Action(async () =>
                            {
                                await ExecutePowerSupplyCommandsAsync();
                            }));
                        }
                        else
                        {
                            await ExecutePowerSupplyCommandsAsync();
                        }
                    }
                }
                else
                {
                    Debug.Print("解析訊息失敗");
                }

            };

            jigSerialManager.StatusChanged += (status, isError) =>
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        bsiJigStatus.Caption = status;
                        bsiJigStatus.ItemAppearance.Normal.ForeColor = isError ? Color.Red : Color.LightGreen;
                    }));
                }
                else
                {
                    bsiJigStatus.Caption = status;
                    bsiJigStatus.ItemAppearance.Normal.ForeColor = isError ? Color.Red : Color.LightGreen;
                }
            };

            // 連線中斷事件
            jigSerialManager.ConnectionLost += () =>
            {
                Debug.Print("連線已中斷！");
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        bsiJigStatus.Caption = "治具連線中斷";
                        bsiJigStatus.ItemAppearance.Normal.ForeColor = Color.Red;
                    }));
                }
            };
            // 連線中斷事件 - 簡單重連
            jigSerialManager.ConnectionLost += async () =>
            {
                Debug.Print("連線已中斷！");
                // 持續重試直到成功
                await ReconnectJigAsync();
            };

            jigSerialManager.Initialize(
                portName: AppSetting.Default.jig_comport_value,
                baudRate: int.Parse(AppSetting.Default.jig_baudrate_value),
                dataBits: int.Parse(AppSetting.Default.jig_databits_value),
                parity: AppSetting.Default.jig_parity_value == "ODD" ? Parity.Odd : AppSetting.Default.jig_parity_value == "EVEN" ? Parity.Even : Parity.None,
                stopBits: AppSetting.Default.jig_stopbits_value == "2" ? StopBits.Two : StopBits.One, isDTR: true);
            var isOpen = jigSerialManager.StartScanning();

            // 初始化連線
            TryJigInitializeConnection();

        }

        private async void TryJigInitializeConnection()
        {
            try
            {
                jigSerialManager.Initialize(
                    portName: AppSetting.Default.jig_comport_value,
                    baudRate: int.Parse(AppSetting.Default.jig_baudrate_value),
                    dataBits: int.Parse(AppSetting.Default.jig_databits_value),
                    parity: AppSetting.Default.jig_parity_value == "ODD" ? Parity.Odd :
                           AppSetting.Default.jig_parity_value == "EVEN" ? Parity.Even : Parity.None,
                    stopBits: AppSetting.Default.jig_stopbits_value == "2" ? StopBits.Two : StopBits.One, isDTR: true);

                var isOpen = jigSerialManager.StartScanning();

                if (isOpen)
                {
                    // 連線成功
                    bsiJigStatus.Caption = "治具連線正常";
                    bsiJigStatus.ItemAppearance.Normal.ForeColor = Color.LightGreen;
                }
                else
                {
                    // 初始連線失敗，開始重試
                    bsiJigStatus.Caption = "掃描器連線中斷";
                    bsiJigStatus.ItemAppearance.Normal.ForeColor = Color.Red;

                    Debug.Print("初始連線失敗，開始自動重連");
                    await ReconnectJigAsync();
                }
            }
            catch (Exception ex)
            {
                // 初始化錯誤，也開始重試
                Debug.Print($"初始化失敗: {ex.Message}");
                bsiJigStatus.Caption = $"初始化失敗: {ex.Message}";
                bsiJigStatus.ItemAppearance.Normal.ForeColor = Color.Red;

                await ReconnectJigAsync();
            }
        }

        private async Task<bool> ReconnectJigAsync()
        {
            int attemptCount = 0;

            while (true) // 無限重試
            {
                attemptCount++;

                UpdateScannerStatus($"治具連線中斷", false);
                Debug.Print($"嘗試重新連線... (第 {attemptCount} 次)");

                // 等待 2 秒後重試
                await Task.Delay(2000);

                try
                {
                    jigSerialManager.StopScanning();
                    jigSerialManager.Close();

                    // 重新初始化
                    jigSerialManager.Initialize(
                        portName: AppSetting.Default.jig_comport_value,
                        baudRate: int.Parse(AppSetting.Default.jig_baudrate_value),
                        dataBits: int.Parse(AppSetting.Default.jig_databits_value),
                        parity: AppSetting.Default.jig_parity_value == "ODD" ? Parity.Odd :
                               AppSetting.Default.jig_parity_value == "EVEN" ? Parity.Even : Parity.None,
                        stopBits: AppSetting.Default.jig_stopbits_value == "2" ? StopBits.Two : StopBits.One, isDTR: true);

                    if (jigSerialManager.StartScanning())
                    {
                        UpdateJigStatus("治具連線正常", true);
                        Debug.Print($"重新連線成功！(共嘗試 {attemptCount} 次)");
                        return true; // 成功
                    }
                    else
                    {
                        Debug.Print($"第 {attemptCount} 次重連失敗：無法開始掃描");
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print($"第 {attemptCount} 次重連錯誤: {ex.Message}");
                }

                // 如果重試次數過多，可以增加延遲時間
                if (attemptCount % 10 == 0)
                {
                    Debug.Print($"已重試 {attemptCount} 次，暫停 10 秒...");
                    await Task.Delay(10000); // 每 10 次失敗後暫停 10 秒
                }
            }
        }

        private void UpdateJigStatus(string message, bool isConnected)
        {
            // 確保在 UI 執行緒中執行
            if (InvokeRequired)
            {
                Invoke(new Action<string, bool>(UpdateJigStatus), message, isConnected);
                return;
            }

            bsiJigStatus.Caption = message;
            bsiJigStatus.ItemAppearance.Normal.ForeColor = isConnected ? Color.LightGreen : Color.LightPink;

            Debug.Print($"掃描器狀態: {message}");
        }
        private void UpdateJigNumber(string number)
        {
            // 確保在 UI 執行緒中執行
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateProductNumber), number);
                return;
            }

            workOder_Number_Value.EditValue = number;
            //bsiScannerStatus.ItemAppearance.Normal.ForeColor = isConnected ? Color.LightGreen : Color.LightPink;

            //Debug.Print($"掃描器狀態: {message}");
        }
        #endregion

        #region 開始測試
        private async void btn_start_Click(object sender, EventArgs e)
        {
            try
            {
                await ExecutePowerSupplyCommandsAsync();
            }
            finally
            {
                btn_start.Enabled = true;
            }
        }
        private async Task ExecutePowerSupplyCommandsAsync()
        {
            if(ProductionTestStep != null)
            {
                isFail = true;
                status.Text = "測試中";
                status.AppearanceItemCaption.BackColor = Color.LightSkyBlue;
                ClearDataPoints(ch1_chart);
                ClearDataPoints(ch2_chart);
                ClearDataPoints(ch3_chart);
                ClearDataPoints(ch4_chart);
                ClearTestLog();
                jigSerialManager.SendLine("testing");
                using var ps = new Gpp2323Serial("COM4", 115200);
                ps.Open();

                foreach (var step in ProductionTestStep.Step)
                {
                    ps.SetOutput(1, step.Voltage, step.Current);
                    await Task.Delay(500);
                    UpdateDataPoint(step.ID, step.ID);
                }
                ps.TurnOff(1);
                jigSerialManager.SendLine("ready");
                if (!isFail)
                {
                    status.Text = "未通過";
                    status.AppearanceItemCaption.BackColor = Color.LightPink;
                }
                else
                {
                    status.Text = "通過";
                    status.AppearanceItemCaption.BackColor = Color.LightGreen;
                }
               
            }
        }
        private void UpdateDataPoint(int id,int step)
        {
            // 確保在 UI 執行緒中執行
            if (InvokeRequired)
            {
                Invoke(new Action<int, int>(UpdateDataPoint), id, step);
                return;
            }
            var LogContent = string.Empty;
            for (int ch = 1; ch <= 4; ch++)
            {
                var data = GlobalSettings.Jig.Channels.Find(r => r.ID == ch);
                var channel = ProductionTestStep.Step.Find(r => r.ID == step).Channel.Find(r => r.ID == ch);

                switch (ch)
                {
                    case 1:
                        LogContent += UpdateLog(ch1_chart, ch1_Current_Value,data, channel);
                        break;
                    case 2:
                        LogContent += UpdateLog(ch2_chart, ch2_Current_Value, data, channel);
                        break;
                    case 3:
                        LogContent += UpdateLog(ch3_chart, ch3_Current_Value, data, channel);
                        break;
                    case 4:
                        LogContent += UpdateLog(ch4_chart, ch4_Current_Value, data, channel);
                        break;
                }
            }
            AddTestLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), id.ToString(), LogContent);

        }
        private string UpdateLog(ChartControl chart, SimpleLabelItem currentLabel, Models.Jig.Channel data, Models.Test.Channel channel)
        {
            var result = string.Empty;
            if (data.I <= 50)
            {
                AddDataPoint(chart, 0, data.Vbus);
                currentLabel.Text = "0";
                result += $"通道:{channel.ID} 量測值:0 測試結果:未量測到負載";
            }
            else
            {
                AddDataPoint(chart, data.I, data.Vbus);
                currentLabel.Text = data.I.ToString();
                if (data.I > channel.HH && channel.HH != 0)
                {
                    isFail = false;
                    result += $"通道:{channel.ID} 量測值:{data.I} 測試結果:未通過(高高限 限值:{channel.HH})";
                }
                else if (data.I > channel.H && channel.H != 0)
                {
                    isFail = false;
                    result += $"通道:{channel.ID} 量測值:{data.I} 測試結果:未通過(高限 限值:{channel.H})";
                }
                else if (data.I < channel.LL && channel.LL != 0)
                {
                    isFail = false;
                    result += $"通道:{channel.ID} 量測值:{data.I} 測試結果:未通過(低低限 限值:{channel.LL})";
                }
                else if (data.I < channel.L && channel.LL != 0)
                {
                    isFail = false;
                    result += $"通道:{channel.ID} 量測值:{data.I} 測試結果:未通過(低限 限值:{channel.L})";
                }
                else
                {
                    result += $"通道:{channel.ID} 量測值:{data.I} 測試結果:通過";
                }
            }
            if(channel.ID != 4)
            {
                result += "\r\n";
            }
            return result;
        }
        #endregion


    }

}