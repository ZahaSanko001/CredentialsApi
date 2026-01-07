using CredentialsApi.Data;
using CredentialsApi.DTOs;
using CredentialsApi.Models;
using CredentialsApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CredentialsApi.Controllers
{
    [Route("api/secure-items")]
    [ApiController]
    [Authorize]
    public class SecureItemsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEncryptionService _encryption;

        public SecureItemsController(AppDbContext context, IEncryptionService encryption)
        {
            _context = context;
            _encryption = encryption;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(ClaimTypes.Name)
                ?? User.FindFirstValue(ClaimTypes.Sid)
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpPost]
        public async Task<IActionResult> Create(SecureItemDTO dto)
        {
            var userId = GetUserId();

            var item = new SecureItem
            {
                Title = dto.Title,
                UserId = userId,
                EncryptedContent = _encryption.Encrypt(dto.Content)
            };

            _context.SecureItems.Add(item);
            await _context.SaveChangesAsync();

            return Ok(new { item.Id });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();

            var items = await _context.SecureItems
                .Where(x => x.UserId == userId)
                .ToListAsync();

            var result = items.Select(x => new ResponseDTO
            {
                Id = x.Id,
                Title = x.Title,
                CreatedAt = x.CreatedAt,
                Content = _encryption.Decrypt(x.EncryptedContent)
            });

            return Ok(result);
        }
    }
}
