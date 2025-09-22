using Microsoft.AspNetCore.Mvc;
using X.Neurons.P.LeoLedtech.Test.Server.BasicUtils;
using X.Neurons.P.LeoLedtech.Test.Server.Models.Database.Product;

namespace X.Neurons.P.LeoLedtech.Test.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public async Task<List<Product>> GetProductController()
        {

            var data = await MariaDbHelper.QueryAsync<Product>(GlobalSettings.DBConnectionString, "SELECT * FROM `Product` WHERE IsDeleted = @IsDeleted", new { IsDeleted = false });
            return data;
        }
        [HttpPost]
        public async Task<IActionResult> AddProductAsync(Product request)
        {
            try
            {
                var newId = await MariaDbHelper.InsertAsync(GlobalSettings.DBConnectionString, "INSERT INTO Product(Model, TestStepId, Description) VALUES (@Model, @TestStepId, @Description)", new { Model = request.Model, TestStepId = request.TestStepId, Description = request.Description });
                var product = await MariaDbHelper.QuerySingleOrDefaultAsync<Product>(GlobalSettings.DBConnectionString, "SELECT * FROM `Product` WHERE Id = @Id", new { Id = newId });
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest("錯誤:" + ex);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProductAsync(Product request)
        {
            try
            {
                var updateStr = SqlUpdateBuilder.BuildUpdateSql("Product", request);
                var updateId = await MariaDbHelper.ExecuteAsync(GlobalSettings.DBConnectionString, updateStr.Sql, updateStr.Parameters);

                var product = await MariaDbHelper.QuerySingleOrDefaultAsync<Product>(GlobalSettings.DBConnectionString, "SELECT * FROM `Product` WHERE Id = @Id", new { Id = updateId });
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest("錯誤:" + ex);
            }
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            try
            {
                var updateId = await MariaDbHelper.ExecuteAsync(GlobalSettings.DBConnectionString, "UPDATE Product SET IsDeleted = @IsDeleted WHERE Id = @Id", new { IsDeleted = true, Id = id });
                var product = await MariaDbHelper.QuerySingleOrDefaultAsync<Product>(GlobalSettings.DBConnectionString, "SELECT * FROM `Product` WHERE Id = @Id", new { Id = updateId });
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest("錯誤:" + ex);
            }
        }
    }
}
