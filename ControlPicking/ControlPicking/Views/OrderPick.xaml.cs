//using Android.App;
using ControlPicking.Models;
using ControlPicking.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ControlPicking.Views
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderPick : ContentPage
    {
        Models.ValidacionPick Validaciones = new Models.ValidacionPick();
        List<Models.Listas> OrdenList;
        string ordentext = "", Case, equipo;
        long idpick;
        int _usuario;

        public OrderPick(string _case, string _equipo, int usuario)
        {
            InitializeComponent();
            Case = _case;
            _usuario = usuario;
            txtOrden.Text = "";
            txtOrden.Focus();

        }



        private async void txtOrden_Completed(object sender, EventArgs e)
        {
            char fragment = (char)28;
            String Ordenleida, ordentxt;
            int tamano;

            if (string.IsNullOrWhiteSpace(txtOrden.Text) == true)
            {
                await DisplayAlert("Error", "Debes escanear una orden", "OK");

                txtOrden.Text = "";
                txtOrden.Focus();
                return;
            }


            Ordenleida = txtOrden.Text;
            tamano = Ordenleida.Length - 1;

            if (char.IsLetterOrDigit(Ordenleida, 1) == false || char.IsSeparator(Ordenleida, 1) == true || Ordenleida.ToString().Contains(fragment) == true)
            {
                ordentxt = Ordenleida.Substring(1, tamano);

            }
            else
            {
                ordentxt = Ordenleida;
            }

            ordentext = ordentxt;

            Orden(ordentxt);



            txtOrden.Text = "";
            txtOrden.Focus();


        }


        public async void Orden(string _Orden)
        {
            OrdenList = new List<Models.Listas>();
            string Mensajes = "";
            lvOrdenes.ItemsSource = null;
            bool Noexiste = false;

            try
            {
                Services.ConexionSql.OpenC();

                SqlCommand SqlQuery = new SqlCommand("ConsultarLPNSel", Services.ConexionSql.Conectar);
                SqlQuery.CommandType = CommandType.StoredProcedure;
                SqlQuery.Parameters.AddWithValue("@LPN", _Orden);


                SqlDataReader SqlRead1 = SqlQuery.ExecuteReader();

                if (SqlRead1.HasRows == false)
                {
                    Noexiste = true;
                    SqlRead1.Close();
                    goto Salir;

                }


                while (SqlRead1.Read())
                {

                    OrdenList.Add(new Models.Listas { Pick_Lnp = SqlRead1["Pick_lpn"].ToString(), Orden = int.Parse(SqlRead1["Order_no"].ToString()), Line = int.Parse(SqlRead1["SO_Line"].ToString()), Item = SqlRead1["Item"].ToString(), Estatus = SqlRead1["Estatus"].ToString(), IdPick = long.Parse(SqlRead1["IdPick"].ToString()) });
                    idpick = long.Parse(SqlRead1["IdPick"].ToString());
                    ordentext = SqlRead1["Pick_lpn"].ToString();
                }


                SqlRead1.Close();




            Salir:



                if (Noexiste == true)
                {

                    await DisplayAlert("Alerta", "No existe esa orden", "Ok");
                    Services.ConexionSql.CloseC();
                    return;

                }

                Services.ConexionSql.CloseC();
                await Navigation.PushAsync(new OrdenDetalle(ordentext.Trim(), idpick, Case, _usuario));

            }
            catch (Exception ex)

            {
                Mensajes = ex.Message;
                Services.ConexionSql.CloseC();
            }
        }

        private async void btnregresar_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        private async void LvOrdenes_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Listas content = (Listas)e.Item;
            idpick = (int)content.IdPick;
            ordentext = (string)content.Pick_Lnp.Trim();
            lvOrdenes.ItemsSource = null;
            txtOrden.Focus();
            //await Navigation.PushAsync(new OrdenDetalle(ordentext, idpick, Case));

        }

        private void pruebasonido_Clicked(object sender, EventArgs e)
        {
            AudioService.Sound("ControlPicking.Sonidos.Error.mp3");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            txtOrden.Focus();
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}