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
    public partial class ServerConfig : DevExpress.XtraEditors.XtraForm
    {
        public ServerConfig()
        {
            InitializeComponent();
            serverIP_value.EditValue = AppSetting.Default.serverIP;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if(IPAddress.TryParse(serverIP_value.EditValue.ToString(),out IPAddress ip))
            {
                AppSetting.Default.serverIP = serverIP_value.EditValue.ToString();
                AppSetting.Default.Save();
                //XtraMessageBox.Show("儲存成功");
                GlobalSettings.ServerApiClient = new ApiClient($"http://{AppSetting.Default.serverIP}:12500");
                XtraMessageBox.Show("保存成功", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                XtraMessageBox.Show("請輸入正確的IP位置", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

    }
}