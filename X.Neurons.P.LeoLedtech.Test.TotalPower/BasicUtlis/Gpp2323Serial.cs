using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Neurons.P.LeoLedtech.Test.TotalPower.BasicUtlis
{
    public sealed class Gpp2323Serial : IDisposable
    {
        private readonly SerialPort _port;
        private readonly object _lock = new();

        public Gpp2323Serial(string comPort, int baudRate = 115200, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            _port = new SerialPort(comPort, baudRate, parity, dataBits, stopBits)
            {
                NewLine = "\n",        // GPP 系列以 LF 結尾
                ReadTimeout = 2000,
                WriteTimeout = 2000
            };
        }

        public void Open()
        {
            if (!_port.IsOpen) _port.Open();
        }

        public void Close()
        {
            if (_port.IsOpen) _port.Close();
        }

        public void Dispose()
        {
            try { Close(); } catch { /* ignore */ }
            _port.Dispose();
        }

        // ====== 你要的 1：設定指定 CH 的電壓/電流並開啟輸出 ======
        public void SetOutput(int ch, double voltage, double current)
        {
            ValidateChannel(ch);
            var ci = CultureInfo.InvariantCulture;
            Write($":SOURce{ch}:VOLTage {voltage.ToString(ci)}");
            Write($":SOURce{ch}:CURRent {current.ToString(ci)}");
            Write($":OUTPut{ch}:STATe ON");
        }

        // ====== 你要的 2：關閉指定 CH 的輸出 ======
        public void TurnOff(int ch)
        {
            ValidateChannel(ch);
            Write($":OUTPut{ch}:STATe OFF");
        }

        // ====== 你要的 3：讀回該 CH 的即時電壓/電流 ======
        public (double Voltage, double Current) ReadMeasurements(int ch)
        {
            ValidateChannel(ch);
            var v = double.Parse(Query($":MEASure{ch}:VOLTage?"), CultureInfo.InvariantCulture);
            var i = double.Parse(Query($":MEASure{ch}:CURRent?"), CultureInfo.InvariantCulture);
            return (v, i);
        }

        // ====== 其他實用：讀取 *IDN?（確認通訊） ======
        public string Identify() => Query("*IDN?");

        // ====== 內部工具 ======
        private void ValidateChannel(int ch)
        {
            if (ch != 1 && ch != 2)
                throw new ArgumentOutOfRangeException(nameof(ch), "Channel must be 1 or 2 for GPP-2323.");
        }

        private void Write(string cmd)
        {
            lock (_lock)
            {
                EnsureOpen();
                _port.DiscardInBuffer();
                _port.WriteLine(cmd);
            }
        }

        private string Query(string cmd)
        {
            lock (_lock)
            {
                EnsureOpen();
                _port.DiscardInBuffer();
                _port.WriteLine(cmd);
                var line = _port.ReadLine(); // 等到 LF
                return line.Trim();
            }
        }

        private void EnsureOpen()
        {
            if (!_port.IsOpen) _port.Open();
        }
    }
}
