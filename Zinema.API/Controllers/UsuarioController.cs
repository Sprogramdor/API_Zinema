using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;
using Zinema.API.Datos;
using Zinema.Modelo;


namespace Zinema.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase {
        private readonly IConfiguration _configuration;
        public UsuarioController(IConfiguration configuration) {
            _configuration = configuration;
        }
        [Route("[action]")]
        [HttpPost]
        // GENERAR EL TOKEN
        public async Task<ActionResult> PostLog([FromBody] Usuario usuario) {

            var cadenaConexion = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Conexion"];
            XDocument xmlParam = XmlMethods.GetXml(usuario);
            DataSet dsResultado = await XmlMethods.EjecutaBase(NameStoreProcedure.SPGetUsuario, cadenaConexion, usuario.Transaccion, xmlParam.ToString());

            List<Usuario> listaData = new List<Usuario>();

            if (dsResultado.Tables.Count > 0) {
                try {

                    if (dsResultado.Tables[0].Rows.Count > 0) {
                        Usuario usuarioTmp = new Usuario();
                        usuarioTmp.Usuario_id = Convert.ToInt32(dsResultado.Tables[0].Rows[0]["Usuario_id"]);
                        usuarioTmp.Username = dsResultado.Tables[0].Rows[0]["Username"].ToString();
                        usuarioTmp.Transaccion = usuario.Transaccion;
                        return Ok(JsonConvert.SerializeObject(CrearToken(usuarioTmp)));
                    } else {
                        RespuestaSP objresponse = new RespuestaSP();
                        objresponse.Leyenda = "Error en las credenciales de acceso";
                        objresponse.Respuesta = "Error";
                        return BadRequest(objresponse);
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
            return Ok(listaData);
        }
        private string CrearToken(Usuario user) {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Usuario_id.ToString()),
                new Claim(ClaimTypes.Name, user.Username.ToString()),
            };

            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value.PadRight(64, '0')));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [Route("[action]")]
        [HttpPost]
        [Authorize]
        // DEVOLVER EL PERFIL CON UN TOKEN
        public async Task<ActionResult<Usuario>> GetPerfil([FromBody] Usuario usuario) {
            var cadenaConexion = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Conexion"];
            XDocument xmlParam = XmlMethods.GetXml(usuario);
            DataSet dsResultado = await XmlMethods.EjecutaBase(NameStoreProcedure.SPGetUsuario, cadenaConexion, usuario.Transaccion, xmlParam.ToString());

            List<Usuario> listaData = new List<Usuario>();

            if (dsResultado.Tables.Count > 0) {
                try {
                    foreach (DataRow row in dsResultado.Tables[0].Rows) {
                        Usuario objResponse = new Usuario {
                            Usuario_id = Convert.ToInt32(row["Usuario_id"]),
                            Foto = row["Foto"].ToString(),
                            Rol = row["Rol"].ToString(),
                            Cedula = row["Cedula"].ToString(),
                            Nombres = row["Nombres"].ToString(),
                            Apellido = row["Apellido"].ToString(),
                            Ciudad = row["Ciudad"].ToString(),
                            Direccion = row["Direccion"].ToString(),
                            Email = row["Email"].ToString(),
                            Username = row["Username"].ToString(),
                            Password = row["Password"].ToString(),
                            Transaccion = usuario.Transaccion
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
        // PERFIL = EDITAR - ELIMINAR
        public async Task<ActionResult> SetUsuario([FromBody] Usuario user) {
            var cadenaConexion = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Conexion"];
            XDocument xmlParam = XmlMethods.GetXml(user);
            DataSet dsResultado = await XmlMethods.EjecutaBase(NameStoreProcedure.SPSetUsuario, cadenaConexion, user.Transaccion, xmlParam.ToString());

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


        // perfil = USUARIO Y CONTRASEÑA
        [Route("[action]")]
        [HttpPost]
        [Authorize]

        public async Task<ActionResult> PostLogUC([FromBody] Usuario usuario) {

            var cadenaConexion = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Conexion"];
            XDocument xmlParam = XmlMethods.GetXml(usuario);
            DataSet dsResultado = await XmlMethods.EjecutaBase(NameStoreProcedure.SPGetUsuario, cadenaConexion, usuario.Transaccion, xmlParam.ToString());

            List<Usuario> listaData = new List<Usuario>();

            if (dsResultado.Tables.Count > 0) {
                try {

                    if (dsResultado.Tables[0].Rows.Count > 0) {
                        Usuario usuarioTmp = new Usuario();
                        usuarioTmp.Usuario_id = Convert.ToInt32(dsResultado.Tables[0].Rows[0]["Usuario_id"]);
                        usuarioTmp.Foto = dsResultado.Tables[0].Rows[0]["Foto"].ToString();
                        usuarioTmp.Rol = dsResultado.Tables[0].Rows[0]["Rol"].ToString();
                        usuarioTmp.Cedula = dsResultado.Tables[0].Rows[0]["Cedula"].ToString();
                        usuarioTmp.Nombres = dsResultado.Tables[0].Rows[0]["Nombres"].ToString();
                        usuarioTmp.Apellido = dsResultado.Tables[0].Rows[0]["Apellido"].ToString();
                        usuarioTmp.Ciudad = dsResultado.Tables[0].Rows[0]["Ciudad"].ToString();
                        usuarioTmp.Direccion = dsResultado.Tables[0].Rows[0]["Direccion"].ToString();
                        usuarioTmp.Email = dsResultado.Tables[0].Rows[0]["Email"].ToString();
                        usuarioTmp.Username = dsResultado.Tables[0].Rows[0]["Username"].ToString();
                        usuarioTmp.Password = dsResultado.Tables[0].Rows[0]["Password"].ToString();
                        usuarioTmp.Transaccion = usuario.Transaccion;

                        listaData.Add(usuarioTmp);
                    } else {
                        RespuestaSP objresponse = new RespuestaSP();
                        objresponse.Leyenda = "Error en las credenciales de acceso";
                        objresponse.Respuesta = "Error";
                        return BadRequest(objresponse);
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
            return Ok(listaData);
        }


        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> PostRegistro([FromBody] Usuario usuario) {

            var cadenaConexion = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Conexion"];
            XDocument xmlParam = XmlMethods.GetXml(usuario);
            DataSet dsResultado = await XmlMethods.EjecutaBase(NameStoreProcedure.SPGetUsuario, cadenaConexion, usuario.Transaccion, xmlParam.ToString());


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
