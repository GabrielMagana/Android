using System.Data;
using System.Data.SqlClient;

namespace ControlPicking.Services
{
    public class ConexionSql
    {
        //public static string Conections = "Data Source=192.168.10.243;Initial Catalog=EtiquetasPDC;Persist Security Info=True;User ID=sa;Password=cabejeCat3";
          public static string Conections = "Data Source=192.168.1.221;Initial Catalog=EtiquetasPDC;Persist Security Info=True;User ID=CP_user;Password=Mazda2020";
        public static SqlConnection Conectar = new SqlConnection(Conections);

        public static void OpenC()
        {

            if (Conectar.State == ConnectionState.Closed)
            {
                Conectar.Open();
            }
        }
        public static void CloseC()
        {
            if (Conectar.State == ConnectionState.Open)
            {
                Conectar.Close();
            }
        }



    }



}
