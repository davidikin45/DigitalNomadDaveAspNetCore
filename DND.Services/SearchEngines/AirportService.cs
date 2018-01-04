using DND.Services.Skyscanner.ModelImport;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

//http://www.partow.net/miscellaneous/airportdatabase/
namespace DND.Services
{
    public class AirportService
    {
        private static object lockObject = new object();
        private static Airports.RootObject AirportsObject = null;
        private static Dictionary<string, Airports.Airport> AirportsDict = null;

        public Airports.Airport GetByIATA(string IATA)
        {
            return null;
            Load();
            if (AirportsDict.ContainsKey(IATA))
            {
                return AirportsDict[IATA];
            }
            else 
            {
                return null;
            }
        }

        public static void Load()
        {
            if (AirportsObject == null)
            {
                lock (lockObject)
                {
                    if (AirportsObject == null)
                    {
                        //string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        //string location = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Airports.json");
                        string location =  @"C:\Development\DND\DND\App_Data\Airports.json";
                        AirportsObject = JsonConvert.DeserializeObject<Airports.RootObject>(File.ReadAllText(location));
                        AirportsDict = new Dictionary<string, Airports.Airport>();
                        foreach (Airports.Airport airport in AirportsObject.Airports)
                        {
                            AirportsDict.Add(airport.IATA, airport);

                        }
                    }
                }
            }       
        }
    }
}
