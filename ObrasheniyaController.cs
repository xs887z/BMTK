using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Obrasheniya.Application.DTOs;
using Obrasheniya.Application.Services;
using Obrasheniya.Domain.Entities;
using Obrasheniya.Infrastructure.Data;

namespace Obrasheniya.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ObrasheniyaController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILexicomService _lexicomService;
    
    public ObrasheniyaController(ApplicationDbContext context, ILexicomService lexicomService)
    {
        _context = context;
        _lexicomService = lexicomService;
    }
    
    [HttpPost]
    public async Task<ActionResult<ObrashenieDto>> Create(CreateObrashenieDto createDto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        var analysis = await _lexicomService.AnalyzeAsync(createDto.Content);
        
        var obrashenie = new Obrashenie
        {
            Id = Guid.NewGuid(),
            Title = createDto.Title,
            Content = createDto.Content,
            Status = ObrashenieStatus.New,
            Category = analysis.Category,
            LexicomAnalysis = JsonSerializer.Serialize(analysis),
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Obrasheniya.Add(obrashenie);
        await _context.SaveChangesAsync();
        
        return Ok(new ObrashenieDto
        {
            Id = obrashenie.Id,
            Title = obrashenie.Title,
            Content = obrashenie.Content,
            Status = obrashenie.Status,
            Category = obrashenie.Category,
            LexicomAnalysis = obrashenie.LexicomAnalysis,
            CreatedAt = obrashenie.CreatedAt,
            User = new UserDto
            {
                Id = userId,
                Email = User.FindFirst(ClaimTypes.Email)!.Value,
                FullName = User.FindFirst(ClaimTypes.Name)!.Value
            }
        });
    }
    
    [HttpGet]
    public async Task<ActionResult<List<ObrashenieDto>>> GetMyObrasheniya()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var userRole = User.FindFirst(ClaimTypes.Role)!.Value;
        
        IQueryable<Obrashenie> query = _context.Obrasheniya.Include(o => o.User);
        
        if (userRole == UserRole.Citizen.ToString())
        {
            query = query.Where(o => o.UserId == userId);
        }
        
        var obrasheniya = await query
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new ObrashenieDto
            {
                Id = o.Id,
                Title = o.Title,
                Content = o.Content,
                Status = o.Status,
                Category = o.Category,
                LexicomAnalysis = o.LexicomAnalysis,
                CreatedAt = o.CreatedAt,
                User = new UserDto
                {
                    Id = o.User.Id,
                    Email = o.User.Email,
                    FullName = o.User.FullName,
                    Role = o.User.Role
                }
            })
            .ToListAsync();
        
        return Ok(obrasheniya);
    }
    
    [HttpGet("all")]
    [Authorize(Roles = "Operator")]
    public async Task<ActionResult<List<ObrashenieDto>>> GetAllObrasheniya()
    {
        var obrasheniya = await _context.Obrasheniya
            .Include(o => o.User)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new ObrashenieDto
            {
                Id = o.Id,
                Title = o.Title,
                Content = o.Content,
                Status = o.Status,
                Category = o.Category,
                LexicomAnalysis = o.LexicomAnalysis,
                CreatedAt = o.CreatedAt,
                User = new UserDto
                {
                    Id = o.User.Id,
                    Email = o.User.Email,
                    FullName = o.User.FullName,
                    Role = o.User.Role
                }
            })
            .ToListAsync();
        
        return Ok(obrasheniya);
    }
}