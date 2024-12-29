using Microsoft.ML.Probabilistic.Algorithms;
using Microsoft.ML.Probabilistic.Models;

namespace MbmlConsoleApp;

internal class TruncatedGaussian
{
    public void Run()
    {
        InferenceEngine inferenceEngine = Probalist.GetInferenceEngine();
        inferenceEngine.Algorithm = new ExpectationPropagation();


        //for (double thresh = 0; thresh <= 1; thresh += 0.1)
        //{
        //    Variable<double> x = Variable.GaussianFromMeanAndVariance(0, 1).Named("x");
        //    Variable.ConstrainTrue(x > thresh);
        //    Console.WriteLine("Dist over x given thresh of " + thresh + "=" + inferenceEngine.Infer(x));
        //}


        Variable<double> threshold = Variable.New<double>().Named("threshold");
        Variable<double> x = Variable.GaussianFromMeanAndVariance(0, 1).Named("x");
        Variable.ConstrainTrue(x > threshold);

        for (double thresh = 0; thresh <= 1; thresh += 0.1)
        {
            threshold.ObservedValue = thresh;
            Console.WriteLine("Dist over x given thresh of " + thresh + "=" + inferenceEngine.Infer(x));
        }
    }
}
