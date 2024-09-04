using System;

namespace BacktestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Check if the correct number of arguments is provided
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: BacktestConsole.exe <test-params> <mkt-data> <output-file>");
                return;
            }

            // Parse command line arguments
            string parametersPath = args[0]; // Path to the JSON file containing the test parameters
            string marketDataPath = args[1]; // Path to the CSV file containing the market data
            string outputFilePath = args[2]; // Path to the output JSON file

            // Run the backtest process
            BackTestConsole.RunBacktest(parametersPath, marketDataPath, outputFilePath);
        }
    }
}
