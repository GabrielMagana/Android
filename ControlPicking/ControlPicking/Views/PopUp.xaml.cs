using Rg.Plugins.Popup.Services;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ControlPicking.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopUp
    {
        int tipoAlert, usuario;
        string title, contend, LeidoNP, OrdenNP, pickLpn, password;

        public PopUp(string Titulo, string Mensaje, string numeroParteLeido, string numeroParteOrden, int tipo, string PickLpn, int Usuario)
        {
            InitializeComponent();
            tipoAlert = tipo;
            title = Titulo;
            contend = Mensaje;
            LeidoNP = numeroParteLeido;
            OrdenNP = numeroParteOrden;
            pickLpn = PickLpn;
            usuario = Usuario;
            Iconoerror.IsVisible = false;
            IconoSuccess.IsVisible = false;
            Iconowarning.IsVisible = false;
            btnno.IsVisible = false;
            TipoAlert();
             CloseWhenBackgroundIsClicked = false;

        }

        public void Load()
        {
               if (tipoAlert != 2)
                {
                    Contrasena.Focus();
                    Contrasena.Text = "";
                }
        }

        private async void btnResp_Clicked(object sender, EventArgs e)
        {
            int Confirmacion;
            string pas;
            char fragment = (char)28;
            int tamano;

            if (tipoAlert != 2)
            {
              
                if (string.IsNullOrEmpty(Contrasena.Text))
                {
                    DisplayAlert("Warning", "Debe capturar la contraseña del foreman.", "OK");
                }
                else
                {
                    pas = Contrasena.Text;
                    if (pas.Length - 1 > 15)
                    {
                        tamano = pas.Length - 1;





                        if (char.IsLetterOrDigit(pas, 1) == false || char.IsSeparator(pas, 1) == true || pas.Contains(fragment) == true)
                        {
                            password = pas.Substring(1, tamano);

                        }
                        else
                        {
                            password = pas;
                        }


                        Confirmacion = Confirmar();
                        if (Confirmacion == null || Confirmacion == 0)
                        {
                            DisplayAlert("Warning", "No tienes permisos para proceder", "OK");
                            Contrasena.Focus();
                            Contrasena.Text = "";

                        }
                        else
                        {

                            RegistarLog();
                            if (tipoAlert == 3 || tipoAlert == 4)
                            {
                                var _navigation = Application.Current.MainPage.Navigation;
                                var _lastPage = this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 1];
                                //// Remove last page
                                 _navigation.RemovePage(_lastPage);
                                //await _navigation.PopAsync();
                                await PopupNavigation.PopAsync();
                            }
                            else
                            {
                                //Navigation.PopAsync();
                                Navigation.PopAsync();
                            }

                        }

                    }
                    else
                    {
                        Contrasena.Focus();
                        Contrasena.Text = "";
                    }
                }


            }

            if (tipoAlert == 2)
            {


                var _navigation = Application.Current.MainPage.Navigation;
                var _lastPage = this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 1];
                //Remove last page
                _navigation.RemovePage(_lastPage);
                //await _navigation.PopAsync();
                await PopupNavigation.PopAsync();

            }

        }

        private async void btnno_Clicked(object sender, EventArgs e)
        {
            await   PopupNavigation.PopAsync();
            //await Navigation.PopAsync();
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
      

        public void TipoAlert()
        {

            if (tipoAlert == 1)
            {
                FondoTitulo.BackgroundColor = Color.Transparent;
                FondoTitulo.BackgroundColor = Color.Red;
                Title.Text = title;
                Contenido.Text = contend;
                Contenido.FontSize = 12;
                Leido.Text = "Numero de parte Leido: " + LeidoNP;
                Leido.FontSize = 10;
                Leido.FontAttributes = FontAttributes.Bold;
                PickLnp.Text = "Numero de parte Orden: " + OrdenNP;
                PickLnp.FontSize = 10;
                PickLnp.FontAttributes = FontAttributes.Bold;
                Contrasena.IsVisible = true;
                Iconoerror.IsVisible = true;
                btnResp.Text = "Aceptar";
                btnResp.WidthRequest = 250;
                btnResp.BackgroundColor = Color.Default;
                

            }
            if (tipoAlert == 2)
            {

                FondoTitulo.BackgroundColor = Color.Transparent;
                FondoTitulo.BackgroundColor = Color.Green;
                Title.Text = title;
                Contenido.Text = contend;
                Contenido.FontSize = 15;
                Leido.IsVisible = false;
                PickLnp.IsVisible = false;
                Contrasena.IsVisible = false;
                IconoSuccess.IsVisible = true;
                btnResp.Text = "OK";
                btnResp.BackgroundColor = Color.Default;
                btnResp.WidthRequest = 250;



            }
            if (tipoAlert == 3)
            {

                FondoTitulo.BackgroundColor = Color.Transparent;
                FondoTitulo.BackgroundColor = Color.Yellow;
                Title.Text = title;
                Contenido.Text = contend;
                Contenido.FontSize = 15;
                Leido.Text = "Numero de parte Leido: " + LeidoNP;
                Leido.FontSize = 10;
                Leido.FontAttributes = FontAttributes.Bold;
                PickLnp.IsVisible = false;
                Contrasena.IsVisible = true;
                Iconowarning.IsVisible = true;
                btnResp.Text = "Aceptar";
                btnResp.BackgroundColor = Color.Default;
               


            }
            if (tipoAlert == 4)
            {

                FondoTitulo.BackgroundColor = Color.Transparent;
                FondoTitulo.BackgroundColor = Color.Yellow;
                Title.Text = title;
                Contenido.Text = contend;
                Contenido.FontSize = 15;
                Leido.IsVisible = false;
                PickLnp.IsVisible = false;
                Contrasena.IsVisible = true;
                Iconowarning.IsVisible = true;
                btnResp.Text = "Ok";
                btnResp.WidthRequest = 110;
                btnno.Text = "Cancel";
                btnno.IsVisible = true;
                btnno.WidthRequest = 110;
                btnResp.BackgroundColor = Color.Default;
                btnno.BackgroundColor = Color.Default;
                


            }
        }
        public void RegistarLog()
        {

            try
            {
                Services.ConexionSql.OpenC();

                SqlCommand SqlQuery = new SqlCommand("CrearLog", Services.ConexionSql.Conectar);
                SqlQuery.CommandType = CommandType.StoredProcedure;
                SqlQuery.Parameters.AddWithValue("@contrasena", password);
                SqlQuery.Parameters.AddWithValue("@usuario", usuario);
                SqlQuery.Parameters.AddWithValue("@PickLpn", pickLpn);
                SqlQuery.Parameters.AddWithValue("@item", OrdenNP);
                SqlQuery.Parameters.AddWithValue("@TipoError", tipoAlert);


                SqlDataReader SqlRead1 = SqlQuery.ExecuteReader();

                SqlRead1.Close();
                Services.ConexionSql.CloseC();
            }
            catch (Exception ex)

            {

            }
        }

        public int Confirmar()
        {
            int Validacion = 0;
            try
            {
                Services.ConexionSql.OpenC();

                SqlCommand SqlQuery = new SqlCommand("ConsultarPermisosSel", Services.ConexionSql.Conectar);
                SqlQuery.CommandType = CommandType.StoredProcedure;
                SqlQuery.Parameters.AddWithValue("@contrasena", password);

                SqlDataReader SqlRead1 = SqlQuery.ExecuteReader();

                if (SqlRead1.HasRows == true)
                {
                    Validacion = 1;

                }
                else
                {
                    Validacion = 0;
                }
                SqlRead1.Close();
                Services.ConexionSql.CloseC();


            }
            catch (Exception ex)

            {
                Validacion = 0;
                return Validacion;
            }
            return Validacion;

        }
    }
}
