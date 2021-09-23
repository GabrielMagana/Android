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
    public partial class OrdenDetalle : ContentPage
    {
        Models.ValidacionPick Validaciones = new Models.ValidacionPick();
        List<Models.Detalle> DetalleList;
        string itemParte;
        int Cantidadleida, CantidadTotal, OrdenNo, Line;
        string numeroparte, Orden;
        Int32 PickUnico;

        public OrdenDetalle(String _orden, Int32 _idPick)
        {
            InitializeComponent();
            ItemDetalle(_orden, _idPick);
            Orden = _orden;
            txtNparte.Focus();
        }
     

        private async void txtNparte_Completed(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtNparte.Text) == true)
            {
                await DisplayAlert("Error", "Debes escanear un número de parte", "OK");
                txtNparte.Text = "";
                txtNparte.Focus();
                return;
            }

           
            if (!numeroparte.ToString().Contains(" ") && numeroparte.Length > 11)
            {
                numeroparte = numeroparte.Substring(0, 12);
            }
            if (numeroparte.ToString().Contains(" ") && numeroparte.Length <= 12)
            {
                
                
                if(numeroparte.Substring(10, 1).Equals(" "))
                    {
                    numeroparte = numeroparte.Substring(0, 9).Trim();
                }
                else
                {
                    numeroparte = numeroparte.Substring(0, 10).Trim();
                }

            }


            itemParte = Itemtext(txtNparte.Text);
            
            if (numeroparte.ToUpper() != itemParte.ToUpper())
            {
                await DisplayAlert("Error", "El número de parte no es el mismo", "OK");
                txtNparte.Text = "";
                txtNparte.Focus();
                return;
            }

            if (Cantidadleida >= CantidadTotal)
            {
                await DisplayAlert("Error", "La orden ya esta completa", "OK");
                txtNparte.Text = "";
                txtNparte.Focus();
                return;
            }

            ActualizarItem(PickUnico);
            txtNparte.Text = "";
            txtNparte.Focus();
            

        }

        public string Itemtext(String _Item)
        {
            string itemtxt = null;
            char fragment = (char)28;

            if (char.IsLetterOrDigit(_Item, 1) == false || char.IsSeparator(_Item, 1) == true || _Item.ToString().Contains(fragment) == true)
            {
                itemtxt = _Item.Substring(1, _Item.Length-1);
            }
            else
            {
                itemtxt = _Item;
            }



            itemtxt= itemtxt.Replace(" ", "$");

            if (!itemtxt.ToString().Contains("$") && itemtxt.Length>11)
            {
                itemtxt = itemtxt.Substring(0, 12);
            }
            if (itemtxt.ToString().Contains("$") && itemtxt.Length > 11)
            {

                if (itemtxt.Substring(9, 1).Equals("$"))
                {
                    itemtxt = itemtxt.Substring(0, 9).Trim();
                }
                else
                {
                    itemtxt = itemtxt.Substring(0, 10).Trim();
                }

            }


            return itemtxt;
        }


        public async void ItemDetalle(string _orden,Int32 Pick)
        {
            DetalleList = new List<Models.Detalle>();
            string Mensajes = "";
            lvOrdenes.ItemsSource = null;

            try
            {
                Services.ConexionSql.OpenC();

                SqlCommand SqlQuery = new SqlCommand("ConsultarLPNSel", Services.ConexionSql.Conectar);
                SqlQuery.CommandType = CommandType.StoredProcedure;
                SqlQuery.Parameters.AddWithValue("@LPN", _orden);
                SqlQuery.Parameters.AddWithValue("@idPick", Pick);



                SqlDataReader SqlRead1 = SqlQuery.ExecuteReader();

                while (SqlRead1.Read())
                {
                    txtleidos.Text = SqlRead1["Cantidad"].ToString();
                    txtQtyTotal.Text = SqlRead1["QtyTotal"].ToString();
                    Cantidadleida = int.Parse(SqlRead1["Cantidad"].ToString());
                    CantidadTotal = int.Parse(SqlRead1["QtyTotal"].ToString());
                    numeroparte = SqlRead1["Item"].ToString().Trim();
                    OrdenNo= int.Parse(SqlRead1["Order_no"].ToString());
                    Line = int.Parse(SqlRead1["SO_Line"].ToString());
                    PickUnico = Int32.Parse(SqlRead1["IdPick"].ToString());

                    DetalleList.Add(new Models.Detalle { Pick_Lnp = SqlRead1["Pick_lpn"].ToString(), Orden = int.Parse(SqlRead1["Order_no"].ToString()), Line = int.Parse(SqlRead1["SO_Line"].ToString()), Item = SqlRead1["Item"].ToString(), Qty = int.Parse(SqlRead1["QtyTotal"].ToString()), IdPick = Int32.Parse(SqlRead1["IdPick"].ToString()) });
                }

                lvOrdenes.ItemsSource = DetalleList;
                SqlRead1.Close();
                Services.ConexionSql.CloseC();

            }
            catch (Exception ex)

            {
                Mensajes = ex.Message;
                await DisplayAlert("Error", Mensajes, "OK");
                Services.ConexionSql.CloseC();
            }
        }


        private void ActualizarItem(Int32 PickUnico)
        {
            string Mensajes;
            try
            {
                Services.ConexionSql.OpenC();

                SqlCommand SqlQuery = new SqlCommand("ActualizarIU", Services.ConexionSql.Conectar);
                SqlQuery.CommandType = CommandType.StoredProcedure;
                SqlQuery.Parameters.AddWithValue("@IdPick", PickUnico);
               
                SqlQuery.ExecuteNonQuery();
                Services.ConexionSql.CloseC();

                Cantidadleida = Cantidadleida + 1;
                txtleidos.Text = Cantidadleida.ToString();
            }
            catch (Exception ex)

            {
                Mensajes = ex.Message;
                Services.ConexionSql.CloseC();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            txtNparte.Focus();
        }

    }
}