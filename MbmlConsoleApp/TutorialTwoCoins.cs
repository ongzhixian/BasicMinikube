using Microsoft.ML.Probabilistic.Models;

namespace MbmlConsoleApp;

internal class TutorialTwoCoins
{
    public void Run()
    {
        Variable<bool> firstCoin = Variable.Bernoulli(0.5);
        Variable<bool> secondCoin = Variable.Bernoulli(0.5);
        Variable<bool> thirdCoin = Variable.Bernoulli(0.5);

        // Setup Model
        Variable<bool> bothHeads = firstCoin & secondCoin & thirdCoin;
        Variable<bool> twoHeads = secondCoin & thirdCoin;


        // Inference
        InferenceEngine inferenceEngine = Probalist.GetInferenceEngine();

        Console.WriteLine("Probability that both coins are heads: " + inferenceEngine.Infer(bothHeads));
        Console.WriteLine("Probability firstCoin         is head: " + inferenceEngine.Infer(firstCoin));
        Console.WriteLine("Probability secondCoin        is head: " + inferenceEngine.Infer(secondCoin));
        Console.WriteLine("Probability secondCoin        is head: " + inferenceEngine.Infer(twoHeads));

        // 0 0
        // 0 1
        // 1 0
        // 1 1 1/4 = .25

        // 0 0 0
        // 1 0 0
        // 0 0 0
        // 1 1 0
        // 0 1 1
        // 1 1 1
        // 0 0 1
        // 1 0 1

        // 0 1
        // 1 0
        // 1 1 1/4 = .25
    }
}
