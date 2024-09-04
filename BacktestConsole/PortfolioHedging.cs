// PortfolioHedging.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PricingLibrary.Computations;
using PricingLibrary.DataClasses;
using PricingLibrary.MarketDataFeed;
using PricingLibrary.RebalancingOracleDescriptions;
using PricingLibrary.TimeHandler;

namespace BacktestConsole
{
    /// <summary>
    /// Manages the portfolio hedging process, including initialization, updating, and rebalancing.
    /// </summary>
    public class PortfolioHedging
    {
        private readonly Pricer _pricer;
        private readonly IRebalancingOracleDescription _oracle;
        private readonly RebalancingManager _rebalancingManager;
        private readonly List<DataFeed> _marketData;
        private List<OutputData> _outputDataList;
        private Portfolio _portfolio;

        /// <summary>
        /// Initializes a new instance of the PortfolioHedging class.
        /// </summary>
        /// <param name="testParams">Parameters for the basket test.</param>
        /// <param name="marketData">List of market data feeds.</param>
        public PortfolioHedging(BasketTestParameters testParams, List<DataFeed> marketData)
        {
            _pricer = new Pricer(testParams);
            _oracle = testParams.RebalancingOracleDescription;
            _marketData = marketData;
            _rebalancingManager = new RebalancingManager(_oracle, _marketData[0].Date);
            _outputDataList = new List<OutputData>(); // Initialize the output data list here
            _portfolio = new Portfolio(testParams.BasketOption.UnderlyingShareIds); // Will be initialized in the InitializePortfolio()
        }

        /// <summary>
        /// Performs the hedging process and returns a list of output data.
        /// </summary>
        /// <returns>A list of OutputData containing the hedging results.</returns>
        public List<OutputData> Hedger()
        {
            InitializePortfolio(_marketData[0]);
            for (int i = 1; i < _marketData.Count; i++)
            {
                UpdatePortfolio(_marketData[i], _portfolio);
                if (_rebalancingManager.ShouldRebalance(_marketData[i].Date))
                {
                    RebalancePortfolio(_marketData[i], _portfolio);
                }
            }

            return _outputDataList;
        }

        /// <summary>
        /// Initializes the portfolio state with the initial market data.
        /// </summary>
        /// <param name="initialDataFeed">The initial market data feed.</param>
        private void InitializePortfolio(DataFeed initialDataFeed)
        {
            double[] spots = initialDataFeed.PriceList.Values.ToArray();
            var pricingResults = _pricer.Price(initialDataFeed.Date, spots);
            double cash = pricingResults.Price - MathUtils.DotProduct(pricingResults.Deltas, spots);
            _portfolio.UpdateState(cash, pricingResults.Deltas);
            _outputDataList.Add(CreateOutputData(initialDataFeed.Date, _portfolio, spots, pricingResults));
        }

        /// <summary>
        /// Updates the portfolio state with new market data.
        /// </summary>
        /// <param name="currentDataFeed">The current market data feed.</param>
        /// <param name="currentState">The current portfolio state.</param>
        private void UpdatePortfolio(DataFeed currentDataFeed, Portfolio portfolio)
        {
            double[] spots = currentDataFeed.PriceList.Values.ToArray();
            var pricingResults = _pricer.Price(currentDataFeed.Date, spots);
            _portfolio.UpdateState(portfolio.Cash, portfolio.Composition.Values.ToArray());
        }

        /// <summary>
        /// Rebalances the portfolio based on the current market data and portfolio state.
        /// </summary>
        /// <param name="currentDataFeed">The current market data feed.</param>
        /// <param name="currentState">The current portfolio state.</param>
        private void RebalancePortfolio(DataFeed currentDataFeed, Portfolio portfolio)
        {
            double[] spots = currentDataFeed.PriceList.Values.ToArray();
            var pricingResults = _pricer.Price(currentDataFeed.Date, spots);
            double riskFreeRateAccrued = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(_rebalancingManager._lastRebalanceDate, currentDataFeed.Date);
            double updatedCash = portfolio.Cash * riskFreeRateAccrued
                                 + MathUtils.DotProduct(portfolio.Composition.Values.ToArray(), spots)
                                 - MathUtils.DotProduct(pricingResults.Deltas, spots);

            _portfolio.UpdateState(updatedCash, pricingResults.Deltas);

            _rebalancingManager._lastRebalanceDate = currentDataFeed.Date;
            _outputDataList.Add(CreateOutputData(currentDataFeed.Date, _portfolio, spots, pricingResults));
        }

        /// <summary>
        /// Creates output data based on the current date, portfolio state, and pricing results.
        /// </summary>
        /// <param name="date">The current date.</param>
        /// <param name="state">The current portfolio state.</param>
        /// <param name="spots">The current spot prices of the underlying assets.</param>
        /// <param name="pricingResults">The current pricing results.</param>
        /// <returns>An OutputData object containing the current portfolio information.</returns>
        private OutputData CreateOutputData(DateTime date, Portfolio portfolio, double[] spots, PricingResults pricingResults)
        {
            double portfolioValue = portfolio.Cash + MathUtils.DotProduct(pricingResults.Deltas, spots);

            return new OutputData
            {
                Date = date,
                Value = portfolioValue,
                Deltas = pricingResults.Deltas,
                DeltasStdDev = pricingResults.DeltaStdDev,
                Price = pricingResults.Price,
                PriceStdDev = pricingResults.PriceStdDev
            };
        }
    }
}
