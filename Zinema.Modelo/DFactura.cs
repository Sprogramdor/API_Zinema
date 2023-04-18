namespace Zinema.Modelo {
    public class DFactura {
        public int DFactura_id { get; set; }
        public Factura? factura { get; set; }
        public Ticket? ticket { get; set; }
        public string? Descripcion { get; set; }
        public float total { get; set; }
        public DateTime Fecha_compra { get; set; }
        public Snacks? snacks { get; set; }

    }
}
