namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Database.Station
{
    public class Station
    {
        public string Id { get; set; }
        /// <summary>
        /// 是否已經刪除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 產品型號
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
