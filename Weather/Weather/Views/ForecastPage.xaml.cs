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
        Forecast loadedForecast;



        public ForecastPage()
        {
            InitializeComponent();
            
            service = new OpenWeatherService();
            groupedforecast = new GroupedForecast();
            loadedForecast = new Forecast();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Code here will run right before the screen appears
            //You want to set the Title or set the City

            //This is making the first load of data
            MainThread.BeginInvokeOnMainThread(async () => {await LoadForecast();});

        }



        
        private async Task LoadForecast()
        {
            //Load forecast. 
            loadedForecast = await service.GetForecastAsync(Title.ToString());
            groupedforecast.City = loadedForecast.City;
            groupedforecast.Items = loadedForecast.Items.GroupBy(item => item.DateTime.Date);
            


            //Update labels in xalm.
            CityName.Text = $"Weather forecast for {groupedforecast.City}";
            WeatherForecastList.ItemsSource = groupedforecast.Items;
            List<ForecastForOneDay> daySumForecastList = new List<ForecastForOneDay>();
            groupedforecast.Items.Skip(1).Take(3).ToList().ForEach(x =>
            {
                daySumForecastList.Add(new ForecastForOneDay { Day = $"{x.Key.DayOfWeek}", Date = $"{x.Key:M}", MaxTemperature = $"{(int)x.Max(m => m.Temperature)}°C", MinTemperature = $"{(int)x.Min(m => m.Temperature)}°C", WindSpeed = $"{x.Max(m => m.WindSpeed)} m/s", Icon = x.SingleOrDefault(s => s.DateTime.Hour == 14).Icon.ToString()});
            });
            ThreeDayWeatherList.ItemsSource = daySumForecastList;
            CurrentWeatherUpdate(0);
        }

        

        private void slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var pic = sender as Slider;
            if (pic != null)
                CurrentWeatherUpdate((int)pic.Value);
        }

        private void CurrentWeatherUpdate(int index)
        {
            wind.Text = $"Wind speed: {loadedForecast.Items[index].WindSpeed} m/s";
            Temp.Text = $"Temperature: {(int)loadedForecast.Items[index].Temperature}°C";
            description.Text = $"{loadedForecast.Items[index].Description}";
            currentWeather.Source = $"{loadedForecast.Items[index].Icon}";
            TimeNow.Text = $"The weather {loadedForecast.Items[index].DateTime:f}";
        }

        private void ThreeDayWeatherList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            int thisDay = groupedforecast.Items.First().Count();

            if (e.SelectedItemIndex == 0) { CurrentWeatherUpdate(thisDay + 4); slider.Value = thisDay + 4; }
            if (e.SelectedItemIndex == 1) { CurrentWeatherUpdate(thisDay + 12); slider.Value = thisDay + 12; }
            if (e.SelectedItemIndex == 2) { CurrentWeatherUpdate(thisDay + 20); slider.Value = thisDay + 20; }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
           await LoadForecast();
        }
    }
}