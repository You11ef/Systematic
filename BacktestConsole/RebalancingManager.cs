using System;
using PricingLibrary.RebalancingOracleDescriptions;
using PricingLibrary.TimeHandler;

namespace BacktestConsole
{
    /// <summary>
    /// Manages the rebalancing process for the portfolio.
    /// </summary>
    public class RebalancingManager
    {
        private readonly IRebalancingOracleDescription _oracle;
        public DateTime _lastRebalanceDate;

        /// <summary>
        /// Initializes a new instance of the RebalancingManager class.
        /// </summary>
        /// <param name="oracle">The rebalancing oracle description.</param>
        /// <param name="initialDate">The initial date for rebalancing.</param>
        public RebalancingManager(IRebalancingOracleDescription oracle, DateTime initialDate)
        {
            _oracle = oracle;
            _lastRebalanceDate = initialDate;
        }

        /// <summary>
        /// Determines whether the portfolio should be rebalanced on the given date.
        /// </summary>
        /// <param name="currentDate">The current date to check for rebalancing.</param>
        /// <returns>True if rebalancing should occur, false otherwise.</returns>
        public bool ShouldRebalance(DateTime currentDate)
        {
            double landmark = MathDateConverter.ConvertToMathDistance(new DateTime(1, 1, 1, 0, 0, 0), new DateTime(1, 1, 2, 0, 0, 0));
            double distance = MathDateConverter.ConvertToMathDistance(_lastRebalanceDate, currentDate) / landmark;

            bool shouldRebalance = false;

            if (_oracle is RegularOracleDescription regularOracle)
            {
                shouldRebalance = distance >= regularOracle.Period;
            }
            else if (_oracle is WeeklyOracleDescription weeklyOracle)
            {
                shouldRebalance = currentDate.DayOfWeek == weeklyOracle.RebalancingDay;
            }

            return shouldRebalance;
        }
    }
}