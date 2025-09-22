using DevExpress.Utils;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraLayout;
using DevExpress.XtraRichEdit;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Net.Http;
using System.Text;
using X.Neurons.P.LeoLedtech.Test.TotalPower.BasicUtlis;
using X.Neurons.P.LeoLedtech.Test.TotalPower.Models;
using X.Neurons.P.LeoLedtech.Test.TotalPower.Models.AddOrder;

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

        private List<bool> ch1IsFail = new List<bool>();
        private List<bool> ch2IsFail = new List<bool>();
        private List<bool> ch3IsFail = new List<bool>();
        private List<bool> ch4IsFail = new List<bool>();

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
            //string imgFolder = Path.Combine(Application.StartupPath, "img");
            string imgFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img");
            BasicUtlis.SvgLoader.LoadFolder(svgImageCollection1, imgFolder, recursive: true, clearExisting: true);
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
            series.ToolTipPointPattern = "電壓: {A:F2} V\n電流: {V:F2} mA";
            chart.CrosshairEnabled = DefaultBoolean.True;
            series.CrosshairLabelPattern = "電壓: {A:F2} V\n電流: {V:F2} mA";

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
        private async void GetTestStep(string productNumber)
        {
            var t = await GlobalSettings.ServerApiClient.GetAsync<Models.Test.TestStep>($"/TestStep/{productNumber}");
            try
            {
                if (int.Parse(t.Guid) >= 1)
                {
                    Test_Number_Value.EditValue = productNumber;
                    ProductionTestStep = t;
                    ClearTestStep();
                    foreach (var cable in t.Cable)
                    {
                        switch (cable.Channel)
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

            }
            catch (Exception ex)
            {

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
        private readonly RichEditDocumentServer _rtfServer = new RichEditDocumentServer();

        private void InitTestLogGrid()
        {
            if (gcTestLog.DataSource == null)
                gcTestLog.DataSource = new BindingList<TestLogEntry>();

            //gvTestLog.PopulateColumns();

            gvTestLog.OptionsBehavior.Editable = false;
            gvTestLog.OptionsView.RowAutoHeight = true;        // 多行會自動撐高
            gvTestLog.OptionsView.ColumnAutoWidth = true;
            gvTestLog.OptionsView.ShowGroupPanel = false;
            gvTestLog.OptionsSelection.EnableAppearanceFocusedCell = false;

            var colContent = gvTestLog.Columns["Content"];
            if (colContent != null)
            {
                var repo = new RepositoryItemRichTextEdit { ReadOnly = true };
                colContent.ColumnEdit = repo;
                // 用 RichText 就不需要再手動設 WordWrap，會自動處理換行/樣式
            }
        }

        private void AddTestLog(string dateTime, string id, string contentHtml)
        {
            // 這裡 contentHtml 已經是含顏色/換行的 HTML（不要再 HtmlEncode）
            string fullHtml = $"<html><body style='font-family:Segoe UI; font-size:9pt'>{contentHtml}</body></html>";

            _rtfServer.HtmlText = fullHtml;
            string rtf = _rtfServer.RtfText;

            var list = (BindingList<TestLogEntry>)gcTestLog.DataSource;
            list.Add(new TestLogEntry
            {
                DateTime = dateTime,
                ID = id,
                Content = rtf   // Content 直接存 RTF（Grid 用 RepositoryItemRichTextEdit 顯示）
            });

            // 自動捲到最後
            gvTestLog.FocusedRowHandle = gvTestLog.RowCount - 1;
            gvTestLog.MakeRowVisible(gvTestLog.FocusedRowHandle);
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
            GetTestStep(number); //獲取測試資訊

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
        private Request_AddTestOrder addTestOrder = new Request_AddTestOrder();
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
            if (ProductionTestStep != null)
            {
                ch1IsFail = new List<bool>();
                ch2IsFail = new List<bool>();
                ch3IsFail = new List<bool>();
                ch4IsFail = new List<bool>();
                addTestOrder = new Request_AddTestOrder();
                addTestOrder.Head = new Request_TestOrderHead();
                addTestOrder.Body = new List<Request_TestOrderBody>();
                addTestOrder.WorkOrderBodyId = Test_Number_Value.EditValue.ToString();
                addTestOrder.Head.CreateDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                addTestOrder.Head.Station = "2";
                addTestOrder.Head.TestUser = 1;
                addTestOrder.Head.TestModel = int.Parse(ProductionTestStep.Guid);
                isFail = true;
                status.Text = "測試中";
                status.AppearanceItemCaption.BackColor = Color.LightSkyBlue;
                ClearDataPoints(ch1_chart);
                ClearDataPoints(ch2_chart);
                ClearDataPoints(ch3_chart);
                ClearDataPoints(ch4_chart);
                ClearTestLog();
                jigSerialManager.SendLine("led,0,0,0,0,0");
                await Task.Delay(50);
                jigSerialManager.SendLine("testing");
                await Task.Delay(50);
                jigSerialManager.SendLine("beeoff");
                using var ps = new Gpp2323Serial($"{AppSetting.Default.powersupply_comport_value}", 115200);
                ps.Open();
                int s = 1;
                foreach (var step in ProductionTestStep.Step)
                {
                    ps.SetOutput(1, step.Voltage, step.Current);
                    await Task.Delay(500);
                    AddBody(s, step.Voltage, step.Current);
                    UpdateDataPoint(step.ID, step.ID);
                    await Task.Delay(250);
                    s++;
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
                if (addTestOrder.Body.Any(r => r.IsPass == false))
                {
                    addTestOrder.Head.IsPass = false;
                }
                else
                {
                    addTestOrder.Head.IsPass = true;
                }
                // R B G
                if (ch1IsFail.Any(r => r == false))
                {
                    jigSerialManager.SendLine("led,1,0,255,0,1");
                }
                await Task.Delay(50);
                if (ch2IsFail.Any(r => r == false))
                {
                    jigSerialManager.SendLine("led,2,255,0,0,1");
                }
                await Task.Delay(50);
                if (ch3IsFail.Any(r => r == false))
                {
                    jigSerialManager.SendLine("led,3,255,0,255,1");
                }
                await Task.Delay(50);
                if (ch4IsFail.Any(r => r == false))
                {
                    jigSerialManager.SendLine("led,4,0,0,255,1");
                }
                if (!isFail)
                {
                    jigSerialManager.SendLine("beeon");
                }
                else
                {
                    jigSerialManager.SendLine("beeoff");
                }
                var t = await GlobalSettings.ServerApiClient.PostAsync<Request_AddTestOrder, Response_Result>("/TestOder/AddTestOrder", addTestOrder);
                //Console.WriteLine();
            }
        }
        private void UpdateDataPoint(int id, int step)
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
                var channel = ProductionTestStep.Step.Find(r => r.ID == step).Channel.Find(r => r.ChannelNumber == ch);

                switch (ch)
                {
                    case 1:
                        LogContent += UpdateLog(ch1_chart, ch1_Current_Value, data, channel);
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
        private string UpdateLog(ChartControl chart, SimpleLabelItem currentLabel,
                         Models.Jig.Channel data, Models.Test.Channel channel)
        {
            var sb = new StringBuilder();

            if (data.I <= 0.01)
            {
                // X=電壓，Y=電流（依你先前設定）
                AddDataPoint(chart, data.Vbus, 0);
                currentLabel.Text = "0";

                sb.Append($"<span style=\"color:#FF8C00;\">通道:{channel.ChannelNumber} 量測值:0 測試結果:" +
                          $"未量測到負載</span>");
            }
            else
            {
                AddDataPoint(chart, data.Vbus, data.I);
                currentLabel.Text = data.I.ToString();

                if (data.I > channel.HH && channel.HH != 0)
                {
                    isFail = false; // 你的原本邏輯保留（若要相反請自行調整）
                    sb.Append($"<span style=\"color:#FF0000;\">通道:{channel.ChannelNumber} 量測值:{data.I} 測試結果:" +
                              $"未通過(高高限 限值:{channel.HH})</span>");
                }
                else if (data.I > channel.H && channel.H != 0)
                {
                    isFail = false;
                    sb.Append($"<span style=\"color:#FF0000;\">通道:{channel.ChannelNumber} 量測值:{data.I} 測試結果:" +
                              $"未通過(高限 限值:{channel.H})</span>");
                }
                else if (data.I < channel.LL && channel.LL != 0)
                {
                    isFail = false;
                    sb.Append($"<span style=\"color:#FF0000;\">通道:{channel.ChannelNumber} 量測值:{data.I} 測試結果:" +
                              $"未通過(低低限 限值:{channel.LL})</span>");
                }
                else if (data.I < channel.L && channel.L != 0) // ← 這裡用 channel.L（修正你之前的 LL）
                {
                    isFail = false;
                    sb.Append($"<span style=\"color:#FF0000;\">通道:{channel.ChannelNumber} 量測值:{data.I} 測試結果:" +
                              $"未通過(低限 限值:{channel.L})</span>");
                }
                else
                {
                    sb.Append($"<span style=\"color:#008000;\">通道:{channel.ChannelNumber} 量測值:{data.I} 測試結果:" +
                              $"通過</span>");
                }
            }

            // 多通道時加換行（RichText/HTML 用 <br>）
            if (channel.ChannelNumber != 4)
                sb.Append("<br>");

            return sb.ToString();
        }
        private void AddBody(int step, double outputVoltage, double outputCurrent)
        {
            var body = new Request_TestOrderBody
            {
                DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Step = step,
                Voltage = outputVoltage,
                Current = outputCurrent,
                Channels = new List<Request_TestOrderChannel>()
            };

            List<bool> isChannelFail = new List<bool>();
            for (int ch = 1; ch <= 4; ch++)
            {
                var rowData = GlobalSettings.Jig.Channels.Find(r => r.ID == ch);
                var channelLimit = ProductionTestStep.Step.Find(r => r.ID == step).Channel.Find(r => r.ChannelNumber == ch);
                var channelResult = new Request_TestOrderChannel();
                channelResult.Channel = ch;
                channelResult.Voltage = rowData.Vbus;
                channelResult.Current = rowData.I;
                channelResult.Power = rowData.P;
                if (rowData.I > channelLimit.HH && channelLimit.HH != 0)
                {
                    isChannelFail.Add(false);
                    channelFail(ch, false);
                }
                else if (rowData.I > channelLimit.H && channelLimit.H != 0)
                {
                    isChannelFail.Add(false);
                    channelFail(ch, false);
                }
                else if (rowData.I < channelLimit.LL && channelLimit.LL != 0)
                {
                    isChannelFail.Add(false);
                    channelFail(ch, false);
                }
                else if (rowData.I < channelLimit.L && channelLimit.LL != 0)
                {
                    isChannelFail.Add(false);
                    channelFail(ch, false);
                }
                else
                {
                    isChannelFail.Add(true);
                    channelFail(ch, true);
                }


                body.Channels.Add(channelResult);
            }
            if (isChannelFail.Any(r => r == false))
            {
                body.IsPass = false;
            }
            else
            {
                body.IsPass = true;

            }
            addTestOrder.Body.Add(body);
        }
        private void channelFail(int ch, bool status)
        {
            switch (ch)
            {
                case 1:
                    ch1IsFail.Add(status);
                    break;
                case 2:
                    ch2IsFail.Add(status);
                    break;
                case 3:
                    ch3IsFail.Add(status);
                    break;
                case 4:
                    ch4IsFail.Add(status);
                    break;
            }
        }
        #endregion

        private void btn_DisableBizz_Click(object sender, EventArgs e)
        {
            jigSerialManager.SendLine("beeoff");
        }
    }

}