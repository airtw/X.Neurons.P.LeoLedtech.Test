using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using X.Neurons.P.LeoLedtech.Test.Server.BasicUtils;
using X.Neurons.P.LeoLedtech.Test.Server.Models;
using X.Neurons.P.LeoLedtech.Test.Server.Models.Database.Jig;
using X.Neurons.P.LeoLedtech.Test.Server.Models.Database.Product;
using X.Neurons.P.LeoLedtech.Test.Server.Models.Database.TestOrder;
using X.Neurons.P.LeoLedtech.Test.Server.Models.Database.TestStep;
using X.Neurons.P.LeoLedtech.Test.Server.Models.Database.WorkOrder;
using X.Neurons.P.LeoLedtech.Test.Server.Models.Test;

namespace X.Neurons.P.LeoLedtech.Test.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestStepController : ControllerBase
    {
        private readonly ILogger<TestStepController> _logger;

        public TestStepController(ILogger<TestStepController> logger)
        {
            _logger = logger;
        }
        [HttpGet("TestStepHead")]
        public async Task<List<TestStepHead>> GetTestStepHeadAsync()
        {
            try
            {

                var testStepHead = await MariaDbHelper.QueryAsync<TestStepHead>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepHead`");
                return testStepHead;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpGet("{productNumber}")]
        public async Task<TestStep> GetAsync(string productNumber)
        {
            try
            {
                var req = StringHelper.SplitPrefixAndNumber(productNumber);
                var workOrderBody = new WorkOrderBody();
                if (req.Prefix != string.Empty && req.Prefix != "" && req.Prefix.Length >= 1)
                {
                    workOrderBody = await MariaDbHelper.QuerySingleOrDefaultAsync<WorkOrderBody>(GlobalSettings.DBConnectionString, "SELECT * FROM `WorkOrderBody` WHERE ProductNumberHead = @ProductNumberHead AND ProductNumberBody = @ProductNumberBody", new { ProductNumberHead = req.Prefix, ProductNumberBody = req.NumberStr });
                }
                else
                {
                    workOrderBody = await MariaDbHelper.QuerySingleOrDefaultAsync<WorkOrderBody>(GlobalSettings.DBConnectionString, "SELECT * FROM `WorkOrderBody` WHERE ProductNumberBody = @ProductNumberBody", new { ProductNumberBody = req.NumberStr });
                }

                var workOrderHead = await MariaDbHelper.QuerySingleOrDefaultAsync<WorkOrderHead>(GlobalSettings.DBConnectionString, "SELECT * FROM `WorkOrderHead` WHERE Id = @Id ", new { Id = workOrderBody.WorkOrderHeadId });
                var product = await MariaDbHelper.QuerySingleOrDefaultAsync<Product>(GlobalSettings.DBConnectionString, "SELECT * FROM `Product` WHERE Id = @Id ", new { Id = workOrderHead.ProductModel });
                var testStepHead = await MariaDbHelper.QuerySingleOrDefaultAsync<TestStepHead>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepHead` WHERE Id = @Id ", new { Id = product.TestStepId });
                var testStepBody = await MariaDbHelper.QueryAsync<TestStepBody>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepBody` WHERE TestStepHeadId = @TestStepHeadId ", new { TestStepHeadId = testStepHead.Id });
                var testStepBodyChannel = await MariaDbHelper.QueryAsync<TestStepBodyChannel>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepBodyChannel` WHERE TestStepHeadId = @TestStepHeadId ", new { TestStepHeadId = testStepHead.Id });
                var r = new TestStep
                {
                    Guid = testStepHead.Id.ToString(),
                    Name = "AT001測試步驟",
                    Description = "測試步驟",
                    Cable = new List<Cable>(),
                    Step = new List<Models.TestStep.Step>()
                };
                foreach (var ch in testStepBodyChannel)
                {
                    r.Cable.Add(new Cable
                    {
                        ID = ch.Id,
                        Name = ch.Name,
                        Channel = ch.Channel,
                        CableColor = ch.CableColor
                    });
                }
                foreach (var step in testStepBody)
                {
                    var s = new Models.TestStep.Step();
                    s.ID = step.Step;
                    s.Voltage = step.Voltage;
                    s.Current = step.Current;
                    s.Channel = new List<Models.TestStep.Channel>();
                    var testStepBodyLimit = await MariaDbHelper.QueryAsync<TestStepBodyLimit>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepBodyLimit` WHERE TestStepBodyId = @TestStepBodyId ", new { TestStepBodyId = step.Id });
                    foreach (var chL in testStepBodyLimit)
                    {
                        s.Channel.Add(new Models.TestStep.Channel
                        {
                            ID = chL.Id,
                            ChannelNumber = chL.Channel,
                            HH = chL.HH,
                            H = chL.H,
                            L = chL.L,
                            LL = chL.LL
                        });
                    }
                    r.Step.Add(s);
                }
                return r;
            }
            catch(Exception ex)
            {
                return new TestStep();
            }
        }
        #region TestStepBody
        [HttpGet("TestStepBody/{headId}")]
        public async Task<List<TestStepBody>> GetTestStepBodyAsync(int headId)
        {
            try
            {
                var testStepBody = await MariaDbHelper.QueryAsync<TestStepBody>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepBody` WHERE TestStepHeadId = @TestStepHeadId",new { TestStepHeadId = headId});
                return testStepBody;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpPost("TestStepBody")]
        public async Task<IActionResult> AddTestStepBodyAsync(TestStepBody request)
        {
            try
            {
                var newId = await MariaDbHelper.InsertAsync(GlobalSettings.DBConnectionString, "INSERT INTO TestStepBody(TestStepHeadId, Name, Step, Voltage, Current, CollectionDelayTime, DleayTime, Description) VALUES (@TestStepHeadId, @Name, @Step, @Voltage, @Current, @CollectionDelayTime, @DleayTime, Description)", new { TestStepHeadId = request.TestStepHeadId, Name = request.Name, Step = request.Step, Voltage = request.Voltage, Current = request.Current, CollectionDelayTime = request.CollectionDelayTime, DleayTime = request.DleayTime, Description = request.Description });
                var testStepBody = await MariaDbHelper.QuerySingleOrDefaultAsync<TestStepBody>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepBody` WHERE Id = @Id", new { Id = newId });
                return Ok(testStepBody);
            }
            catch (Exception ex)
            {
                return BadRequest("錯誤:" + ex);
            }
        }
        [HttpPut("TestStepBody")]
        public async Task<IActionResult> UpdateTestStepBodyAsync(TestStepBody request)
        {
            try
            {
                var updateStr = SqlUpdateBuilder.BuildUpdateSql("TestStepBody", request);
                var updateId = await MariaDbHelper.ExecuteAsync(GlobalSettings.DBConnectionString, updateStr.Sql, updateStr.Parameters);

                var testStepBody = await MariaDbHelper.QuerySingleOrDefaultAsync<TestStepBody>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepBody` WHERE Id = @Id", new { Id = updateId });
                return Ok(testStepBody);
            }
            catch (Exception ex)
            {
                return BadRequest("錯誤:" + ex);
            }
        }
        [HttpPut("TestStepBody/{id}/Step")]
        public async Task<IActionResult> UpdateTestStepBodyStepAsync(int id, DTO_TestStepBodyChange request)
        {
            try
            {
                //a
                var testStepBody = await MariaDbHelper.QuerySingleOrDefaultAsync<TestStepBody>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepBody` WHERE Id = @Id", new { Id = id });
                request.Id = testStepBody.Id;
                var updateStr = SqlUpdateBuilder.BuildUpdateSql("TestStepBody", request);
                var updateId = await MariaDbHelper.ExecuteAsync(GlobalSettings.DBConnectionString, updateStr.Sql, updateStr.Parameters);

                //var testStepBody = await MariaDbHelper.QuerySingleOrDefaultAsync<TestStepBody>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepBody` WHERE Id = @Id", new { Id = updateId });
                return Ok(testStepBody);
            }
            catch (Exception ex)
            {
                return BadRequest("錯誤:" + ex);
            }
        }
        [HttpDelete("TestStepBody/{Id}")]
        public async Task<IActionResult> DeleteTestStepBodyAsync(int id)
        {
            try
            {
                var updateId = await MariaDbHelper.ExecuteAsync(GlobalSettings.DBConnectionString, "DELETE FROM  TestStepBody WHERE Id = @Id", new { Id = id });
                var testStepBody = await MariaDbHelper.QuerySingleOrDefaultAsync<TestStepBody>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepBody` WHERE Id = @Id", new { Id = updateId });
                return Ok(testStepBody);
            }
            catch (Exception ex)
            {
                return BadRequest("錯誤:" + ex);
            }
        }
        #endregion
        #region Channel
        [HttpGet("TestStepBodyChannel/{headId}")]
        public async Task<List<TestStepBodyChannel>> GetTestStepBodyChannelAsync(int headId)
        {
            try
            {

                var testStepBodyChannel = await MariaDbHelper.QueryAsync<TestStepBodyChannel>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepBodyChannel` WHERE TestStepHeadId = @TestStepHeadId" , new { TestStepHeadId = headId });
                return testStepBodyChannel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpPost("TestStepBodyChannel")]
        public async Task<IActionResult> AddTestStepBodyChannelAsync(TestStepBodyChannel request)
        {
            try
            {
                var newId = await MariaDbHelper.InsertAsync(GlobalSettings.DBConnectionString, "INSERT INTO TestStepBodyChannel(TestStepHeadId, Channel, Name, CableColor, Description) VALUES (@TestStepHeadId, @Channel, @Name, @CableColor, @Description)", new { TestStepHeadId = request.TestStepHeadId, Channel = request.Channel, Name = request.Name, CableColor = request.CableColor, Description = request.Description });
                var testStepBodyChannel = await MariaDbHelper.QuerySingleOrDefaultAsync<Jig>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepBodyChannel` WHERE Id = @Id", new { Id = newId });
                return Ok(testStepBodyChannel);
            }
            catch (Exception ex)
            {
                return BadRequest("錯誤:" + ex);
            }
        }
        [HttpPut("TestStepBodyChannel")]
        public async Task<IActionResult> UpdateTestStepBodyChannelAsync(TestStepBodyChannel request)
        {
            try
            {
                var updateStr = SqlUpdateBuilder.BuildUpdateSql("TestStepBodyChannel", request);
                var updateId = await MariaDbHelper.ExecuteAsync(GlobalSettings.DBConnectionString, updateStr.Sql, updateStr.Parameters);

                var testStepBodyChannel = await MariaDbHelper.QuerySingleOrDefaultAsync<TestStepBodyChannel>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepBodyChannel` WHERE Id = @Id", new { Id = updateId });
                return Ok(testStepBodyChannel);
            }
            catch (Exception ex)
            {
                return BadRequest("錯誤:" + ex);
            }
        }
        [HttpDelete("TestStepBodyChannel/{Id}")]
        public async Task<IActionResult> DeleteTestStepBodyChannelAsync(int id)
        {
            try
            {
                //搜尋TestStepBodyChannel
                var testStepBodyChannel = await MariaDbHelper.QuerySingleOrDefaultAsync<TestStepBodyChannel>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepBodyChannel` WHERE Id = @Id", new { Id = id });
                //搜尋TestStepHead
                var testStepHead = await MariaDbHelper.QuerySingleOrDefaultAsync<TestStepHead>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepHead` WHERE Id = @Id", new { Id = testStepBodyChannel.TestStepHeadId });
                //搜尋TestStepBody
                var testStepBody = await MariaDbHelper.QueryAsync<TestStepBody>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepBody` WHERE TestStepHeadId = @TestStepHeadId", new { TestStepHeadId = testStepHead.Id });
                //搜尋TestStepBodyLimit
                foreach(var body in testStepBody)
                {
                    //搜尋每個body的限值
                    var testStepBodyLimit = await MariaDbHelper.QueryAsync<TestStepBodyLimit>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepBodyLimit` WHERE TestStepBodyId = @TestStepBodyId AND Channel = @Channel", new { TestStepBodyId = body.Id, Channel = testStepBodyChannel.Channel});
                    foreach(var limit in testStepBodyLimit)
                    {
                        var deleteLimit = await MariaDbHelper.ExecuteAsync(GlobalSettings.DBConnectionString, "DELETE FROM  TestStepBodyLimit WHERE Id = @Id", new { Id = limit.Id });
                    }
                }
        
                //執行刪除channel
                var updateId = await MariaDbHelper.ExecuteAsync(GlobalSettings.DBConnectionString, "DELETE FROM  TestStepBodyChannel WHERE Id = @Id", new { Id = id });
                var final = await MariaDbHelper.QuerySingleOrDefaultAsync<Jig>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepBodyChannel` WHERE Id = @Id", new { Id = updateId });
                
                return Ok(final);
            }
            catch (Exception ex)
            {
                return BadRequest("錯誤:" + ex);
            }
        }
        #endregion
        #region TestStepBodyLimit
        [HttpGet("TestStepBodyLimit/{bodyId}")]
        public async Task<List<TestStepBodyLimit>> GetTestStepBodyLimitAsync(int bodyId)
        {
            try
            {

                var testStepBodyLimit = await MariaDbHelper.QueryAsync<TestStepBodyLimit>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepBodyLimit` WHERE TestStepBodyId = @TestStepBodyId", new { TestStepBodyId =bodyId });
                return testStepBodyLimit;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpPost("TestStepBodyLimit")]
        public async Task<IActionResult> AddTestStepBodyLimitAsync(TestStepBodyLimit request)
        {
            try
            {
                var newId = await MariaDbHelper.InsertAsync(GlobalSettings.DBConnectionString, "INSERT INTO TestStepBodyLimit(TestStepBodyId, Channel, HH, H, L, LL) VALUES (@TestStepBodyId, @Channel, @HH, @H, @L, @LL)", new { TestStepBodyId = request.TestStepBodyId, Channel = request.Channel, HH = request.HH, H = request.H, L = request.L, LL = request.LL });
                var testStepBodyChannel = await MariaDbHelper.QuerySingleOrDefaultAsync<Jig>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepBodyChannel` WHERE Id = @Id", new { Id = newId });
                return Ok(testStepBodyChannel);
            }
            catch (Exception ex)
            {
                return BadRequest("錯誤:" + ex);
            }
        }
        [HttpPut("TestStepBodyLimit")]
        public async Task<IActionResult> UpdateTestStepBodyLimitAsync(TestStepBodyLimit request)
        {
            try
            {
                var updateStr = SqlUpdateBuilder.BuildUpdateSql("TestStepBodyLimit", request);
                var updateId = await MariaDbHelper.ExecuteAsync(GlobalSettings.DBConnectionString, updateStr.Sql, updateStr.Parameters);
                var testStepBodyLimit = await MariaDbHelper.QuerySingleOrDefaultAsync<TestStepBodyChannel>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepBodyLimit` WHERE Id = @Id", new { Id = updateId });
                return Ok(testStepBodyLimit);
            }
            catch (Exception ex)
            {
                return BadRequest("錯誤:" + ex);
            }
        }
        [HttpDelete("TestStepBodyLimit/{Id}")]
        public async Task<IActionResult> DeleteTestStepBodyLimitAsync(int id)
        {
            try
            {
                var updateId = await MariaDbHelper.ExecuteAsync(GlobalSettings.DBConnectionString, "DELETE FROM  TestStepBodyLimit WHERE Id = @Id", new { Id = id });
                var testStepBodyLimit = await MariaDbHelper.QuerySingleOrDefaultAsync<TestStepBodyLimit>(GlobalSettings.DBConnectionString, "SELECT * FROM `TestStepBodyLimit` WHERE Id = @Id", new { Id = updateId });
                return Ok(testStepBodyLimit);
            }
            catch (Exception ex)
            {
                return BadRequest("錯誤:" + ex);
            }
        }
        #endregion
    }
}
