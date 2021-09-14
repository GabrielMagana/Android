using System;
using System.Data;
using System.Data.SqlClient;

namespace ControlPicking.Models
{
    public class ValidacionPick
    {



        public void EncabezadoFolio(int _Linea, DateTime date, string time, string _contenedor, string _Caja, string _Operador, int _Empleado)
        {
            try
            {
                Services.ConexionSql.OpenC();

                SqlCommand SqlQuery = new SqlCommand("RelFolioEncabezadoIU", Services.ConexionSql.Conectar);
                SqlQuery.CommandType = CommandType.StoredProcedure;
                SqlQuery.Parameters.AddWithValue("@Fecha", date);
                SqlQuery.Parameters.AddWithValue("@Hora", time);
                SqlQuery.Parameters.AddWithValue("@ClaLinea", _Linea);
                SqlQuery.Parameters.AddWithValue("@ClaveContenedor", _contenedor);
                SqlQuery.Parameters.AddWithValue("@ClaveCaja", _Caja);
                SqlQuery.Parameters.AddWithValue("@Operador", _Operador);
                SqlQuery.Parameters.AddWithValue("@ClaEmpleado", _Empleado);

                SqlDataReader SqlRead1 = SqlQuery.ExecuteReader();

                SqlRead1.Close();
                Services.ConexionSql.CloseC();

            }
            catch (Exception)
            {

                Services.ConexionSql.CloseC();

            }


        }


        public string SelloFolio(int _Linea, DateTime date, string time, string _contenedor)
        {
            string mensaje = "";
            try
            {
                Services.ConexionSql.OpenC();

                SqlCommand SqlQuery = new SqlCommand("RelSelloFolioIU", Services.ConexionSql.Conectar);
                SqlQuery.CommandType = CommandType.StoredProcedure;
                SqlQuery.Parameters.AddWithValue("@Fecha", date);
                SqlQuery.Parameters.AddWithValue("@Hora", time);
                SqlQuery.Parameters.AddWithValue("@ClaLinea", _Linea);
                SqlQuery.Parameters.AddWithValue("@ClaveContenedor", _contenedor);

                SqlDataReader SqlRead1 = SqlQuery.ExecuteReader();

                SqlRead1.Close();
                Services.ConexionSql.CloseC();

            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                Services.ConexionSql.CloseC();

            }

            return mensaje;


        }

        public string Sello(int _Linea, DateTime date, string time, string _contenedor)
        {
            string mensaje = "";
            try
            {
                Services.ConexionSql.OpenC();

                SqlCommand SqlQuery = new SqlCommand("RelSelloIU", Services.ConexionSql.Conectar);
                SqlQuery.CommandType = CommandType.StoredProcedure;
                SqlQuery.Parameters.AddWithValue("@Fecha", date);
                SqlQuery.Parameters.AddWithValue("@Hora", time);
                SqlQuery.Parameters.AddWithValue("@ClaLinea", _Linea);
                SqlQuery.Parameters.AddWithValue("@ClaveContenedor", _contenedor);

                SqlDataReader SqlRead1 = SqlQuery.ExecuteReader();

                SqlRead1.Close();
                Services.ConexionSql.CloseC();

            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                Services.ConexionSql.CloseC();

            }

            return mensaje;


        }

    }
}
