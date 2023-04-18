using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Xml.Linq;
using Zinema.API.Datos;
using Zinema.Modelo;

namespace Zinema.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class SnacksController : ControllerBase {

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<Snacks>> GetSnacks(string transaccion) {
            Snacks snacksTmp = new Snacks();
            snacksTmp.Transaccion = transaccion;

            var cadenaConexion = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Conexion"];
            XDocument xmlParam = XmlMethods.GetXml(snacksTmp);
            DataSet dsResultado = await XmlMethods.EjecutaBase(NameStoreProcedure.SPGetSnacks, cadenaConexion, snacksTmp.Transaccion, xmlParam.ToString());

            List<Snacks> listaData = new List<Snacks>();

            if (dsResultado.Tables.Count > 0) {
                try {
                    foreach (DataRow row in dsResultado.Tables[0].Rows) {
                        Snacks objResponse = new Snacks {
                            Snacks_id = Convert.ToInt32(row["Snacks_id"]),
                            Image = row["Image"].ToString(),
                            Nombre = row["Nombre"].ToString(),
                            Descripcion = row["Descripcion"].ToString(),
                            Precio = float.Parse(row["Precio"].ToString()),
                            Transaccion = snacksTmp.Transaccion
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

        public async Task<ActionResult> SetSnacks([FromBody] Snacks snack) {
            var cadenaConexion = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Conexion"];
            XDocument xmlParam = XmlMethods.GetXml(snack);
            DataSet dsResultado = await XmlMethods.EjecutaBase(NameStoreProcedure.SPSetSnacks, cadenaConexion, snack.Transaccion, xmlParam.ToString());

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
