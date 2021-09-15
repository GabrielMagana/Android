using Acr.UserDialogs;
using ControlPicking.Models;
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
        String ordentext = "";
        public OrderPick()
        {
            InitializeComponent();
            txtOrden.Focus();
        }
      

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var result = await this.DisplayAlert("Alerta", "Deseas salir de la aplicación?", "Si", "No").ConfigureAwait(false);
                if (result) System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            });
            return true;
        }


        private async void txtOrden_Completed(object sender, EventArgs e)
        {
            char fragment = (char)28;
            String Ordenleida, ordentxt;
            int tamano;

            if (string.IsNullOrWhiteSpace(txtOrden.Text) == true)
            {
                await DisplayAlert("Error", "Debes escanear una orden", "OK");
                txtOrden.Focus();
                txtOrden.Text = "";
                return;
            }


            Ordenleida = txtOrden.Text;
            tamano = Ordenleida.Length-1;

            if (char.IsLetterOrDigit(Ordenleida, 1) == false || char.IsSeparator(Ordenleida, 1) == true ||  Ordenleida.ToString().Contains(fragment) == true)
            {
                ordentxt = Ordenleida.Substring(1, tamano);
               
            }
            else
            {
                ordentxt = Ordenleida;
            }

            ordentext = ordentxt;

            Orden(ordentxt);
            txtOrden.Focus();
            txtOrden.Text = "";

        
        }


        public async void Orden(string _Orden)
        {
            OrdenList = new List<Models.Listas>();
            string Mensajes = "";
            lvOrdenes.ItemsSource = null;
            bool Noexiste =false;

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
                    goto Salir;
                }
                

                while (SqlRead1.Read())
                {
                    OrdenList.Add(new Models.Listas { Pick_Lnp = SqlRead1["Pick_lpn"].ToString(), Orden = int.Parse(SqlRead1["Order_no"].ToString()), Line = int.Parse(SqlRead1["SO_Line"].ToString()), Item = SqlRead1["Item"].ToString(), Fecha = SqlRead1["Schedule_ship_date"].ToString(), Estatus = SqlRead1["Estatus"].ToString() });
                }

                lvOrdenes.ItemsSource = OrdenList;


                SqlRead1.Close();


            Salir:

                if (Noexiste == true)
                {
                    await DisplayAlert("Alerta","No existe esa orden", "Ok");
                }
                Services.ConexionSql.CloseC();

                

            }
            catch (Exception ex)

            {
                Mensajes = ex.Message;
                Services.ConexionSql.CloseC();
            }
        }




        private async void lvOrdenes_ItemTapped(object sender, ItemTappedEventArgs e)
        {

            lvOrdenes.ItemsSource = null;
            await Navigation.PushAsync(new OrdenDetalle(ordentext));
        }

        private void txtOrden_Completed_1(object sender, EventArgs e)
        {

        }
    }
}