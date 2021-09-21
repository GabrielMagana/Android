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


        public OrdenDetalle(String _orden)
        {
            InitializeComponent();
            ItemDetalle(_orden);
            Orden = _orden;
            txtNparte.Focus();
            txtNparte.Text = "";
        }
     

        private async void txtNparte_Completed(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtNparte.Text) == true)
            {
                await DisplayAlert("Error", "Debes escanear un número de parte", "OK");
                txtNparte.Focus();
                txtNparte.Text = "";
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
                txtNparte.Focus();
                txtNparte.Text = "";
                return;
            }

            if (Cantidadleida >= CantidadTotal)
            {
                await DisplayAlert("Error", "La orden ya esta completa", "OK");
                txtNparte.Focus();
                txtNparte.Text = "";
                return;
            }

            ActualizarItem(itemParte.ToUpper(), Orden,OrdenNo,Line);

            txtNparte.Focus();
            txtNparte.Text = "";

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
            if (itemtxt.ToString().Contains("$") && itemtxt.Length <= 12)
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


        public async void ItemDetalle(string _orden)
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
                    DetalleList.Add(new Models.Detalle { Pick_Lnp = SqlRead1["Pick_lpn"].ToString(), Orden = int.Parse(SqlRead1["Order_no"].ToString()), Line = int.Parse(SqlRead1["SO_Line"].ToString()), Item = SqlRead1["Item"].ToString(), Item_description = SqlRead1["Item_Description"].ToString(), Qty = int.Parse(SqlRead1["QtyTotal"].ToString()) });
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


        private void ActualizarItem(string NParte, string Orden,int OrdenNo, int Line)
        {
            string Mensajes;
            try
            {
                Services.ConexionSql.OpenC();

                SqlCommand SqlQuery = new SqlCommand("ActualizarIU", Services.ConexionSql.Conectar);
                SqlQuery.CommandType = CommandType.StoredProcedure;
                SqlQuery.Parameters.AddWithValue("@item", NParte);
                SqlQuery.Parameters.AddWithValue("@Orden", OrdenNo);
                SqlQuery.Parameters.AddWithValue("@Line", Line);
                SqlQuery.Parameters.AddWithValue("@LnpPadre", Orden);

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
    }
}