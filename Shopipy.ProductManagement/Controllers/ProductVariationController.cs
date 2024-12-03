using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopipy.ProductManagement.DTOs;
using Shopipy.ProductManagement.Services;
using Shopipy.Persistence.Models;
using Shopipy.Shared;
using Shopipy.Shared.Services;

namespace Shopipy.ProductManagement.Controllers
{
    [Route("api/{businessId}/products/{productId}/variations")]
    [ApiController]
    public class ProductVariationController : ControllerBase
    {
        private readonly IProductVariationService _variationService;
        private readonly IMapper _mapper;

        public ProductVariationController(IProductVariationService variationService, IMapper mapper)
        {
            _variationService = variationService;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
        public async Task<ActionResult<ProductVariationResponseDTO>> CreateVariationAsync(int businessId, int productId, ProductVariationRequestDTO dto)
        {
            var variation = _mapper.Map<ProductVariation>(dto);
            variation.CreatedAt = DateTime.UtcNow;
            variation.UpdatedAt = DateTime.UtcNow;

            var createdVariation = await _variationService.CreateVariationAsync(variation, productId, businessId);
            var variationResponseDTO = _mapper.Map<ProductVariationResponseDTO>(createdVariation);

            return CreatedAtAction(nameof(GetVariationByIdAsync),
                new { businessId, productId, variationId = createdVariation.VariationId },
                variationResponseDTO);
        }

        [HttpGet("{variationId}")]
        public async Task<ActionResult<ProductVariationResponseDTO>> GetVariationByIdAsync(int businessId, int productId, int variationId)
        {
            var variation = await _variationService.GetVariationByIdAsync(variationId, productId, businessId);
            if (variation == null) return NotFound();

            var variationResponseDTO = _mapper.Map<ProductVariationResponseDTO>(variation);
            return Ok(variationResponseDTO);
        }

        [HttpPut("{variationId}")]
        [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
        public async Task<ActionResult<ProductVariationResponseDTO>> UpdateVariationAsync(int businessId, int productId, int variationId, ProductVariationRequestDTO dto)
        {
            var variation = _mapper.Map<ProductVariation>(dto);
            variation.UpdatedAt = DateTime.UtcNow;

            var updatedVariation = await _variationService.UpdateVariationAsync(variationId, variation, productId, businessId);
            if (updatedVariation == null) return NotFound();

            var variationResponseDTO = _mapper.Map<ProductVariationResponseDTO>(updatedVariation);
            return Ok(variationResponseDTO);
        }

        [HttpDelete("{variationId}")]
        [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
        public async Task<IActionResult> DeleteVariationAsync(int businessId, int productId, int variationId)
        {
            var success = await _variationService.DeleteVariationAsync(variationId, productId, businessId);
            if (!success) return NotFound();

            return NoContent();
        }
    }

}
