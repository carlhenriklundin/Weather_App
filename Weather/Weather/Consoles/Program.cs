using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Json; //Requires nuget package System.Net.Http.Json
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;

using Weather.Models;
using Weather.Services;

namespace Weather.Consoles
{
    //Your can move your Console application Main here. Rename Main to myMain and make it NOT static and async
    class Program
    {
        #region used by the Console
        Views.ConsolePage theConsole;
        StringBuilder theConsoleString;
        public Program(Views.ConsolePage myConsole)
        {
            //used for the Console
            theConsole = myConsole;
            theConsoleString = new StringBuilder();
            
        }
        #endregion

        #region Console Demo program
        //This is the method you replace with your async method renamed and NON static Main
        public async Task myMain()
        {
            
            OpenWeatherService service = new OpenWeatherService();
            service.WeatherForecastAvailable += ReportWeatherDataAvailable;
            List<Forecast> forecasts = new List<Forecast>();

            Exception exception = null;
            try
            {
                double latitude = 59.5086798659495;
                double longitude = 18.2654625932976;

                forecasts.Add(await service.GetForecastAsync(latitude, longitude));
                theConsole.WriteLine(forecasts[0].City);
                forecasts.Add(await service.GetForecastAsync("Miami"));
                theConsole.WriteLine(forecasts[1].City);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            foreach (var forecast in forecasts)
            {
                theConsoleString.AppendLine("-------------------");

                if (forecast != null)
                {
                    theConsoleString.AppendLine($"Weather forecast for {forecast.City}");
                    var GroupedList = forecast.Items.GroupBy(item => item.DateTime.Date);

                    foreach (var group in GroupedList)
                    {
                        theConsoleString.AppendLine(group.Key.Date.ToShortDateString());

                        foreach (var item in group)
                        {
                            theConsoleString.AppendLine($"   -  {item.DateTime.ToShortTimeString()}: {item.Description}, temperature: {item.Temperature} degC, wind: {item.WindSpeed} m/s");
                        }
                    }
                }
                else
                {
                    theConsoleString.AppendLine("Geolocation weater service error.");
                    theConsoleString.AppendLine($"Error: {exception.Message}");
                }
            }

            theConsole.WriteLine(theConsoleString.ToString());






        }

        //If you have any event handlers, they could be placed here
        void ReportWeatherDataAvailable(object sender, string message)
        {
            theConsole.WriteLine($"Event message: {message}"); //theConsole is a Captured Variable, don't use myConsoleString here
        }
        #endregion
    }
}
