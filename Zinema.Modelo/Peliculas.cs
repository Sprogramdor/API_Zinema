namespace Zinema.Modelo {
    public class Peliculas {
        public int Pelicula_id { get; set; }
        public string? Poster { get; set; }
        public string? Titulo_original { get; set; }
        public string? Reparto { get; set; }
        public string? Sinopsis { get; set; }
        public int Tiempo_duracion { get; set; }
        public string? Tipo { get; set; }
        public string? Categoria { get; set; }
        public string? Clasificacion { get; set; }
        public string? Estado { get; set; }
        public float Precio { get; set; }

        public string Transaccion { get; set; }
    }
}
