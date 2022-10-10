using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ControlPicking.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class frmCase : ContentPage
    {

        string equipo, Usuario;

        public frmCase(string usuario)
        {
            InitializeComponent();
            equipo = DeviceInfo.Name;
            Usuario = usuario;
        }

        //protected override bool OnBackButtonPressedAsync()
        //{
        //    Device.BeginInvokeOnMainThread(async () =>
        //    {
        //        var result = await this.DisplayAlert("Alerta", "Deseas salir de la aplicación?", "Si", "No").ConfigureAwait(false);
        //        if (result) System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
        //    });
        //    txtCase.Text = "";
        //    txtCase.Focus();
        //    return true;
        //}
        private async void txtCase_Completed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCase.Text))
            {
                await Navigation.PushAsync(new OrderPick(txtCase.Text, equipo, int.Parse(Usuario)));
            }
        }
    }
}