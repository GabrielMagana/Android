using ControlPicking.Services;
using System;
using System.Data;
using System.Data.SqlClient;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ControlPicking.Views
{
    public partial class Login : ContentPage
    {
        Models.ValidacionPick Validaciones = new Models.ValidacionPick();
        string equipo;

        public Login()
        {

            InitializeComponent();
            Password.Focus();
            equipo = DeviceInfo.Name;

        }


        public void llenar()
        {
            var deviceType = DeviceInfo.DeviceType;
            object devicename = null;
            object version = null;
            object platform = null;
            lblPruebaInfo.Text = $"{devicename}{version}{platform}";           // Read the files
            lblPruebaDevice.Text = DeviceDisplay.MainDisplayInfo.ToString();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Password.Text = "";
            Password.Focus();
            DeviceDisplay.MainDisplayInfoChanged += DeviceDisplay_MainDisplayInfoChanged;
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            DeviceDisplay.MainDisplayInfoChanged -= DeviceDisplay_MainDisplayInfoChanged;
        }

        private void DeviceDisplay_MainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            lblPruebaDevice.Text = e.DisplayInfo.ToString();
        }
        protected override bool OnBackButtonPressed()
        {

            Device.BeginInvokeOnMainThread(async () =>
            {
                var result = await this.DisplayAlert("Alerta", "Deseas salir de la aplicación?", "Si", "No").ConfigureAwait(false);
                if (result) System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            });
            Usuario.Text = "";
            Password.Text = "";
            Usuario.Focus();
            return true;

        }
        async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            string result;
            string mensaje;




            if (Password.Text.Length >= 5)
            {
                result = Validaciones.ValidacionLogin(Usuario.Text, Password.Text);

                if (result != "")
                {
                    DisplayAlert("Error", result, "OK");
                }
                else
                {
                    try
                    {
                        ConexionSql.OpenC();

                        SqlCommand SqlQuery = new SqlCommand("ValidarUsuarioSel", ConexionSql.Conectar);
                        SqlQuery.CommandType = CommandType.StoredProcedure;
                        SqlQuery.Parameters.AddWithValue("@Usuario", Usuario.Text);
                        SqlQuery.Parameters.AddWithValue("@password", Password.Text);

                        SqlDataReader SqlRead = SqlQuery.ExecuteReader();

                        if (SqlRead.Read())
                        {
                            SqlRead.Close();
                            ConexionSql.CloseC();
                            await Navigation.PushAsync(new OrderPick("Nave7", equipo, int.Parse(Usuario.Text)));


                        }
                        else
                        {
                            await DisplayAlert("Error", "No existe el usuario o la contraseña es incorrecta", "OK");
                            SqlRead.Close();
                            ConexionSql.CloseC();
                        }

                    }
                    catch (Exception ex)
                    {

                        if (ex.Message.Contains("error: 40"))
                        {
                            mensaje = "No estas conectado a la red";
                        }
                        else
                        {
                            mensaje = ex.Message;
                        }
                        ConexionSql.CloseC();
                        await DisplayAlert("Error", mensaje, "OK");


                    }
                }
            }
        }


    }
}