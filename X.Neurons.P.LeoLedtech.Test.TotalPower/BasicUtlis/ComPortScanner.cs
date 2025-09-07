using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using X.Neurons.P.LeoLedtech.Test.TotalPower.Models;

namespace X.Neurons.P.LeoLedtech.Test.TotalPower.BasicUtlis
{
    public static class ComPortScanner
    {
        /// <summary>
        /// 方法1: 使用WMI查詢Win32_PnPEntity獲取完整名稱
        /// </summary>
        public static List<ComPortInfo> GetComPortsWithFullNames()
        {
            var comPorts = new List<ComPortInfo>();

            try
            {
                using (var searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\""))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        var name = obj["Name"]?.ToString();
                        if (!string.IsNullOrEmpty(name) && name.Contains("COM"))
                        {
                            // 使用正則表達式提取COM端口號
                            var match = Regex.Match(name, @"COM(\d+)");
                            if (match.Success)
                            {
                                var portName = match.Value; // COM4
                                var description = name.Replace($"({portName})", "").Trim(); // GPP Serial

                                var portInfo = new ComPortInfo
                                {
                                    PortName = portName,
                                    FriendlyName = name,  // GPP Serial (COM4)
                                    Description = description,  // GPP Serial
                                    DeviceInstanceId = obj["DeviceID"]?.ToString(),
                                    Manufacturer = obj["Manufacturer"]?.ToString(),
                                    Service = obj["Service"]?.ToString(),
                                    IsPresent = true
                                };

                                comPorts.Add(portInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WMI查詢錯誤: {ex.Message}");
            }

            return comPorts;
        }

        /// <summary>
        /// 方法2: 通過註冊表獲取COM端口信息
        /// </summary>
        public static List<ComPortInfo> GetComPortsFromRegistry()
        {
            var comPorts = new List<ComPortInfo>();

            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM"))
                {
                    if (key != null)
                    {
                        foreach (string valueName in key.GetValueNames())
                        {
                            var portName = key.GetValue(valueName)?.ToString();
                            if (!string.IsNullOrEmpty(portName))
                            {
                                var portInfo = new ComPortInfo
                                {
                                    PortName = portName,
                                    Description = valueName,
                                    FriendlyName = $"{valueName} ({portName})"
                                };

                                comPorts.Add(portInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"註冊表查詢錯誤: {ex.Message}");
            }

            return comPorts;
        }

        /// <summary>
        /// 方法3: 結合多種方法獲取最完整的信息
        /// </summary>
        public static List<ComPortInfo> GetAllComPortsWithFullInfo()
        {
            var result = new List<ComPortInfo>();

            // 首先使用WMI獲取詳細信息
            var wmiPorts = GetComPortsWithFullNames();
            var wmiPortNames = wmiPorts.Select(p => p.PortName).ToHashSet();
            result.AddRange(wmiPorts);

            // 獲取系統報告的所有可用端口
            var availablePorts = SerialPort.GetPortNames();

            // 添加WMI沒有找到但系統報告存在的端口
            foreach (var portName in availablePorts)
            {
                if (!wmiPortNames.Contains(portName))
                {
                    result.Add(new ComPortInfo
                    {
                        PortName = portName,
                        FriendlyName = $"Unknown Device ({portName})",
                        Description = "Unknown Device",
                        IsPresent = true
                    });
                }
            }

            // 測試端口可用性
            foreach (var port in result)
            {
                port.IsAvailable = TestComPortAvailability(port.PortName);
            }

            return result.OrderBy(p => ExtractPortNumber(p.PortName)).ToList();
        }


        private static bool TestComPortAvailability(string portName)
        {
            try
            {
                using (var port = new SerialPort(portName))
                {
                    port.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private static int ExtractPortNumber(string portName)
        {
            var match = Regex.Match(portName, @"COM(\d+)");
            return match.Success ? int.Parse(match.Groups[1].Value) : 0;
        }

        /// <summary>
        /// 創建端口名稱到友好名稱的字典映射
        /// </summary>
        public static Dictionary<string, string> GetComPortNameMapping()
        {
            var mapping = new Dictionary<string, string>();
            var ports = GetComPortsWithFullNames();

            foreach (var port in ports)
            {
                mapping[port.PortName] = port.FriendlyName;
            }

            return mapping;
        }
    }

}
