using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.Neurons.P.LeoLedtech.Test.TotalPower.Models.Jig;
using Message = X.Neurons.P.LeoLedtech.Test.TotalPower.Models.Jig.Message;

namespace X.Neurons.P.LeoLedtech.Test.TotalPower.BasicUtlis
{
    public class JigUtlis
    {
        public static Message ParseMessage(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            try
            {
                var message = new Message
                {
                    Channels = new List<Channel>()
                };

                // 以分號分割各個部分
                string[] parts = input.Split(';');

                foreach (string part in parts)
                {
                    string trimmedPart = part.Trim();

                    if (trimmedPart.StartsWith("ver:"))
                    {
                        // 解析版本號
                        message.Version = trimmedPart.Substring(4);
                    }
                    else if (trimmedPart.StartsWith("CH="))
                    {
                        // 解析通道資料
                        var channel = ParseChannel(trimmedPart);
                        if (channel != null)
                        {
                            message.Channels.Add(channel);
                        }
                    }
                    else if (trimmedPart.StartsWith("button:"))
                    {
                        // 解析按鈕狀態
                        string buttonValue = trimmedPart.Substring(7);
                        message.Button = buttonValue == "1";
                    }
                }

                return message;
            }
            catch (Exception ex)
            {
                // 可以記錄錯誤或拋出異常，根據你的需求
                System.Diagnostics.Debug.Print($"解析訊息錯誤: {ex.Message}");
                return null;
            }
        }

        private static Channel ParseChannel(string channelData)
        {
            try
            {
                // 解析格式：CH=1,Vbus=0.000,Vload=0.000,I=-0.145,P=0.000
                var channel = new Channel();

                // 以逗號分割各個參數
                string[] parameters = channelData.Split(',');

                foreach (string param in parameters)
                {
                    string trimmedParam = param.Trim();

                    if (trimmedParam.StartsWith("CH="))
                    {
                        string idStr = trimmedParam.Substring(3);
                        if (int.TryParse(idStr, out int id))
                        {
                            channel.ID = id;
                        }
                    }
                    else if (trimmedParam.StartsWith("Vbus="))
                    {
                        string value = trimmedParam.Substring(5);
                        if (double.TryParse(value, out double vbus))
                        {
                            channel.Vbus = vbus;
                        }
                    }
                    else if (trimmedParam.StartsWith("Vload="))
                    {
                        string value = trimmedParam.Substring(6);
                        if (double.TryParse(value, out double vload))
                        {
                            channel.Vload = vload;
                        }
                    }
                    else if (trimmedParam.StartsWith("I="))
                    {
                        string value = trimmedParam.Substring(2);
                        if (double.TryParse(value, out double current))
                        {
                            channel.I = current;
                        }
                    }
                    else if (trimmedParam.StartsWith("P="))
                    {
                        string value = trimmedParam.Substring(2);
                        if (double.TryParse(value, out double power))
                        {
                            channel.P = power;
                        }
                    }
                }

                return channel;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print($"解析通道資料錯誤: {ex.Message}");
                return null;
            }
        }
    }
}
