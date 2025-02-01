
using Microsoft.AspNetCore.Mvc;

namespace MobileWebApp.Models;

public class OperationModels
{
    void DoWork()
    {
        OperationResult operation= new OperationResult
        {
            IsSuccess = true,
            Message = "Item is added"
        };


        if (operation.IsSuccess)
        {
        }
    }
}

public interface IOperationResult
{
    bool Equals(object? obj);
    bool Equals(OperationResult? other);
    int GetHashCode();
    string ToString();
}


public record OperationResult : IOperationResult
{
    public string OperationName { get; set; } = null!;

    public bool IsSuccess { get; set; }

    public string? Message { get; set; } // Report what has been done on Success; Or reason for failure
}

public record SuccessResult : OperationResult
{
    public SuccessResult(string operationName, string? message = null)
    {
        this.OperationName = operationName;
        this.IsSuccess = true;
        this.Message = message;
    }
}

public record FailureResult : OperationResult
{
    public FailureResult(string operationName, string? message = null)
    {
        OperationName = operationName;
        IsSuccess = false;
        Message = message;
    }
}


// Operation that affects records
public record RecordsOperationResult
{
    public bool IsSuccess;

    public string? Message;
}