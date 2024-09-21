using TaskListWebApp.Models;

namespace TaskListWebApp.Intents;

public interface ITaskListQueries
{
    Task<IEnumerable<TodoTask>> GetTodoTaskListAsync(string? filterCriteria);
}

public interface ITaskListCommands
{
    Task AddTodoTaskAsync(string taskDescription);

    void MarkAllTodoTaskAsComplete();

    //Task ToggleTodoTaskCompleteAsync(int id, bool flagValue);
    Task ToggleTodoTaskCompleteAsync(int id, bool flagValue);

    void EditTodoTask(int id);
    Task DeleteTodoTaskAsync(int id);

    void CancelEditTodoTask(int id);
    Task UpdateTodoTaskAsync(int id);
}
