using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Neurons.P.LeoLedtech.Test.TotalPower.BasicUtlis
{
    public class ComPortUtlis
    {


        /// <summary>
        /// 打開通訊埠
        /// </summary>
        /// <param name="serialPort"></param>
        /// <param name="dataBuffer"></param>
        /// <returns></returns>
        public static (bool isOpen, string message) OpenSerialPort(SerialPort serialPort, StringBuilder dataBuffer)
        {
            try
            {
                if (serialPort != null && !serialPort.IsOpen)
                {
                    serialPort.Open();
                    //UpdateStatus($"串列埠 {serialPort.PortName} 已開啟", false);

                    // 清除緩衝區
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                    dataBuffer.Clear();
                    return (true, "開啟串列埠成功");
                    //return true;
                }
            }
            catch (Exception ex)
            {
                return (false, $"開啟串列埠失敗: {ex.Message}");
            }
            return (false, "開啟串列埠:未知錯誤");
        }
        /// <summary>
        /// 關閉通訊埠
        /// </summary>
        /// <param name="serialPort"></param>
        /// <returns></returns>
        public static (bool isClose, string message) CloseSerialPort(SerialPort serialPort)
        {
            try
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                    return (true, "串列埠已關閉");
                }
            }
            catch (Exception ex)
            {
                return (false, $"關閉串列埠錯誤: {ex.Message}");
            }
            return (false, $"關閉串列埠:未知錯誤");
        }
        // 資料接收事件（即時觸發）
        //private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e, DevExpress.XtraEditors.XtraForm from)
        //{
        //    // 注意：這個事件在背景執行緒中觸發
        //    // 如果需要更新 UI，必須使用 Invoke

        //    try
        //    {
        //        SerialPort sp = (SerialPort)sender;
        //        string data = sp.ReadExisting();

        //        if (!string.IsNullOrEmpty(data))
        //        {
        //            // 使用 Invoke 確保在 UI 執行緒中執行
        //            from.Invoke(new Action(() => ProcessReceivedData(data)));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        from.Invoke(new Action(() => UpdateStatus($"資料接收錯誤: {ex.Message}", true)));
        //    }
        //}
    }
}
