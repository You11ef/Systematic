using System;
using System.Collections.Generic;
using System.Linq;

namespace BacktestConsole
{
    /// <summary>
    /// Represents the current state of the portfolio.
    /// </summary>
    public class Portfolio
    {
        /// <summary>
        /// Gets or sets the current cash position of the portfolio.
        /// </summary>
        public double Cash { get; set; }

        /// <summary>
        /// Gets or sets the current composition of the portfolio.
        /// </summary>
        public Dictionary<string, double> Composition { get; set; } = new Dictionary<string, double>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PortfolioState"/> class with the specified share names.
        /// </summary>
        /// <param name="sharesNames">An array of share names to initialize the composition.</param>
        public Portfolio(string[] sharesNames)
        {
            Composition = sharesNames.ToDictionary(name => name, name => 0.0);
        }

        /// <summary>
        /// Updates the cash and composition values of the portfolio.
        /// </summary>
        /// <param name="newCash">The new cash amount to set.</param>
        /// <param name="newValues">An array of doubles containing the new values for the composition.</param>
        public void UpdateState(double newCash, double[] newValues)
        {
            Cash = newCash;
            Composition = Composition.Keys.Zip(
                newValues,  
                (key, value) => new { key, value })  
                .ToDictionary(x => x.key, x => x.value);  
        }
    }
}
