using Microsoft.ML.Probabilistic.Math;
using Microsoft.ML.Probabilistic.Models;

namespace MbmlConsoleApp;

internal class LearningGaussian
{
    public void Run()
    {
        InferenceEngine inferenceEngine = Probalist.GetInferenceEngine();

        // Sample data from standard Gaussian  
        double[] data = new double[100];
        for (int i = 0; i < data.Length; i++)
            data[i] = Rand.Normal(0, 1);

        // Create mean and precision random variables  
        Variable<double> mean = Variable.GaussianFromMeanAndVariance(0, 100);
        Variable<double> precision = Variable.GammaFromShapeAndScale(1, 1);

        //for (int i = 0; i < data.Length; i++)
        //{
        //    Variable<double> x = Variable.GaussianFromMeanAndPrecision(mean, precision);
        //    x.ObservedValue = data[i];
        //}

        Microsoft.ML.Probabilistic.Models.Range dataRange = new Microsoft.ML.Probabilistic.Models.Range(data.Length).Named("n");
        VariableArray<double> x = Variable.Array<double>(dataRange);
        x[dataRange] = Variable.GaussianFromMeanAndPrecision(mean, precision).ForEach(dataRange);
        x.ObservedValue = data;



        Console.WriteLine("mean=" + inferenceEngine.Infer(mean));
        Console.WriteLine("prec=" + inferenceEngine.Infer(precision));

    }
}
