using System;
using System.Linq;
using System.Collections.Generic;

namespace Weather.Models
{
    public class Forecast
    {
        public string City { get; set; }
        public List<ForecastItem> Items { get; set; }
    }

    public class GroupedForecast
    {
        public string City { get; set; }
        public IEnumerable<IGrouping<DateTime, ForecastItem>> Items { get; set; }
    }

    public class ForecastForOneDay
    {
        public string Day { get; set; }
        public string Date { get; set; }
        public string MaxTemperature { get; set; }
        public string MinTemperature { get; set; }
        public string WindSpeed { get; set; }
    }

}
