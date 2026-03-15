using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Pour la sécurité
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TodoItemsController : ControllerBase
{
    private readonly TodoService _todoService;

    public TodoItemsController(TodoService todoService)
    {
        _todoService = todoService;
    }

    // GET: api/TodoItems
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
    {
        var items = await _todoService.GetAsync();
        return items.Select(x => ItemToDTO(x)).ToList();
    }

    // GET: api/TodoItems/5
    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<TodoItemDTO>> GetTodoItem(string id)
    {
        var todoItem = await _todoService.GetAsync(id);

        if (todoItem == null) return NotFound();

        return ItemToDTO(todoItem);
    }

    // PUT: api/TodoItems/5
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> PutTodoItem(string id, TodoItemDTO todoDTO)
    {
        if (id != todoDTO.Id) return BadRequest();

        var todoItem = await _todoService.GetAsync(id);
        if (todoItem == null) return NotFound();

        todoItem.Name = todoDTO.Name;
        todoItem.IsComplete = todoDTO.IsComplete;

        await _todoService.UpdateAsync(id, todoItem);

        return NoContent();
    }

    // POST: api/TodoItems
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<TodoItemDTO>> PostTodoItem(TodoItemDTO todoDTO)
    {
        var todoItem = new TodoItem
        {
            Name = todoDTO.Name,
            IsComplete = todoDTO.IsComplete
        };

        await _todoService.CreateAsync(todoItem);

        return CreatedAtAction(
            nameof(GetTodoItem),
            new { id = todoItem.Id },
            ItemToDTO(todoItem));
    }

    // DELETE: api/TodoItems/5
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> DeleteTodoItem(string id)
    {
        var todoItem = await _todoService.GetAsync(id);
        if (todoItem == null) return NotFound();

        await _todoService.RemoveAsync(id);
        return NoContent();
    }

    private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
       new TodoItemDTO
       {
           Id = todoItem.Id,
           Name = todoItem.Name,
           IsComplete = todoItem.IsComplete
       };
}