using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ManagerProject.Data;
using System.ComponentModel.DataAnnotations;

[Route("api/projects")]
[ApiController]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProjectsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // CREATE
    [HttpPost("")]
    public async Task<IActionResult> CreateProject([FromBody] ProjectClass request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var project = new Project
        {
            Name = request.ProjectName,
            Description = request.Description,
            StartDate = request.StartDate ?? DateTime.Now,
            CreatedBy = int.Parse(userIdClaim.Value),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.Projects.Add(project);
        var affectedRows = await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Tạo dự án thành công!",
            AffectedRows = affectedRows,
            Project = project
        });
    }

    // READ ALL
    [HttpGet("")]
    public async Task<IActionResult> GetAllProjects()
    {
        var projects = await _context.Projects
            .Include(p => p.Creator)
            .ToListAsync();
        return Ok(projects);
    }

    // READ BY ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectById(int id)
    {
        var project = await _context.Projects
            .Include(p => p.Creator)
            .FirstOrDefaultAsync(p => p.ProjectID == id);

        if (project == null)
            return NotFound();

        return Ok(project);
    }

    // UPDATE
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectClass request)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return NotFound();

        project.Name = request.ProjectName;
        project.Description = request.Description;
        project.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Cập nhật thành công", Project = project });
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return NotFound();

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Xoá thành công" });
    }
}
public class ProjectClass
{
    [Required(ErrorMessage = "Tên dự án không được để trống.")]
    public string ProjectName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime? StartDate { get; set; }
}
