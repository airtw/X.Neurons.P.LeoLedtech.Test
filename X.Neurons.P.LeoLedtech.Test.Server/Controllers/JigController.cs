using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using X.Neurons.P.LeoLedtech.Test.Server.BasicUtils;
using X.Neurons.P.LeoLedtech.Test.Server.Models.Database.Jig;

namespace X.Neurons.P.LeoLedtech.Test.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JigController : ControllerBase
    {
        private readonly ILogger<JigController> _logger;

        public JigController(ILogger<JigController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public async Task<List<Jig>> GetAllAsync()
        {
            try
            {
                var jig = await MariaDbHelper.QueryAsync<Jig>(GlobalSettings.DBConnectionString, "SELECT * FROM `Jig` WHERE IsDeleted = @IsDeleted", new { IsDeleted = false });
                return jig;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddJigAsync(Jig request)
        {
            try
            {
                var newId = await MariaDbHelper.InsertAsync(GlobalSettings.DBConnectionString, "INSERT INTO Jig(ProductId, Number, Description) VALUES (@ProductId, @Number, @Description)", new { ProductId = request.ProductId, Number = request.Number, Description = request.Description });
                var jig = await MariaDbHelper.QuerySingleOrDefaultAsync<Jig>(GlobalSettings.DBConnectionString, "SELECT * FROM `Jig` WHERE Id = @Id", new { Id = newId });
                return Ok(jig);
            }
            catch (Exception ex)
            {
                return BadRequest("錯誤:" + ex);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateJigAsync(Jig request)
        {
            try
            {
                var updateStr = SqlUpdateBuilder.BuildUpdateSql("Jig",request);
                var updateId = await MariaDbHelper.ExecuteAsync(GlobalSettings.DBConnectionString, updateStr.Sql, updateStr.Parameters);

                var jig = await MariaDbHelper.QuerySingleOrDefaultAsync<Jig>(GlobalSettings.DBConnectionString, "SELECT * FROM `Jig` WHERE Id = @Id", new { Id = updateId });
                return Ok(jig);
            }
            catch (Exception ex)
            {
                return BadRequest("錯誤:" + ex);
            }
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteJigAsync(int id)
        {
            try
            {
                var updateId = await MariaDbHelper.ExecuteAsync(GlobalSettings.DBConnectionString, "UPDATE Jig SET IsDeleted = @IsDeleted WHERE Id = @Id", new { IsDeleted = true, Id = id });
                var jig = await MariaDbHelper.QuerySingleOrDefaultAsync<Jig>(GlobalSettings.DBConnectionString, "SELECT * FROM `Jig` WHERE Id = @Id", new { Id = updateId });
                return Ok(jig);
            }
            catch (Exception ex)
            {
                return BadRequest("錯誤:" + ex);
            }
        }
    }
}
