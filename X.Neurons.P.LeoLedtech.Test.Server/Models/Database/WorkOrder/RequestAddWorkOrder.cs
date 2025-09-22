namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Database.WorkOrder
{
    public class RequestAddWorkOrder
    {
        public string Number { get; set; }
        public string ProdcutNumberHead { get; set; }
        public int ProdcutQuantity { get; set; }
        public int ProductModel { get; set; }
        public string Description { get; set; }
    }
}
