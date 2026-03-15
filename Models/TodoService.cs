using MongoDB.Driver;
using TodoApi.Models;
using Microsoft.Extensions.Options;

namespace TodoApi.Services;

public class TodoService
{
    private readonly IMongoCollection<TodoItem> _todoCollection;

    public TodoService(IConfiguration config)
    {
        var client = new MongoClient(config.GetSection("TodoDatabase:ConnectionString").Value);
        var database = client.GetDatabase(config.GetSection("TodoDatabase:DatabaseName").Value);
        _todoCollection = database.GetCollection<TodoItem>(config.GetSection("TodoDatabase:CollectionName").Value);
    }

    public async Task<List<TodoItem>> GetAsync() =>
        await _todoCollection.Find(_ => true).ToListAsync();

    public async Task<TodoItem?> GetAsync(string id) =>
        await _todoCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(TodoItem newTodo) =>
        await _todoCollection.InsertOneAsync(newTodo);

    public async Task UpdateAsync(string id, TodoItem updatedTodo) =>
        await _todoCollection.ReplaceOneAsync(x => x.Id == id, updatedTodo);

    public async Task RemoveAsync(string id) =>
        await _todoCollection.DeleteOneAsync(x => x.Id == id);
}