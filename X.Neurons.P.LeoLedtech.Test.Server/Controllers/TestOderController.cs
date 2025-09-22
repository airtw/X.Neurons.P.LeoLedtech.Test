using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using X.Neurons.P.LeoLedtech.Test.Server.BasicUtils;
using X.Neurons.P.LeoLedtech.Test.Server.Models;
using X.Neurons.P.LeoLedtech.Test.Server.Models.Database.Station;
using X.Neurons.P.LeoLedtech.Test.Server.Models.Database.TestOrder;
using X.Neurons.P.LeoLedtech.Test.Server.Models.Database.TestOrder.AddOrder;
using X.Neurons.P.LeoLedtech.Test.Server.Models.Database.TestStep;
using X.Neurons.P.LeoLedtech.Test.Server.Models.Database.WorkOrder;
using X.Neurons.P.LeoLedtech.Test.Server.Models.PDF;

namespace X.Neurons.P.LeoLedtech.Test.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestOderController : ControllerBase
    {
        private readonly ILogger<TestOderController> _logger;

        public TestOderController(ILogger<TestOderController> logger)
        {
            _logger = logger;
        }
        [HttpGet("TestOrderHead")]
        public async Task<List<TestOrderHead>> GetTestOrderHeadAsync()
        {
            var systemTime = DateTime.Now;
            List <TestOrderHead> reslut = new List<TestOrderHead>();
            var data = await MariaDbHelper.QuerySingleOrDefaultAsync<List<TestOrderHead>>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestOrderHead`");
            return data;
        }
        [HttpGet("TestOrderHead/{productNumberId}")]
        public async Task<List<DTO_TestOrderHead>> GetTestOrderHeadAsync(int productNumberId)
        {
            var data = await MariaDbHelper.QueryAsync<TestOrderHead>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestOrderHead` WHERE ProductNumber = @ProductNumberId", new { ProductNumberId = productNumberId });
            var testStep = await MariaDbHelper.QueryAsync<TestStepHead>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepHead`");
            var productInfo = await MariaDbHelper.QuerySingleOrDefaultAsync<WorkOrderBody>(GlobalSettings.DBConnectionString, "SELECT * FROM `WorkOrderBody` WHERE Id = @Id", new { Id = productNumberId });
            var result = new List<DTO_TestOrderHead>();
            foreach(var d in data)
            {
                result.Add(new DTO_TestOrderHead
                {
                    Id = d.Id,
                    CreateDateTime = d.CreateDateTime,
                    Station = d.Station,
                    TestUser = d.TestUser,
                    ProductNumber = $"{productInfo.ProductNumberHead}{productInfo.ProductNumberBody}",
                    TestModel = testStep.Find(r => r.Id == d.TestModel).Name,
                    IsPass = d.IsPass
                });
            }

            return result;
        }
        [HttpGet("TestOrderBody/{testOrderHeadId}")]
        public async Task<List<DTO_TestOrderBody>> GetTestOrderBodyAsync(int testOrderHeadId)
        {
            var data = await MariaDbHelper.QueryAsync<TestOrderBody>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestOrderBody` WHERE TestOrderHeadId = @testOrderHeadId", new { TestOrderHeadId = testOrderHeadId });

            var result = new List<DTO_TestOrderBody>();
            foreach (var d in data)
            {
                var testDetail = await MariaDbHelper.QueryAsync<TestOrderChannel>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestOrderChannel` WHERE TestOrderBodyId = @TestOrderBodyId", new { TestOrderBodyId = d.Id });
                var channel = new List<DTO_TestOrderChannel>();
                foreach(var c in testDetail)
                {
                    channel.Add(new DTO_TestOrderChannel
                    {
                        Channel = c.Channel,
                        Current = $"{c.Current} mA",
                        Voltage = $"{Math.Ceiling(c.Voltage)} V",
                        Power = $"{c.Power} mW"
                    });
                }
                var isPass = d.IsPass == true ? "通過" : "未通過";
                result.Add(new DTO_TestOrderBody
                {
                    Id = d.Id,
                    DateTime = d.DateTime,
                    StepDetail = $"步驟:{d.Step} 輸出電壓:{d.Voltage} 輸出電流:{d.Current} 是否通過:{isPass}",
                    TestOrderChannel = channel

                });
            }

            return result;
        }
        [HttpGet("TestReport/{testOrderHeadId}")]
        public async Task<IActionResult> GetTestReportAsync(int testOrderHeadId)
        {
            try
            {
                var headData = await MariaDbHelper.QuerySingleOrDefaultAsync<TestOrderHead>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestOrderHead` WHERE Id = @Id", new { Id = testOrderHeadId });
                var workOrderBody = await MariaDbHelper.QuerySingleOrDefaultAsync<WorkOrderBody>(GlobalSettings.DBConnectionString, "SELECT * FROM `WorkOrderBody` WHERE Id = @Id", new { Id = headData.ProductNumber });
                var workOrderHead = await MariaDbHelper.QuerySingleOrDefaultAsync<WorkOrderHead>(GlobalSettings.DBConnectionString, "SELECT * FROM `WorkOrderHead` WHERE Id = @Id", new { Id = workOrderBody.WorkOrderHeadId });
                // 設置 QuestPDF 許可證為社區版本
                QuestPDF.Settings.License = LicenseType.Community;

                // 創建報告物件
                var reportResult = new TestReport();
                reportResult.WorkOrderNumber = $"{workOrderHead.Number}";
                reportResult.ProductSerialNumber = $"{workOrderBody.ProductNumberHead}{workOrderBody.ProductNumberBody}";
                reportResult.TestDateTime = headData.CreateDateTime;

                // 從資料庫取得測試步驟資料
                var data = await MariaDbHelper.QueryAsync<TestOrderBody>(GlobalSettings.DBConnectionString,"SELECT * FROM `TestOrderBody` WHERE TestOrderHeadId = @testOrderHeadId ORDER BY Step", new { TestOrderHeadId = testOrderHeadId });

                var allTestStep = new List<TestStep>();

                foreach (var d in data)
                {
                    var testStep = new TestStep();
                    testStep.StepNumber = d.Step.ToString();
                    testStep.TestTime = d.DateTime;

                    // 設定步驟詳情，如果資料庫沒有則用預設格式
                    testStep.StepDetails = $"輸出電壓: {d.Voltage}V 輸出電流: {d.Current}A";

                    // 取得該步驟的通道測試結果
                    var testDetail = await MariaDbHelper.QueryAsync<TestOrderChannel>(GlobalSettings.DBConnectionString,"SELECT * FROM `TestOrderChannel` WHERE TestOrderBodyId = @TestOrderBodyId ORDER BY Channel", new { TestOrderBodyId = d.Id });

                    var channels = new List<ChannelResult>();
                    foreach (var c in testDetail)
                    {
                        channels.Add(new ChannelResult
                        {
                            Channel = $"CH{c.Channel}",
                            Current = $"{c.Current} mA",
                            Voltage = $"{Math.Ceiling(c.Voltage)} V",
                            Power = $"{c.Power} mW",
                            Status = d.IsPass
                        });
                    }

                    testStep.ChannelResults = channels;
                    allTestStep.Add(testStep); // 加入到測試步驟列表
                }

                reportResult.TestSteps = allTestStep;

                // 創建PDF文檔
                var document = new TestReportDocument(reportResult);

                // 創建內存流存儲PDF
                using var stream = new MemoryStream();

                // 生成PDF到記憶體流
                document.GeneratePdf(stream);

                // 重設流位置到開始
                stream.Position = 0;

                // 複製到新的byte array以確保資料完整
                var pdfBytes = stream.ToArray();

                // 設定檔案名稱
                var fileName = $"TestReport_{testOrderHeadId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                // 回傳PDF檔案
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                // 錯誤處理
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet("LastTestReport/{productId}")]
        public async Task<IActionResult> GetLastTestReportAsync(int productId)
        {
            try
            {
                var headData = await MariaDbHelper.QueryAsync<TestOrderHead>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestOrderHead` WHERE ProductNumber = @ProductId ORDER BY CreateDateTime", new { ProductId = productId });
                var workOrderBody = await MariaDbHelper.QuerySingleOrDefaultAsync<WorkOrderBody>(GlobalSettings.DBConnectionString, "SELECT * FROM `WorkOrderBody` WHERE Id = @Id", new { Id = headData.Last().ProductNumber });
                var workOrderHead = await MariaDbHelper.QuerySingleOrDefaultAsync<WorkOrderHead>(GlobalSettings.DBConnectionString, "SELECT * FROM `WorkOrderHead` WHERE Id = @Id", new { Id = workOrderBody.WorkOrderHeadId });
                // 設置 QuestPDF 許可證為社區版本
                QuestPDF.Settings.License = LicenseType.Community;

                // 創建報告物件
                var reportResult = new TestReport();
                reportResult.WorkOrderNumber = $"{workOrderHead.Number}";
                reportResult.ProductSerialNumber = $"{workOrderBody.ProductNumberHead}{workOrderBody.ProductNumberBody}";
                reportResult.TestDateTime = headData.Last().CreateDateTime;

                // 從資料庫取得測試步驟資料
                var data = await MariaDbHelper.QueryAsync<TestOrderBody>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestOrderBody` WHERE TestOrderHeadId = @testOrderHeadId ORDER BY Step", new { TestOrderHeadId = headData.Last().Id });

                var allTestStep = new List<TestStep>();

                foreach (var d in data)
                {
                    var testStep = new TestStep();
                    testStep.StepNumber = d.Step.ToString();
                    testStep.TestTime = d.DateTime;

                    // 設定步驟詳情，如果資料庫沒有則用預設格式
                    testStep.StepDetails = $"輸出電壓: {d.Voltage}V 輸出電流: {d.Current}A";

                    // 取得該步驟的通道測試結果
                    var testDetail = await MariaDbHelper.QueryAsync<TestOrderChannel>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestOrderChannel` WHERE TestOrderBodyId = @TestOrderBodyId ORDER BY Channel", new { TestOrderBodyId = d.Id });

                    var channels = new List<ChannelResult>();
                    foreach (var c in testDetail)
                    {
                        channels.Add(new ChannelResult
                        {
                            Channel = $"CH{c.Channel}",
                            Current = $"{c.Current} mA",
                            Voltage = $"{Math.Ceiling(c.Voltage)} V",
                            Power = $"{c.Power} mW",
                            Status = d.IsPass
                        });
                    }

                    testStep.ChannelResults = channels;
                    allTestStep.Add(testStep); // 加入到測試步驟列表
                }

                reportResult.TestSteps = allTestStep;

                // 創建PDF文檔
                var document = new TestReportDocument(reportResult);

                // 創建內存流存儲PDF
                using var stream = new MemoryStream();

                // 生成PDF到記憶體流
                document.GeneratePdf(stream);

                // 重設流位置到開始
                stream.Position = 0;

                // 複製到新的byte array以確保資料完整
                var pdfBytes = stream.ToArray();

                // 設定檔案名稱
                var fileName = $"TestReport_{productId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                // 回傳PDF檔案
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                // 錯誤處理
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost("AddTestOrder")]
        public async Task<Response_Result> AddTestOrder(Request_AddTestOrder request)
        {
            var req = StringHelper.SplitPrefixAndNumber(request.WorkOrderBodyId);

            var workOrderBody = new TestOrderHead();
            if (req.Prefix != string.Empty && req.Prefix != "" && req.Prefix.Length >=1)
            {
                workOrderBody = await MariaDbHelper.QuerySingleOrDefaultAsync<TestOrderHead>(GlobalSettings.DBConnectionString, "SELECT * FROM `WorkOrderBody` WHERE ProductNumberHead = @ProductNumberHead ProductNumberBody = @ProductNumberBody", new { ProductNumberHead = req.Prefix, ProductNumberBody = req.NumberStr});
            }
            else
            {
                workOrderBody = await MariaDbHelper.QuerySingleOrDefaultAsync<TestOrderHead>(GlobalSettings.DBConnectionString, "SELECT * FROM `WorkOrderBody` WHERE ProductNumberBody = @ProductNumberBody", new { ProductNumberBody = req.NumberStr });
            }
            var updateWorkOrderBody = await MariaDbHelper.ExecuteAsync(GlobalSettings.DBConnectionString, "UPDATE WorkOrderBody SET IsTest = @IsTest , LastTestDateTime = @LastTestDateTime ,LastTestUser =@LastTestUser , LastIsPass = @LastIsPass WHERE id = @WorkOrderBodyId", new { IsTest = true, LastTestDateTime = request.Head.CreateDateTime, LastTestUser = 1, LastIsPass = request.Head.IsPass, WorkOrderBodyId = workOrderBody.Id});
            //UPDATE users SET name = @Name WHERE id = @Id

            var newTestOrderHead = await MariaDbHelper.InsertAsync(GlobalSettings.DBConnectionString, "INSERT INTO TestOrderHead(CreateDateTime, Station, TestUser, ProductNumber, TestModel, IsPass) VALUES (@CreateDateTime, @Station, @TestUser, @ProductNumber, @TestModel, @IsPass)", new { CreateDateTime = request.Head.CreateDateTime, Station = request.Head.Station, TestUser = request.Head.TestUser, ProductNumber = workOrderBody.Id, TestModel = request.Head.TestModel, IsPass = request.Head.IsPass });
            
            foreach(var body in request.Body)
            {
                var newTestOrderBody = await MariaDbHelper.InsertAsync(GlobalSettings.DBConnectionString, "INSERT INTO TestOrderBody(TestOrderHeadId, DateTime, Step, Voltage, Current, IsPass) VALUES (@TestOrderHeadId, @DateTime, @Step, @Voltage, @Current ,@IsPass)", new { TestOrderHeadId = newTestOrderHead, DateTime = body.DateTime, Step = body.Step, Voltage = body.Voltage, Current = body.Current, IsPass = body.IsPass});
                foreach(var channel in body.Channels)
                {
                    var newTestOrderChannel = await MariaDbHelper.InsertAsync(GlobalSettings.DBConnectionString, "INSERT INTO TestOrderChannel(TestOrderBodyId, Channel, Voltage, Current, Power) VALUES (@TestOrderBodyId, @Channel, @Voltage, @Current, @Power)", new { TestOrderBodyId = newTestOrderBody, Channel = channel.Channel, Voltage = channel.Voltage, Current = channel.Current, Power = channel.Power });
                }
            }
            return new Response_Result { IsOK = true};
        }
    }
}
