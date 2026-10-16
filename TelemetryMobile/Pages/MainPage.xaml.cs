using TelemetryMobile.Models;
using TelemetryMobile.PageModels;

namespace TelemetryMobile.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}