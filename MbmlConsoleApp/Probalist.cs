using Microsoft.ML.Probabilistic.Distributions;
using Microsoft.ML.Probabilistic.Models;

namespace MbmlConsoleApp;

internal class Probalist
{
    public static InferenceEngine GetInferenceEngine()
    {
        var inferenceEngine = new InferenceEngine();
        
        // Default inferenceEngine.Compiler.CompilerChoice is 'Microsoft.ML.Probabilistic.Compiler.CompilerChoice.Auto'
        // This will generate the following runtime error:
        // Unhandled exception. System.PlatformNotSupportedException: Current platform is not supported by the current compiler choice Auto. Try a different one.
        inferenceEngine.Compiler.CompilerChoice = Microsoft.ML.Probabilistic.Compiler.CompilerChoice.Roslyn;
        
        return inferenceEngine;
    }

    public static void PrintGaussianArray(Gaussian[] gaussianArray, string? comment)
    {
        if (!string.IsNullOrWhiteSpace(comment)) Console.WriteLine("COMMENT: {0}", comment);
        for (int i = 0; i < gaussianArray.Length; i++)
        {
            Gaussian gaussianValue = gaussianArray[i];
            Console.WriteLine($"Player {i} Skill: Mean={gaussianValue.GetMean()}, Variance={gaussianValue.GetVariance()}");
        }

        // Output:
        //Player 0 Skill: Mean = 6, Variance = 9
        //Player 1 Skill: Mean = 6, Variance = 9
        //Player 2 Skill: Mean = 6, Variance = 9
        //Player 3 Skill: Mean = 6, Variance = 9
        //Player 4 Skill: Mean = 6, Variance = 9
    }
}
