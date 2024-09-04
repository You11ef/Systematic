using System.Globalization;
using CsvHelper;
using PricingLibrary.MarketDataFeed; // Utiliser DataFeed de PricingLibrary
using PricingLibrary.DataClasses;
using PricingLibrary.RebalancingOracleDescriptions;
using System.Text.Json.Serialization;
using System.Text.Json;


namespace BacktestConsole
{
    public static class ParameterLoader
    {
        // Fonction pour charger les paramètres de test depuis un fichier JSON
        public static BasketTestParameters LoadTestParameters(string filePath)
        {
            var json = File.ReadAllText(filePath); // Lire le contenu du fichier JSON
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(), new RebalancingOracleDescriptionConverter() }
            };
            return JsonSerializer.Deserialize<BasketTestParameters>(json, options) ?? throw new InvalidOperationException("Unexpected null value"); // Désérialiser le JSON en objet BasketTestParameters
        }
    }


    public static class MarketDataLoader
    {
        // Fonction pour charger les données de marché depuis un fichier CSV
        public static List<DataFeed> LoadMarketData(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // Lecture des enregistrements CSV comme objets dynamiques
                var records = csv.GetRecords<dynamic>().ToList();

                // Utilisation de LINQ pour grouper et convertir les données en objets DataFeed
                var marketData = records
                    .GroupBy(
                        d => DateTime.ParseExact((string)d.DateOfPrice, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture), // Grouper par DateOfPrice avec conversion explicite en string
                        t => new { Symb = ((string)t.Id).Trim(), Val = double.Parse((string)t.Value, CultureInfo.InvariantCulture) }, // Convertir Id en string et Value en double
                        (key, g) => new DataFeed(
                            key,
                            g.ToDictionary(
                                e => e.Symb, // Clé du dictionnaire : string
                                e => e.Val  // Valeur du dictionnaire : double
                            )
                        )
                    )
                    .ToList(); // Convertir en liste de DataFeed

                return marketData;
            }
        }
    }





}
