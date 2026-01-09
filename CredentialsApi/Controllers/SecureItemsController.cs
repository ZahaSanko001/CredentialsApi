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

        public SecureItemsController(AppDbContext context)
        {
            _context = context;
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
                EncryptedContent = dto.Content
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
                Content = x.EncryptedContent
            });

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserId();
            var item = await _context.SecureItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (item == null) return NotFound();
            if (item.UserId != userId) return Forbid();

            var result = new ResponseDTO
            {
                Id = item.Id,
                Title = item.Title,
                CreatedAt = item.CreatedAt,
                Content = item.EncryptedContent
            };

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, SecureItemDTO dto)
        {
            var userId = GetUserId();
            var item = await _context.SecureItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (item == null) return NotFound();
            if (item.UserId != userId) return Forbid();

            item.Title = dto.Title;
            item.EncryptedContent = dto.Content;

            await _context.SaveChangesAsync();

            return Ok("Updated Successfully");
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var item = await _context.SecureItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (item == null) return NotFound();
            if (item.UserId != userId) return Forbid();

            _context.SecureItems.Remove(item);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = GetUserId();
            var query = _context.SecureItems.Where(x => x.UserId == userId);

            if (!string.IsNullOrWhiteSpace(q)) query = query.Where(x => x.Title.Contains(q));

            var items = await query.OrderByDescending(x => x.CreatedAt)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            var result = items.Select(x => new ResponseDTO
            {
                Id = x.Id,
                Title = x.Title,
                CreatedAt = x.CreatedAt,
                Content = x.EncryptedContent
            });

            return Ok(result);
        }
    }
}
