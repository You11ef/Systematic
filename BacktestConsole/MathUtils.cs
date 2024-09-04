using System.Linq;

namespace BacktestConsole
{
    /// <summary>
    /// Provides utility mathematical functions for the portfolio hedging process.
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// Calculates the dot product of two arrays.
        /// </summary>
        /// <param name="a">The first array.</param>
        /// <param name="b">The second array.</param>
        /// <returns>The dot product of the two arrays.</returns>
        public static double DotProduct(double[] a, double[] b)
        {
            return a.Zip(b, (x, y) => x * y).Sum();
        }
    }
}