using CashBackMerge.Game.Configuration;
using CashBackMerge.Game.Models;
using CashBackMerge.Game.Services;
using CashBackMerge.Simulator.Configuration;
using CashBackMerge.Simulator.Models;
using CashBackMerge.Simulator.Strategies;

namespace CashBackMerge.Simulator.Services;

public class CampaignSimulator
{
    private readonly CampaignConfig _campaignConfig;
    private readonly GameConfig _gameConfig;
    private readonly int _seed;

    public CampaignSimulator(
        CampaignConfig campaignConfig, 
        GameConfig gameConfig,
        int seed)
    {
        CampaignConfigValidator.Validate(campaignConfig);
        GameConfigValidator.Validate(gameConfig);

        _campaignConfig = campaignConfig;
        _gameConfig = gameConfig;
        _seed = seed;
    }

    public CampaignStatistics Run()
    {
        var gameStrategy = StrategyMapper.Get(_campaignConfig.StrategyName);
        var gameFactory = new GameFactory(_gameConfig, _seed);

        var rewardDistribution = new Dictionary<int, int>();
        var (totalScore, maxScore, totalReward) = (0, 0, 0);

        for (var i = 0; i < _campaignConfig.PlayersCount; i++)
        {
            var session = gameFactory.CreateNewGame();

            while (session.Status == GameStatus.InProgress)
                session.ApplyMove(gameStrategy.GetNextMove(session));

            var (score, reward) = (session.Score, session.Reward);

            totalScore += score;
            totalReward += reward;
            maxScore = Math.Max(maxScore, score);
            rewardDistribution[reward] = rewardDistribution.GetValueOrDefault(reward) + 1;
        }

        return new CampaignStatistics(
            GameConfig: _gameConfig,
            CampaignConfig: _campaignConfig,
            AverageScore: (decimal)totalScore / _campaignConfig.PlayersCount,
            MaxScore: maxScore,
            AverageReward: (decimal)totalReward / _campaignConfig.PlayersCount,
            RewardDistribution: rewardDistribution
        );
    }
}
