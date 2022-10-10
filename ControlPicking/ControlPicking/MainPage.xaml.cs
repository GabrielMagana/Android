using Xamarin.Forms;

namespace ControlPicking
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }


        protected bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var result = await this.DisplayAlert("Alerta", "Deseas salir de la aplicación?", "Si", "No");
                if (result) await this.Navigation.PopAsync();
            });
            return true;
        }
    }
}
