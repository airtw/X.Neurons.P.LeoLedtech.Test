using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Neurons.P.LeoLedtech.Test.TotalPower.Models
{
    public class ComPortInfo
    {
        /// <summary>
        /// COM端口名稱，如 "COM1", "COM4" 等
        /// 用途：程式連接串口時使用的端口標識符
        /// 來源：SerialPort.GetPortNames() 或 WMI查詢結果解析
        /// </summary>
        public string PortName { get; set; }

        /// <summary>
        /// 系統顯示的完整友好名稱，如 "GPP Serial (COM4)"
        /// 用途：在用戶界面顯示，讓用戶容易識別設備
        /// 來源：WMI Win32_PnPEntity.Name 屬性
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// 設備描述部分（去掉端口號），如 "GPP Serial"
        /// 用途：識別設備類型，用於設備篩選和分類
        /// 來源：從FriendlyName解析得出，移除 "(COMx)" 部分
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Windows設備實例ID，唯一標識硬體設備
        /// 用途：設備管理、驅動程式匹配、硬體追蹤
        /// 來源：WMI Win32_PnPEntity.DeviceID 屬性
        /// 範例：USB\VID_10C4&PID_EA60\0001
        /// </summary>
        public string DeviceInstanceId { get; set; }

        /// <summary>
        /// 設備製造商名稱
        /// 用途：顯示硬體廠商信息，用於設備識別和支援
        /// 來源：WMI Win32_PnPEntity.Manufacturer 屬性
        /// 範例：Silicon Labs, FTDI, Prolific 等
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// Windows服務名稱，通常是驅動程式服務
        /// 用途：識別使用的驅動程式類型，診斷驅動問題
        /// 來源：WMI Win32_PnPEntity.Service 屬性
        /// 範例：usbser, silabser, ftdibus 等
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// 設備是否在系統中存在（已插入且被識別）
        /// 用途：判斷設備是否物理連接到系統
        /// 來源：WMI查詢能找到該設備時為true
        /// 注意：存在不等於可用，可能被其他程式佔用
        /// </summary>
        public bool IsPresent { get; set; }

        /// <summary>
        /// 端口是否可用（可以被程式打開使用）
        /// 用途：判斷端口是否可以進行串口通信
        /// 來源：嘗試打開SerialPort連接的測試結果
        /// 注意：false可能表示被其他程式佔用或硬體故障
        /// </summary>
        public bool IsAvailable { get; set; }
    }
}
