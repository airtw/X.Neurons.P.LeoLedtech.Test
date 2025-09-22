using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using X.Neurons.P.LeoLedtech.Test.Server.BasicUtils;
using X.Neurons.P.LeoLedtech.Test.Server.Models;
using X.Neurons.P.LeoLedtech.Test.Server.Models.Database.Product;
using X.Neurons.P.LeoLedtech.Test.Server.Models.Database.TestOrder;
using X.Neurons.P.LeoLedtech.Test.Server.Models.Database.WorkOrder;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace X.Neurons.P.LeoLedtech.Test.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkOderController : ControllerBase
    {
        private readonly ILogger<WorkOderController> _logger;

        public WorkOderController(ILogger<WorkOderController> logger)
        {
            _logger = logger;
        }
        [HttpGet("WorkOrderHead")]
        public async Task<List<DTO_WorkOrderHead>> GetWorkOrderHeadAsync()
        {

            var data = await MariaDbHelper.QueryAsync<WorkOrderHead>(GlobalSettings.DBConnectionString, "SELECT * FROM `WorkOrderHead`");
            //var product = await MariaDbHelper.QueryAsync<Product>(GlobalSettings.DBConnectionString, "SELECT * FROM `Product`");
            var reslut = new List<DTO_WorkOrderHead>();
            foreach (var d in data)
            {
                var workOrderBody = await MariaDbHelper.QueryAsync<WorkOrderBody>(GlobalSettings.DBConnectionString, "SELECT * FROM `WorkOrderBody` WHERE WorkOrderHeadId = @WorkOrderHeadId", new { WorkOrderHeadId  = d.Id});

                var newData = new DTO_WorkOrderHead()
                {
                    Id = d.Id,
                    Create = d.Create,
                    CreateDateTime = d.CreateDateTime,
                    Number = d.Number,
                    ProductModel = d.ProductModel,
                    ProdcutQuantity = workOrderBody.Count,
                    Description = d.Description
                };
                if (workOrderBody.FirstOrDefault() != null)
                {
                    newData.ProdcutNumberHead = workOrderBody.FirstOrDefault().ProductNumberHead;
                }
                reslut.Add(newData);

            }
            return reslut;
        }
        [HttpGet("OneWorkOrderBody/{id}")]
        public async Task<List<DTO_WorkOrderBody>> GetOneWorkOrderBodyAsync(int id)
        {
            var resultData = new List<DTO_WorkOrderBody>();
            var data = await MariaDbHelper.QueryAsync<WorkOrderBody>(GlobalSettings.DBConnectionString, "SELECT * FROM `WorkOrderBody` WHERE WorkOrderHeadId=@Id ", new { Id = id});
            foreach(var d in data)
            {
                resultData.Add(new DTO_WorkOrderBody
                {
                    Id = d.Id,
                    Create = d.Create,
                    CreateDateTime = d.CreateDateTime,
                    IsTest = d.IsTest,
                    LastIsPass = d.LastIsPass,
                    LastTestDateTime = d.LastTestDateTime,
                    LastTestUser = d.LastTestUser,
                    WorkOrderHeadId = d.WorkOrderHeadId,
                    ProductNumber = $"{d.ProductNumberHead}{d.ProductNumberBody}",
                    Description = d.Description,
                });
            }
            return resultData;
        }
        [HttpPost("WorkOrderBody")]
        public async Task<List<DTO_WorkOrderBody>> GetWorkOrderBodyAsync(RequestWorkOrder request)
        {
            var resultData = new List<DTO_WorkOrderBody>();
            if(request.ProductNumber != string.Empty && request.ProductNumber != "" && request.ProductNumber.Length >= 3)
            {
                var match = Regex.Match(request.ProductNumber, @"^([A-Za-z]+)([0-9]+)$");
                string prefix = match.Groups[1].Value;   // "DL"
                string numberStr = match.Groups[2].Value; // "0001"
                if (match.Success)
                {
                    var data = await MariaDbHelper.QueryAsync<WorkOrderBody>(GlobalSettings.DBConnectionString, "SELECT * FROM `WorkOrderBody` WHERE ProductNumberHead = @ProductNumberHead AND ProductNumberBody = @ProductNumberBody", new { ProductNumberHead = prefix,  ProductNumberBody = numberStr });
                    foreach (var d in data)
                    {

                        resultData.Add(new DTO_WorkOrderBody
                        {
                            Id = d.Id,
                            Create = d.Create,
                            CreateDateTime = d.CreateDateTime,
                            IsTest = d.IsTest,
                            LastIsPass = d.LastIsPass,
                            LastTestDateTime = d.LastTestDateTime,
                            LastTestUser = d.LastTestUser,
                            WorkOrderHeadId = d.WorkOrderHeadId,
                            ProductNumber = $"{d.ProductNumberHead}{d.ProductNumberBody}",
                            Description = d.Description,
                        });
                    }
                }
                else
                {
                    var data = await MariaDbHelper.QueryAsync<WorkOrderBody>(GlobalSettings.DBConnectionString, "SELECT * FROM `WorkOrderBody` WHERE ProductNumberBody = @ProductNumberBody", new { ProductNumberBody = request.ProductNumber });
                    foreach (var d in data)
                    {

                        resultData.Add(new DTO_WorkOrderBody
                        {
                            Id = d.Id,
                            Create = d.Create,
                            CreateDateTime = d.CreateDateTime,
                            IsTest = d.IsTest,
                            LastIsPass = d.LastIsPass,
                            LastTestDateTime = d.LastTestDateTime,
                            LastTestUser = d.LastTestUser,
                            WorkOrderHeadId = d.WorkOrderHeadId,
                            ProductNumber = $"{d.ProductNumberBody}",
                            Description = d.Description,
                        });
                    }
                }
            }
            else
            {
                var data = await MariaDbHelper.QueryAsync<WorkOrderBody>(GlobalSettings.DBConnectionString, "SELECT * FROM `WorkOrderBody` WHERE WorkOrderHeadId = @WorkOrderHeadId AND CreateDateTime BETWEEN @StartDate AND @EndDate", new { WorkOrderHeadId = request.WorkOderNumber.Id, StartDate = request.StartTime, EndDate = request.EndTime });
                foreach (var d in data)
                {
                    resultData.Add(new DTO_WorkOrderBody
                    {
                        Id = d.Id,
                        Create = d.Create,
                        CreateDateTime = d.CreateDateTime,
                        IsTest = d.IsTest,
                        LastIsPass = d.LastIsPass,
                        LastTestDateTime = d.LastTestDateTime,
                        LastTestUser = d.LastTestUser,
                        WorkOrderHeadId = d.WorkOrderHeadId,
                        ProductNumber = $"{d.ProductNumberHead}{d.ProductNumberBody}",
                        Description = d.Description,
                    });
                }
            }
            return resultData;
        }
        [HttpPost("AddWorkOrder")]
        public async Task<WorkOrderHead> AddWorkOrderAsync(RequestAddWorkOrder request)
        {
            //單號1140915116 [114]年 [09]月 [15]日 [116]序號
            //流水號 51160001 [5]由民國轉西元 [116]為序號 [0001]後四位則為流水號
            if(request.Number.Length == 10)
            {
                var numberYear = request.Number.Substring(0, 3); //拿年
                var numberMonth = request.Number.Substring(3, 2); //拿月
                var numberDay = request.Number.Substring(5, 2); //拿日
                var numberSerial = request.Number.Substring(7, 3); //拿序號
                var adYear = DateTimeHelper.ConvertROCToAD(int.Parse(numberYear)).ToString().Last(); //轉換後第一碼

                var createDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var newData = new WorkOrderHead
                {
                    CreateDateTime = createDateTime,
                    IsDeleted = false,
                    Number = request.Number,
                    ProductModel = request.ProductModel,
                    Description = request.Description,
                };

                var newId = await MariaDbHelper.InsertAsync(GlobalSettings.DBConnectionString, "INSERT INTO WorkOrderHead(CreateDateTime, Number, ProductModel, Description) VALUES (@CreateDateTime,@Number, @ProductModel, @Description)", new { CreateDateTime = newData.CreateDateTime, Number = newData.Number, ProductModel = newData.ProductModel, Description = newData.Description });
                var productNumberHeadData = new List<WorkOrderBody>();
                if (request.ProdcutNumberHead != null && request.ProdcutNumberHead != string.Empty && request.ProdcutNumberHead != "" && request.ProdcutNumberHead.Length >= 1)
                {
                    productNumberHeadData = await MariaDbHelper.QueryAsync<WorkOrderBody>(GlobalSettings.DBConnectionString, "SELECT * FROM WorkOrderBody WHERE ProductNumberHead = @Head ORDER BY ProductNumberBody ASC;", new { Head = request.ProdcutNumberHead });
                    for (int i = 1; i <= request.ProdcutQuantity; i++)
                    {
                        var last = int.Parse(productNumberHeadData.Last().ProductNumberBody);
                        var seq = last + i;                  // 000123 → 第一筆變 000124
                        var body = $"{adYear}{numberSerial}{seq.ToString("D4")}";       // 補零至6位，例如 000124

                        var newProduct = await MariaDbHelper.InsertAsync(GlobalSettings.DBConnectionString, "INSERT INTO WorkOrderBody(WorkOrderHeadId, CreateDateTime, ProductNumberHead, ProductNumberBody) VALUES (@WorkOrderHeadId, @CreateDateTime, @ProductNumberHead, @ProductNumberBody)", new { WorkOrderHeadId = newId, CreateDateTime = createDateTime, ProductNumberHead = request.ProdcutNumberHead, ProductNumberBody = $"{body}" });
                        Console.WriteLine(i);
                    }
                }
                else
                {
                    var t = new List<WorkOrderBody>();
                    for (int i = 1; i <= request.ProdcutQuantity; i++)
                    {
                        int last, seq;
                        string body;
                        last = 0;
                        seq = last + i;                  // 000123 → 第一筆變 000124
                        body = $"{adYear}{numberSerial}{seq.ToString("D4")}";       // 補零至6位，例如 000124

                        var newProduct = await MariaDbHelper.InsertAsync(GlobalSettings.DBConnectionString, "INSERT INTO WorkOrderBody(WorkOrderHeadId, CreateDateTime, ProductNumberBody) VALUES (@WorkOrderHeadId, @CreateDateTime, @ProductNumberBody)", new { WorkOrderHeadId = newId, CreateDateTime = createDateTime, ProductNumberBody = $"{body}" });
                        Console.WriteLine(i);
                    }

                }
                return newData;
            }
            else
            {
                return null;
            }
    
        }
    }
}
