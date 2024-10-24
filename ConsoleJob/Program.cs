
//int it = 0;
//int count(int[] coins, int n, int sum)
//{

//    if (sum == 0)
//    {
//        Console.WriteLine($"{it++}: n = {n}, sum = {sum}, result = 1");
//        return 1;
//    }

//    if (sum < 0)
//    {
//        Console.WriteLine($"{it++}: n = {n}, sum = {sum}, result = 0");
//        return 0;
//    }
//    if (n <= 0)
//    {
//        Console.WriteLine($"{it++}: n = {n}, sum = {sum}, result = 0");
//        return 0;
//    }

//    var solnCount = count(coins, n - 1, sum) + count(coins, n, sum - coins[n - 1]);
//    Console.WriteLine($"{it++}: n = {n}, sum = {sum}, result = {count}, coin = {coins[n - 1]}");
//    return solnCount;
//}

////int[] coins = [5, 2, 4];
////int targetAmount = 13;

//int[] coins = [1, 2, 3];
//int targetAmount = 4;



//var solutions = count(coins, coins.Length, targetAmount); // Given n coins, find target amount

//Console.WriteLine($"Solutions {solutions}");

//// count(coins,     n, sum) // Given n coins, find target amount
//// count(coins, n - 1, sum) // Given n-1 coins, find target amount
////                ...,
//// count(coins,     0, sum) // Given n==0 coins, no coins, we cannot possible have any solution (except is target sum is 0)


//// count(coins, n,                sum) // Given n coins, find target amount
//// count(coins, n, sum - coins[n - 1]) // Given n coins, find target amount 
///

using System;
using System.Collections.Generic;

class CoinChangeOptimized
{
    public static List<List<int>> FindAllSequences(int amount, int[] coins)
    {
        int n = coins.Length;
        int[] dp = new int[amount + 1];
        int[,] lastCoin = new int[amount + 1, n];

        // Initialize dp array
        for (int i = 1; i <= amount; i++)
        {
            dp[i] = int.MaxValue;
        }

        // Fill dp array
        for (int i = 0; i < n; i++)
        {
            for (int j = coins[i]; j <= amount; j++)
            {
                if (dp[j - coins[i]] != int.MaxValue && dp[j] > dp[j - coins[i]] + 1)
                {
                    dp[j] = dp[j - coins[i]] + 1;
                    lastCoin[j, i] = 1;
                }
                else if (dp[j - coins[i]] != int.MaxValue && dp[j] == dp[j - coins[i]] + 1)
                {
                    lastCoin[j, i] = 1;
                }
            }
        }

        // Reconstruct sequences
        List<List<int>> sequences = new List<List<int>>();
        if (dp[amount] != int.MaxValue)
        {
            ReconstructSequences(coins, lastCoin, amount, new List<int>(), sequences);
        }

        return sequences;
    }

    private static void ReconstructSequences(int[] coins, int[,] lastCoin, int amount, List<int> currentSequence, List<List<int>> sequences)
    {
        if (amount == 0)
        {
            sequences.Add(new List<int>(currentSequence));
            return;
        }

        for (int i = 0; i < coins.Length; i++)
        {
            if (lastCoin[amount, i] == 1)
            {
                currentSequence.Add(coins[i]);
                ReconstructSequences(coins, lastCoin, amount - coins[i], currentSequence, sequences);
                currentSequence.RemoveAt(currentSequence.Count - 1);
            }
        }
    }

    public static void Main(string[] args)
    {
        int amount = 4;
        int[] coins = { 1, 2, 3 };

        var allSequences = FindAllSequences(amount, coins);

        Console.WriteLine("All possible sequences:");
        foreach (var sequence in allSequences)
        {
            Console.WriteLine(string.Join(", ", sequence));
        }
    }
}