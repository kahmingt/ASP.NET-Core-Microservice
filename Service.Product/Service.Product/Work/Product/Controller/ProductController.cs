using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Service.Product.Shared.Database.Entity;
using Service.Product.Shared.Repository;
using Service.Product.Work.Model;
using Service.Product.Work.Utility;

namespace Service.Product.Work.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _db;

        public ProductController(
            ILogger<ProductController> logger,
            IMapper mapper,
            IUnitOfWork db)
        {
            _logger = logger;
            _mapper = mapper;
            _db = db;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateProductAsync([FromBody] ProductDetailCreateModel productDetails)
        {
            if (!ModelState.IsValid || productDetails is null)
            {
                _logger.LogError("400 Bad Request. Invalid or null.");
                return BadRequest("Invalid");
            }
            try
            {
                var modelDB = _mapper.Map<ProductDetailCreateModel, dbProduct>(productDetails);
                await _db.ProductRepository.CreateProductAsync(modelDB);
                await _db.CommitChangesAsync();
                return CreatedAtRoute("CreateProduct", new { id = modelDB.ProductId }, modelDB);
            }
            catch (Exception ex)
            {
                _logger.LogError("500 Internal Server Error. " + ex.Message + ex.InnerException.Message);
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteProductDetailsByIdAsync(int id)
        {
            try
            {
                var modelDB = await _db.ProductRepository.GetProductDetailsByIdAsync(id);
                if (modelDB is null)
                {
                    _logger.LogError("404 Not Found. Model is null.");
                    return NotFound("Invalid");
                }
                else
                {
                    await _db.ProductRepository.DeleteProductByIdAsync(modelDB);
                    await _db.CommitChangesAsync();
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("500 Internal Server Error. " + ex.Message + ex.InnerException.Message);
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDetailRetrieveModel>> GetProductDetailsByIdAsync(int id)
        {
            try
            {
                var modelDB = await _db.ProductRepository.GetProductDetailsByIdAsync(id);
                if (modelDB is null)
                {
                    _logger.LogError("404 Not Found. Model is null.");
                    return NotFound("Invalid");
                }
                else
                {
                    var modelDTO = _mapper.Map<dbProduct, ProductDetailRetrieveModel>(modelDB);
                    return Ok(modelDTO);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("500 Internal Server Error. " + ex.Message + ex.InnerException.Message);
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<dbProduct>> GetProductListAsync([FromQuery] ProductQueryableParameter parameter)
        {
            if (!ModelState.IsValid || parameter is null)
            {
                _logger.LogError("400 Bad Request. Invalid or null.");
                return BadRequest("Invalid");
            }
            try
            {
                var modelDB = await _db.ProductRepository.GetProductListAsync(parameter);
                if (modelDB is null)
                {
                    _logger.LogError("404 Not Found. Model is null.");
                    return NotFound("Invalid");
                }
                else
                {
                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(modelDB.GetPagingMetadata));
                    var modelDTO = _mapper.Map<List<dbProduct>, List<ProductListRetrieveModel>>(modelDB);
                    return Ok(modelDTO);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("500 Internal Server Error. " + ex.Message + ex.InnerException.Message);
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateProductDetailsById(int id, [FromBody] ProductDetailUpdateModel productDetails)
        {
            if (!ModelState.IsValid || productDetails is null)
            {
                _logger.LogError("400 Bad Request. Invalid or null.");
                return BadRequest("Invalid");
            }
            try
            {
                var modelDB = await _db.ProductRepository.GetProductDetailsByIdAsync(id);
                if (modelDB is null)
                {
                    _logger.LogError("404 Not Found.");
                    return NotFound("Invalid");
                }
                else
                {
                    modelDB = _mapper.Map<ProductDetailUpdateModel, dbProduct>(productDetails);
                    await _db.ProductRepository.UpdateProductDetailsByIdAsync(modelDB);
                    await _db.CommitChangesAsync();
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("500 Internal Server Error. " + ex.Message + ex.InnerException.Message);
                return StatusCode(500, "Internal server error");
            }
        }


    }
}