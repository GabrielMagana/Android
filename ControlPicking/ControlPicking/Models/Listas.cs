namespace ControlPicking.Models
{
    public class Listas
    {
        public string Pick_Lnp { get; set; }
        public string Item { get; set; }
        public string Estatus { get; set; }
        public int Orden { get; set; }
        public int Line { get; set; }

        public long IdPick { get; set; }

    }
    public class Detalle
    {
        public long IdPick { get; set; }
        public string Pick_Lnp { get; set; }
        public string Item { get; set; }
        public int Qty { get; set; }
        public int Orden { get; set; }
        public int Line { get; set; }


    }

}


