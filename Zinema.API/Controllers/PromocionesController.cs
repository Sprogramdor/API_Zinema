using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Xml.Linq;
using Zinema.API.Datos;
using Zinema.Modelo;


namespace Zinema.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PromocionesController : ControllerBase {

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<Promociones>> GetPromociones(string transaccion) {
            Promociones promocionesTmp = new Promociones();
            promocionesTmp.Transaccion = transaccion;
            var cadenaConexion = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Conexion"];
            XDocument xmlParam = XmlMethods.GetXml(promocionesTmp);
            DataSet dsResultado = await XmlMethods.EjecutaBase(NameStoreProcedure.SPGetPromociones, cadenaConexion, promocionesTmp.Transaccion, xmlParam.ToString());
            List<Promociones> listaData = new List<Promociones>();
            if (dsResultado.Tables.Count > 0) {
                try {
                    foreach (DataRow row in dsResultado.Tables[0].Rows) {
                        Promociones objResponse = new Promociones {
                            Promociones_id = Convert.ToInt32(row["Promociones_id"]),
                            Poster = row["Poster"].ToString(),
                            Descripcion = row["Descripcion"].ToString(),
                            Transaccion = promocionesTmp.Transaccion
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

        public async Task<ActionResult> SetPromociones([FromBody] Promociones promo) {
            var cadenaConexion = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Conexion"];
            XDocument xmlParam = XmlMethods.GetXml(promo);
            DataSet dsResultado = await XmlMethods.EjecutaBase(NameStoreProcedure.SPSetPromociones, cadenaConexion, promo.Transaccion, xmlParam.ToString());

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
