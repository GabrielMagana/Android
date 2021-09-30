using ControlPicking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ControlPicking
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

     
        protected override bool OnBackButtonPressedAsync()
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
