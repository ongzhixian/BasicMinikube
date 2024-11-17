


//unchecked
//{
//    int r = int.MaxValue + 1;
//    Console.WriteLine($"Max value is {int.MaxValue}, {r}");
//}


int a = int.MaxValue;

try
{
    checked
    {
        int r = a + 1;
        Console.WriteLine($"Max value is {int.MaxValue}, {r}");
    }
}
catch (OverflowException e)
{
    Console.WriteLine(e.Message);  // output: Arithmetic operation resulted in an overflow.
}