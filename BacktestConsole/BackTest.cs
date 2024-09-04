using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PricingLibrary.DataClasses;
using PricingLibrary.MarketDataFeed;

namespace BacktestConsole
{
    public class BackTestConsole
    {
        // Method to run the hedging process and write results to an output file
        public static void RunBacktest(string parametersPath, string marketDataPath, string outputFilePath)
        {
            try
            {
                // Load test parameters and market data
                BasketTestParameters testParams = ParameterLoader.LoadTestParameters(parametersPath);
                List<DataFeed> marketData = MarketDataLoader.LoadMarketData(marketDataPath);

                var portfolioHedging = new PortfolioHedging(testParams, marketData);
                List<OutputData> outputDataList = portfolioHedging.Hedger();

                // Serialize the OutputData list to JSON and save to the specified output file
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Ensure field names are camelCase
                };
                string jsonOutput = JsonSerializer.Serialize(outputDataList, options);
                File.WriteAllText(outputFilePath, jsonOutput);

                Console.WriteLine($"Output written to {outputFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
