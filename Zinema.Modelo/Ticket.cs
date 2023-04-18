namespace Zinema.Modelo {
    public class Ticket {
        public int Ticket_id { get; set; }
        public DateTime Fecha_compra { get; set; }
        public Usuario? usuario { get; set; }
        public Asientos? asiento { get; set; }
        public Funciones? funcion { get; set; }
    }
}
