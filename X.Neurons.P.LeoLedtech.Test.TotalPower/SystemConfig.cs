using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Neurons.P.LeoLedtech.Test.TotalPower.BasicUtlis;

namespace X.Neurons.P.LeoLedtech.Test.TotalPower
{
    public partial class SystemConfig : DevExpress.XtraEditors.XtraForm
    {
        public SystemConfig()
        {
            InitializeComponent();
            serverIP_value.EditValue = AppSetting.Default.serverIP;

        }
        private void SystemConfig_Load(object sender, EventArgs e)
        {
            InitJigSerialCombobox();
            InitJiaSerialComPort();
            InitPowerSupplySerialCombobox();
            InitPowerSupplySerialComPort();
            InitScannerSerialCombobox();
            InitScannerSerialComPort();
        }
        #region 系統設定頁面
        private void btn_system_setting_save_Click(object sender, EventArgs e)
        {
            if (IPAddress.TryParse(serverIP_value.EditValue.ToString(), out IPAddress ip))
            {
                AppSetting.Default.serverIP = serverIP_value.EditValue.ToString();
                AppSetting.Default.Save();
                GlobalSettings.ServerApiClient = new ApiClient($"http://{AppSetting.Default.serverIP}:12500");
                XtraMessageBox.Show("保存成功", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                XtraMessageBox.Show("請輸入正確的IP位置", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion
        #region 治具設定頁面
        private void btn_reComPortScanner_Click(object sender, EventArgs e)
        {
            InitJiaSerialComPort();
        }
        private void btn_jig_setting_save_Click(object sender, EventArgs e)
        {
            AppSetting.Default.jig_baudrate_value = cbe_jig_baudrate_value.EditValue.ToString();
            AppSetting.Default.jig_parity_value = cbe_jig_parity_value.EditValue.ToString();
            AppSetting.Default.jig_databits_value = cbe_jig_databits_value.EditValue.ToString();
            AppSetting.Default.jig_stopbits_value = cbe_jig_stopbits_value.EditValue.ToString();
            AppSetting.Default.jig_comport_value = cbe_jig_comport_value.EditValue.ToString();
            AppSetting.Default.Save();
            XtraMessageBox.Show("保存成功", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void InitJigSerialCombobox()
        {
            cbe_jig_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("110", "110"));
            cbe_jig_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("300", "300"));
            cbe_jig_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("600", "600"));
            cbe_jig_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("1200", "1200"));
            cbe_jig_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("2400", "2400"));
            cbe_jig_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("4800", "4800"));
            cbe_jig_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("9600", "9600"));
            cbe_jig_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("14400", "14400"));
            cbe_jig_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("19200", "19200"));
            cbe_jig_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("38400", "38400"));
            cbe_jig_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("56000", "56000"));
            cbe_jig_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("57600", "57600"));
            cbe_jig_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("115200", "115200"));
            cbe_jig_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("128000", "128000"));
            cbe_jig_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("256000", "256000"));

            cbe_jig_parity_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("ODD", "ODD"));
            cbe_jig_parity_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("EVEN", "EVEN"));
            cbe_jig_parity_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("NONE", "NONE"));

            cbe_jig_databits_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("7", "7"));
            cbe_jig_databits_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("8", "8"));

            cbe_jig_stopbits_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("1", "1"));
            cbe_jig_stopbits_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("2", "2"));

            cbe_jig_baudrate_value.EditValue = AppSetting.Default.jig_baudrate_value;
            cbe_jig_parity_value.EditValue = AppSetting.Default.jig_parity_value;
            cbe_jig_databits_value.EditValue = AppSetting.Default.jig_databits_value;
            cbe_jig_stopbits_value.EditValue = AppSetting.Default.jig_stopbits_value;
        }
        private void InitJiaSerialComPort()
        {
            cbe_jig_comport_value.Properties.Items.Clear();
            var ports = ComPortScanner.GetAllComPortsWithFullInfo();
            cbe_jig_comport_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("未選擇", "none"));
            foreach (var port in ports)
            {
                cbe_jig_comport_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(port.FriendlyName, port.PortName));
            }
            if (ports.Any(r => r.PortName == AppSetting.Default.jig_comport_value) || AppSetting.Default.jig_comport_value == "none")
            {
                cbe_jig_comport_value.EditValue = AppSetting.Default.jig_comport_value;
            }
            else
            {
                cbe_jig_comport_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem($"{AppSetting.Default.jig_comport_value}(原始設定，設備尚未連接)", AppSetting.Default.jig_comport_value));
                cbe_jig_comport_value.EditValue = AppSetting.Default.jig_comport_value;
            }
        }
        #endregion
        #region 電源供應器設定頁面
        private void btn_powersupply_reComPortScanner_Click(object sender, EventArgs e)
        {
            InitPowerSupplySerialComPort();
        }

        private void btn_powersupply_setting_save_Click(object sender, EventArgs e)
        {
            AppSetting.Default.powersupply_baudrate_value = cbe_powersupply_baudrate_value.EditValue.ToString();
            AppSetting.Default.powersupply_parity_value = cbe_powersupply_parity_value.EditValue.ToString();
            AppSetting.Default.powersupply_databits_value = cbe_powersupply_databits_value.EditValue.ToString();
            AppSetting.Default.powersupply_stopbits_value = cbe_powersupply_stopbits_value.EditValue.ToString();
            AppSetting.Default.powersupply_comport_value = cbe_powersupply_comport_value.EditValue.ToString();
            AppSetting.Default.Save();
            XtraMessageBox.Show("保存成功", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void InitPowerSupplySerialCombobox()
        {
            cbe_powersupply_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("110", "110"));
            cbe_powersupply_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("300", "300"));
            cbe_powersupply_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("600", "600"));
            cbe_powersupply_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("1200", "1200"));
            cbe_powersupply_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("2400", "2400"));
            cbe_powersupply_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("4800", "4800"));
            cbe_powersupply_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("9600", "9600"));
            cbe_powersupply_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("14400", "14400"));
            cbe_powersupply_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("19200", "19200"));
            cbe_powersupply_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("38400", "38400"));
            cbe_powersupply_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("56000", "56000"));
            cbe_powersupply_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("57600", "57600"));
            cbe_powersupply_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("115200", "115200"));
            cbe_powersupply_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("128000", "128000"));
            cbe_powersupply_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("256000", "256000"));

            cbe_powersupply_parity_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("ODD", "ODD"));
            cbe_powersupply_parity_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("EVEN", "EVEN"));
            cbe_powersupply_parity_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("NONE", "NONE"));

            cbe_powersupply_databits_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("7", "7"));
            cbe_powersupply_databits_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("8", "8"));

            cbe_powersupply_stopbits_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("1", "1"));
            cbe_powersupply_stopbits_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("2", "2"));

            cbe_powersupply_baudrate_value.EditValue = AppSetting.Default.powersupply_baudrate_value;
            cbe_powersupply_parity_value.EditValue = AppSetting.Default.powersupply_parity_value;
            cbe_powersupply_databits_value.EditValue = AppSetting.Default.powersupply_databits_value;
            cbe_powersupply_stopbits_value.EditValue = AppSetting.Default.powersupply_stopbits_value;
        }
        private void InitPowerSupplySerialComPort()
        {
            cbe_powersupply_comport_value.Properties.Items.Clear();
            var ports = ComPortScanner.GetAllComPortsWithFullInfo();
            cbe_powersupply_comport_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("未選擇", "none"));
            foreach (var port in ports)
            {
                cbe_powersupply_comport_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(port.FriendlyName, port.PortName));
            }
            if (ports.Any(r => r.PortName == AppSetting.Default.powersupply_comport_value) || AppSetting.Default.powersupply_comport_value == "none")
            {
                cbe_powersupply_comport_value.EditValue = AppSetting.Default.powersupply_comport_value;
            }
            else
            {
                cbe_powersupply_comport_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem($"{AppSetting.Default.powersupply_comport_value}(原始設定，設備尚未連接)", AppSetting.Default.powersupply_comport_value));
                cbe_powersupply_comport_value.EditValue = AppSetting.Default.powersupply_comport_value;
            }
        }
        #endregion
        #region 掃描器設定
        private void btn_scanner_reComPortScanner_Click(object sender, EventArgs e)
        {
            InitScannerSerialComPort();
        }

        private void btn_scanner_setting_save_Click(object sender, EventArgs e)
        {
            AppSetting.Default.scanner_baudrate_value = cbe_scanner_baudrate_value.EditValue.ToString();
            AppSetting.Default.scanner_parity_value = cbe_scanner_parity_value.EditValue.ToString();
            AppSetting.Default.scanner_databits_value = cbe_scanner_databits_value.EditValue.ToString();
            AppSetting.Default.scanner_stopbits_value = cbe_scanner_stopbits_value.EditValue.ToString();
            AppSetting.Default.scanner_comport_value = cbe_scanner_comport_value.EditValue.ToString();
            AppSetting.Default.Save();
            XtraMessageBox.Show("保存成功", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void InitScannerSerialCombobox()
        {
            cbe_scanner_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("110", "110"));
            cbe_scanner_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("300", "300"));
            cbe_scanner_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("600", "600"));
            cbe_scanner_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("1200", "1200"));
            cbe_scanner_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("2400", "2400"));
            cbe_scanner_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("4800", "4800"));
            cbe_scanner_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("9600", "9600"));
            cbe_scanner_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("14400", "14400"));
            cbe_scanner_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("19200", "19200"));
            cbe_scanner_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("38400", "38400"));
            cbe_scanner_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("56000", "56000"));
            cbe_scanner_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("57600", "57600"));
            cbe_scanner_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("115200", "115200"));
            cbe_scanner_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("128000", "128000"));
            cbe_scanner_baudrate_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("256000", "256000"));

            cbe_scanner_parity_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("ODD", "ODD"));
            cbe_scanner_parity_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("EVEN", "EVEN"));
            cbe_scanner_parity_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("NONE", "NONE"));

            cbe_scanner_databits_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("7", "7"));
            cbe_scanner_databits_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("8", "8"));

            cbe_scanner_stopbits_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("1", "1"));
            cbe_scanner_stopbits_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("2", "2"));

            cbe_scanner_baudrate_value.EditValue = AppSetting.Default.scanner_baudrate_value;
            cbe_scanner_parity_value.EditValue = AppSetting.Default.scanner_parity_value;
            cbe_scanner_databits_value.EditValue = AppSetting.Default.scanner_databits_value;
            cbe_scanner_stopbits_value.EditValue = AppSetting.Default.scanner_stopbits_value;
        }
        private void InitScannerSerialComPort()
        {
            cbe_scanner_comport_value.Properties.Items.Clear();
            var ports = ComPortScanner.GetAllComPortsWithFullInfo();
            cbe_scanner_comport_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("未選擇", "none"));
            foreach (var port in ports)
            {
                cbe_scanner_comport_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(port.FriendlyName, port.PortName));
            }
            if (ports.Any(r => r.PortName == AppSetting.Default.scanner_comport_value) || AppSetting.Default.scanner_comport_value == "none")
            {
                cbe_scanner_comport_value.EditValue = AppSetting.Default.scanner_comport_value;
            }
            else
            {
                cbe_scanner_comport_value.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem($"{AppSetting.Default.scanner_comport_value}(原始設定，設備尚未連接)", AppSetting.Default.scanner_comport_value));
                cbe_scanner_comport_value.EditValue = AppSetting.Default.scanner_comport_value;
            }
        }
        #endregion
    }
}