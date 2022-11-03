using ControlPicking.Services;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
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
        int Cantidadleida, CantidadTotal, OrdenNo, Line, Usuario;
        string numeroparte, Orden, equipo, _case1, picklpn;
        long PickUnico;

        public OrdenDetalle(string _orden, long _idPick, string _case, int _usuario)
        {
            InitializeComponent();
            ItemDetalle(_orden, _idPick);
            equipo = DeviceInfo.Name;
            Usuario = _usuario;
            _case1 = _case;
            Orden = _orden;
            PickUnico = _idPick;
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


            if (!numeroparte.ToString().Contains(" ") && numeroparte.Length >= 12)
            {
                numeroparte = numeroparte.Substring(0, 12);
            }
            if (numeroparte.ToString().Contains(" ") && numeroparte.Length < 12)
            {


                if (numeroparte.Substring(10, 1).Equals(" "))
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

                var pr = new PopUp("Error", "Numero de parte diferente", itemParte.ToUpper(), numeroparte.ToUpper(), 1, picklpn, Usuario);
                AudioService.Sound("ControlPicking.Sonidos.Error.mp3");
                await Navigation. PushAsync(pr);


                //await DisplayAlert("Error", "El número de parte no es el mismo", "OK");
            
                return;
            }

            if (Cantidadleida >= CantidadTotal)
            {
                var pr = new PopUp("Warning", "La orden ya esta completa", itemParte.ToUpper(), "", 3, picklpn, Usuario);
                AudioService.Sound("ControlPicking.Sonidos.Alert.mp3");
                await PopupNavigation.PushAsync(pr);

                //await DisplayAlert("Error", "La orden ya esta completa", "OK");
            
                return;
            }

            ActualizarItemAsync(PickUnico, _case1);
           

        }

        public string Itemtext(String _Item)
        {
            //string itemtxt = null;
            //char fragment = (char)28;

            //if (char.IsLetterOrDigit(_Item, 1) == false || char.IsSeparator(_Item, 1) == true || _Item.ToString().Contains(fragment) == true)
            //{
            //    itemtxt = _Item.Substring(1, _Item.Length - 1);
            //}
            //else
            //{
            //    itemtxt = _Item;
            //}

            ////leer 12 caractaeres
            //// Leer etiquetas con caracteres '-_
            ////Pantallas de capturas

            //itemtxt = itemtxt.Replace(" ", "$");

            //if (!itemtxt.ToString().Contains("$") && itemtxt.Length > 11)
            //{
            //    itemtxt = itemtxt.Substring(0, 12);
            //}
            //if (itemtxt.ToString().Contains("$") && itemtxt.Length > 11)
            //{

            //    if (itemtxt.Substring(9, 1).Equals("$"))
            //    {
            //        itemtxt = itemtxt.Substring(0, 9).Trim();
            //    }
            //    else
            //    {
            //        itemtxt = itemtxt.Substring(0, 10).Trim();
            //    }

            //}

            string str = null;
            string str2;
            char ch = (char)28;

            if ((_Item.Contains("'") || _Item.Contains("_")) || _Item.Contains("-"))
            {
                if (_Item.Contains("'"))
                {
                    _Item = _Item.Replace("'", "");
                }
                if (_Item.Contains("-"))
                {
                    _Item = _Item.Replace("-", "");
                }
                if (_Item.Contains("_"))
                {
                    _Item = _Item.Replace("_", "");
                }
            }
            if (_Item.Length <= 13)
            {
                _Item = _Item + "        ";
                _Item = _Item.Substring(0, 13);
            }

            if (_Item.Length < 12)
            {
                str2 = str = "";
            }
            else
            {

                str = !((!char.IsLetterOrDigit(_Item, 1) || char.IsSeparator(_Item, 1)) || _Item.ToString().Contains<char>(ch)) ? _Item : _Item.Substring(1, _Item.Length - 1);
                str2 = str.Substring(0, 12).Trim();
            }
            return str2;


        }


        public async void ItemDetalle(string _orden, long Pick)
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
                    OrdenNo = int.Parse(SqlRead1["Order_no"].ToString());
                    Line = int.Parse(SqlRead1["SO_Line"].ToString());
                    PickUnico = long.Parse(SqlRead1["IdPick"].ToString());
                    picklpn = SqlRead1["Pick_lpn"].ToString();
                    DetalleList.Add(new Models.Detalle { Pick_Lnp = SqlRead1["Pick_lpn"].ToString(), Orden = int.Parse(SqlRead1["Order_no"].ToString()), Line = int.Parse(SqlRead1["SO_Line"].ToString()), Item = SqlRead1["Item"].ToString(), Qty = int.Parse(SqlRead1["QtyTotal"].ToString()), IdPick = long.Parse(SqlRead1["IdPick"].ToString()) });
                }

                lvOrdenes.ItemsSource = DetalleList;
                SqlRead1.Close();
                Services.ConexionSql.CloseC();

                if (Cantidadleida == CantidadTotal)
                {

                    var pr = new PopUp("Successfull", "La orden ya esta completa", numeroparte, "", 2, picklpn, Usuario);
                    AudioService.Sound("ControlPicking.Sonidos.Successfull.mp3");
                    await PopupNavigation.PushAsync(pr);
                    //await DisplayAlert("Error", "La orden ya esta completa", "OK");
                }

            }
            catch (Exception ex)

            {
                Mensajes = ex.Message;
                await DisplayAlert("Error", Mensajes, "OK");
                Services.ConexionSql.CloseC();
            }

        }


        private async Task ActualizarItemAsync(long PickUnico, string finalcase)
        {
            string Mensajes;
            int cantidad = 0;

            try
            {
                Services.ConexionSql.OpenC();

                SqlCommand SqlQuery = new SqlCommand("ActualizarIU", Services.ConexionSql.Conectar);
                SqlQuery.CommandType = CommandType.StoredProcedure;
                SqlQuery.Parameters.AddWithValue("@IdPick", PickUnico);
                SqlQuery.Parameters.AddWithValue("@Case", finalcase);
                SqlQuery.Parameters.AddWithValue("@Equipo", Usuario);

                SqlQuery.ExecuteNonQuery();
                Services.ConexionSql.CloseC();


                Services.ConexionSql.OpenC();

                SqlCommand sqlQuery1 = new SqlCommand("BuscarNP", Services.ConexionSql.Conectar);
                sqlQuery1.CommandType = CommandType.StoredProcedure;
                sqlQuery1.Parameters.AddWithValue("@IdPick", PickUnico);

                SqlDataReader SqlRead1 = sqlQuery1.ExecuteReader();
                while (SqlRead1.Read())
                {
                    cantidad = int.Parse(SqlRead1["Cantidad"].ToString());
                }
                SqlRead1.Close();
                Services.ConexionSql.CloseC();



                Cantidadleida = Cantidadleida + cantidad;
                txtleidos.Text = Cantidadleida.ToString();


                if (Cantidadleida == CantidadTotal)
                {
                    var pr = new PopUp("Successfull", "La orden ya esta completa", numeroparte, "", 2, picklpn, Usuario);
                    await PopupNavigation.PushAsync(pr);
                    //await DisplayAlert("Error", "La orden ya esta completa", "OK");
                    return;
                }

                txtNparte.Text = "";
                txtNparte.Focus();

            }
            catch (Exception ex)

            {
                Mensajes = ex.Message;
                Services.ConexionSql.CloseC();
            }
        }

        protected override void OnAppearing()
        {
           
            txtNparte.Text = "";
            txtNparte.Focus();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
}