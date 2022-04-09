using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Weather.Models;
using Weather.Services;

namespace Weather.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ForecastPage : ContentPage
    {
        OpenWeatherService service;
        GroupedForecast groupedforecast;
        

        public ForecastPage()
        {
            InitializeComponent();
            
            service = new OpenWeatherService();
            groupedforecast = new GroupedForecast();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Code here will run right before the screen appears
            //You want to set the Title or set the City

            //This is making the first load of data
            MainThread.BeginInvokeOnMainThread(async () => {await LoadForecast();});


            //CityName.Text = groupedforecast.City.ToString();

            
        }


        private async void ListViewItemTapped(object sender, ItemTappedEventArgs e)
        {
          //  var item = e.Item as Forecast;
          //  if (item != null) await DisplayAlert("Item tapped", $"Rectangle selected: {item}", "OK");

           // ((ListView)sender).SelectedItem = null;
        }

        
        private async Task LoadForecast()
        {
            //Heare you load the forecast 
            var loadedForecast = await service.GetForecastAsync(this.Title.ToString());
            groupedforecast.City = loadedForecast.City;
            groupedforecast.Items = loadedForecast.Items.GroupBy(item => item.DateTime.Date);
            CityName.Text = groupedforecast.City.ToString();
            CustomGroupedList.ItemsSource = groupedforecast.Items;
            List<ForecastForOneDay> daySumForecastList = new List<ForecastForOneDay>();
            groupedforecast.Items.Take(3).ToList().ForEach(x =>
            {
                daySumForecastList.Add(new ForecastForOneDay { Day = $"{x.Key.DayOfWeek}", Date = $"{x.Key:M}", MaxTemperature = $"{(int)x.Max(m => m.Temperature)}°C", MinTemperature = $"{(int)x.Min(m => m.Temperature)}°C", WindSpeed = $"{x.Max(m => m.WindSpeed)} m/s", Icon = x.SingleOrDefault(s => s.DateTime.Hour == 14).Icon.ToString()});
            });
            CustomList.ItemsSource = daySumForecastList;
            image.Source = $"";
        }
    }
}