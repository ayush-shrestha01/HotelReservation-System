using Hrs.Api.Repository.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace Hrs.Api.Controllers;

/// <summary>
/// Example controller demonstrating Unit of Work pattern usage.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ExampleController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ExampleController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Example: Get all entities of a specific type.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEntityById(int id)
    {
        // Example: var repository = _unitOfWork.Repository<YourEntity>();
        // var entity = await repository.GetByIdAsync(id);
        // if (entity == null) return NotFound();
        // return Ok(entity);

        return Ok("Replace YourEntity with your actual entity class");
    }

    /// <summary>
    /// Example: Create an entity with transaction management.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateEntity([FromBody] object entity)
    {
        try
        {
            // Begin transaction
            await _unitOfWork.BeginTransactionAsync();

            // Example: var repository = _unitOfWork.Repository<YourEntity>();
            // await repository.AddAsync(newEntity);
            
            // Save changes within transaction
            await _unitOfWork.CommitAsync();

            return CreatedAtAction(nameof(GetEntityById), new { id = "newId" }, entity);
        }
        catch
        {
            // Automatic rollback on exception
            await _unitOfWork.RollbackAsync();
            return StatusCode(500, "An error occurred while creating the entity.");
        }
        finally
        {
            // Dispose resources
            await _unitOfWork.DisposeAsync();
        }
    }

    /// <summary>
    /// Example: Update multiple entities across repositories.
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateMultipleEntities()
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Example: Multiple repository operations
            // var repo1 = _unitOfWork.Repository<Entity1>();
            // var repo2 = _unitOfWork.Repository<Entity2>();
            // 
            // var entity1 = await repo1.GetByIdAsync(1);
            // if (entity1 != null)
            // {
            //     repo1.Update(entity1);
            // }
            //
            // var entity2 = await repo2.GetByIdAsync(1);
            // if (entity2 != null)
            // {
            //     repo2.Update(entity2);
            // }

            await _unitOfWork.CommitAsync();
            return Ok("Multiple entities updated successfully.");
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            return StatusCode(500, "An error occurred while updating entities.");
        }
        finally
        {
            await _unitOfWork.DisposeAsync();
        }
    }
}

