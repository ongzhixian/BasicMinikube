using System.Security.Cryptography.X509Certificates;

using Microsoft.ML.Probabilistic.Distributions;
using Microsoft.ML.Probabilistic.Models;

namespace MbmlConsoleApp;

internal class TrueSkillDemo
{
    class Game
    {
        public int Number { get; set; }
        public int Winner { get; set; }
        public int Loser { get; set; }
    }

    public void Run()
    {

        // Initialize inference engine
        InferenceEngine inferenceEngine = Probalist.GetInferenceEngine();

        // The winner and loser in each of 6 samples games (game is 2 player game with total of 5 players (encoded as '0', '1', '2', '3', '4')
        Game[] games = [
            new Game { Number = 1,  Winner = 0, Loser = 1 },
            new Game { Number = 2,  Winner = 0, Loser = 3 },
            new Game { Number = 3,  Winner = 0, Loser = 4 },
            new Game { Number = 4,  Winner = 1, Loser = 2 },
            new Game { Number = 5,  Winner = 3, Loser = 1 },
            new Game { Number = 6,  Winner = 4, Loser = 2 }
            ];

                                                                // Game    1  2  3  4  5  6
        var winnerData = games.Select(r => r.Winner).ToArray(); // new[] { 0, 0, 0, 1, 3, 4 };
        var loserData = games.Select(r => r.Loser).ToArray();   // new[] { 1, 3, 4, 2, 1, 2 };


        // Define the statistical model as a probabilistic program
        var numberOfGames = new Microsoft.ML.Probabilistic.Models.Range(games.Length);
        var allPlayersList = new Microsoft.ML.Probabilistic.Models.Range(winnerData.Concat(loserData).Max() + 1);

        var playerSkills = Variable.Array<double>(allPlayersList); // Array to store result (skill metric of each player)
        playerSkills[allPlayersList] = Variable.GaussianFromMeanAndVariance(6, 9).ForEach(allPlayersList);
        
        Probalist.PrintGaussianArray(inferenceEngine.Infer<Gaussian[]>(playerSkills), "Initial");

        var winners = Variable.Array<int>(numberOfGames);
        var losers = Variable.Array<int>(numberOfGames);

        using (Variable.ForEach(numberOfGames))
        {
            // The player performance is a noisy version of their skill
            var winnerPerformance = Variable.GaussianFromMeanAndVariance(playerSkills[winners[numberOfGames]], 1.0);
            var loserPerformance  = Variable.GaussianFromMeanAndVariance(playerSkills[losers[numberOfGames]], 1.0);

            // The winner performed better in this game
            Variable.ConstrainTrue(winnerPerformance > loserPerformance);
        }

        // Attach the data to the model
        winners.ObservedValue = winnerData;
        losers.ObservedValue = loserData;


        // Run inference
        var inferredSkills = inferenceEngine.Infer<Gaussian[]>(playerSkills);

        // The inferred skills are uncertain, which is captured in their variance
        var orderedPlayerSkills = inferredSkills
            .Select((s, i) => new { Player = i, Skill = s })
            .OrderByDescending(ps => ps.Skill.GetMean());

        foreach (var playerSkill in orderedPlayerSkills)
        {
            Console.WriteLine($"Player {playerSkill.Player} skill: {playerSkill.Skill}");
        }

    }

}
