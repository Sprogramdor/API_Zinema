using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Xml.Linq;
using Zinema.API.Datos;
using Zinema.Modelo;


namespace Zinema.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PeliculasController : ControllerBase {
        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<Peliculas>> GetPeliculas(string transaccion) {
            Peliculas peliculasTmp = new Peliculas();
            peliculasTmp.Transaccion = transaccion;
            var cadenaConexion = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Conexion"];
            XDocument xmlParam = XmlMethods.GetXml(peliculasTmp);
            DataSet dsResultado = await XmlMethods.EjecutaBase(NameStoreProcedure.SPGetPeliculas, cadenaConexion, peliculasTmp.Transaccion, xmlParam.ToString());
            List<Peliculas> listaData = new List<Peliculas>();
            if (dsResultado.Tables.Count > 0) {
                try {
                    foreach (DataRow row in dsResultado.Tables[0].Rows) {
                        Peliculas objResponse = new Peliculas {
                            Pelicula_id = Convert.ToInt32(row["Pelicula_id"]),
                            Poster = row["Poster"].ToString(),
                            Titulo_original = row["Titulo_original"].ToString(),
                            Reparto = row["Reparto"].ToString(),
                            Sinopsis = row["Sinopsis"].ToString(),
                            Tiempo_duracion = Convert.ToInt32(row["Tiempo_duracion"]),
                            Tipo = row["Tipo"].ToString(),
                            Categoria = row["Categoria"].ToString(),
                            Clasificacion = row["Clasificacion"].ToString(),
                            Estado = row["Estado"].ToString(),
                            Precio = float.Parse(row["Precio"].ToString()),
                            Transaccion = peliculasTmp.Transaccion
                        };
                        listaData.Add(objResponse);
                    }
                } catch (Exception ex) {
                    Console.Write("error");
                    Console.WriteLine(ex.Message);
                }
            }
            return Ok(listaData);
        }

        [Route("[action]")]
        [HttpPost]
        [Authorize]

        public async Task<ActionResult> SetPeliculas([FromBody] Peliculas peli) {
            var cadenaConexion = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Conexion"];
            XDocument xmlParam = XmlMethods.GetXml(peli);
            DataSet dsResultado = await XmlMethods.EjecutaBase(NameStoreProcedure.SPSetPeliculas, cadenaConexion, peli.Transaccion, xmlParam.ToString());

            RespuestaSP objresponse = new RespuestaSP();
            if (dsResultado.Tables.Count > 0) {
                try {
                    objresponse.Respuesta = dsResultado.Tables[0].Rows[0]["Respuesta"].ToString();
                    objresponse.Leyenda = dsResultado.Tables[0].Rows[0]["Leyenda"].ToString();
                } catch (Exception ex) {
                    objresponse.Respuesta = "Error";
                    objresponse.Leyenda = "No se pudo completar la transacción";
                }
            }
            return Ok(objresponse);
        }


    }
}
