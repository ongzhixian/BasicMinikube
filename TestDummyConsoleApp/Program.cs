Console.WriteLine("[PROGRAM START]");

CancellationTokenSource cts = new();
CancellationToken cancellationToken = cts.Token;


Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs e) =>
{
    Console.WriteLine("Cancelling...");
    cts.Cancel();
    e.Cancel = true;
};

while (!cancellationToken.IsCancellationRequested)
{
    try
    {
        await Task.Delay(3000, cancellationToken);
        Console.WriteLine($"Hello, World announced at {DateTime.Now:O}");
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine("TaskCanceledException");
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("OperationCanceledException");
    }
    catch (Exception)
    {
        throw;
    }
}
Console.WriteLine("[PROGRAM END]");
