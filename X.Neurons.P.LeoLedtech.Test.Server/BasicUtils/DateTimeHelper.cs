namespace X.Neurons.P.LeoLedtech.Test.Server.BasicUtils
{
    public class DateTimeHelper
    {
        /// <summary>
        /// 國歷轉西元
        /// </summary>
        /// <param name="rocYear"></param>
        /// <returns></returns>
        public static int ConvertROCToAD(int rocYear)
        {
            return rocYear + 1911;
        }

    }
}
