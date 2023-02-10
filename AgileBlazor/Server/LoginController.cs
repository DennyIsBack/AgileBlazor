using AgileBlazor.Shared.CompServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using static AgileBlazor.Shared.UserLogin;
using AgileBlazor.Shared.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace AgileBlazor.Server
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly IConfiguration configuration;

        public LoginController(IConfiguration _configuration) => configuration = _configuration;
        public string UserIp
        {
            get
            {
                var remoteIpAddress = HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();
                return remoteIpAddress == "::1" ? "127.0.0.1" : remoteIpAddress; //::1 indica que o ipv6 é localhost
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetImageBackground()
        {
            var urlImage = "";
            try
            {

                using (var service = NewServiceClient.InstanceServiceComp())
                {
                    var configAppEmpresa = service.ConsultaConfAppEmpresa(new VoCredencial());
                    if (configAppEmpresa != null)
                    {
                        if (configAppEmpresa.ByteArray.Length > 0 && configAppEmpresa.ByteArray != null)
                        {
                            urlImage = Convert.ToBase64String(configAppEmpresa.ByteArray);
                        }
                    }
                }
            }
            //if (ConfigApp.Get.ByteArray.Length > 0 && ConfigApp.Get.ByteArray != null)
            //{
            //    urlImage = Convert.ToBase64String(ConfigApp.Get.ByteArray);
            //}
            catch (Exception)
            {
            }
            return Json(new { TextString = urlImage });
        }


        [HttpGet("getConfiguration")]
        public async Task<JsonResult> getConfiguration()
        {
            try
            {
                appSettings configurationSettings = new appSettings()
                {
                    reCaptchaPublicKey = Convert.ToString(configuration["appSettings:reCaptchaPublicKey"]),
                    reCaptchaPrivateKey = Convert.ToString(configuration["appSettings:reCaptchaPrivateKey"]),
                    clientUseReCaptcha = Convert.ToBoolean(configuration["appSettings:clientUseReCaptcha"]),

                };

                return Json(new appSettings()
                {
                    reCaptchaPublicKey = configurationSettings.reCaptchaPublicKey,
                    reCaptchaPrivateKey = configurationSettings.reCaptchaPrivateKey,
                    clientUseReCaptcha = configurationSettings.clientUseReCaptcha
                });
            }

            catch (Exception ex)
            {
                return Json(new { message = ex });
            }

        }

        [HttpPost]
        public async Task<ActionResult<LoginViewModel>> LoginSystem(LoginViewModel model)
        {
            try
            {

                VoMensagemLogin ret;
                using (var service = NewServiceClient.InstanceServiceComp())
                {
                    ret = service.Login(new VoFiltroLogin() { Usuario = model.User.ToUpper(), Senha = model.Password, App = "Agile.MainApp", Ip = UserIp });
                }

                if (ret.TransacaoOk != StatusRetorno.OK)
                {
                    //Session["consConfigCashier" + key] = null;
                    //Session["consConfigReq" + key] = null;
                    //Session["consConfigReqInBatch" + key] = null;
                    //Session["consConfigClient" + key] = null;
                    //Session["consConfigExams" + key] = null;
                    //Session["consConfigPanel" + key] = null;
                    //Session["consConfigTable" + key] = null;

                    return Json(new { success = false, message = ret.Mensagem });
                }

                if (ret.Usuario.StatusLogin == EnumStatusLogin.LoginValido)
                {
                    var userData = new UserData()
                    {
                        UserName = model.User.ToUpper(),
                        Password = model.Password,
                        SessionUserId = (string.IsNullOrEmpty(ret.Usuario.IdSessao) ? "" : ret.Usuario.IdSessao.Replace("#", ""))/*Este replace é feito no silverlight*/,
                        UserDatabase = ret.Usuario.NomeUsuario,
                        AccessKey = ret.Usuario.NomeBaseDados,
                        Customer = "01" /*está sempre fixo em 01 pois o sistema não está trabalhando com multi empresa*/,
                        Division = "01" /*está sempre fixo em 01 pois o sistema não está tratando a divisão*/
                    };

                    //Seta os papeis na claim para login
                    var claims = new List<Claim>()
                    {
                        new Claim(JwtRegisteredClaimNames.UniqueName, userData.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.Name, userData.UserName),
                        new Claim(ClaimTypes.Role, "LAB_LAUD")
                };

                //verificar papel do laudo
                //foreach (var paper in ret.PermissaoUsuario.Direitos)
                //{
                //    if ((new string[] { "PAP_ASSLAUD", "PAP_DIGLAUD", "PAP_DIGLAUDCOOR" }).Contains(paper.Papel.Codigo))
                //        claims.Add(new Claim(ClaimTypes.Role, "LAB_LAUD"));
                //}

                //Seta as permissões na variavel de sessão
                var keyjwt = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSecurityKey"]));
                var creds = new SigningCredentials(keyjwt, SecurityAlgorithms.HmacSha256);
                var expiry = DateTime.Now.AddHours(Convert.ToInt32(configuration["JwtExpiryInHours"]));

                //Cria jwt Token
                JwtSecurityToken token = new JwtSecurityToken(
                    issuer: configuration["JwtIssuer"],
                    audience: configuration["JwtAudience"],
                    claims: claims,
                    expires: expiry,
                    signingCredentials: creds);


                return Json(new UserToken() { Success = true, Token = new JwtSecurityTokenHandler().WriteToken(token), Expiry = expiry });
            }
                else
            {
                string pMensagem = ret.Usuario.Mensagem;
                if (String.IsNullOrEmpty(pMensagem))
                {
                    pMensagem = ret.Usuario.Status;
                    if (String.IsNullOrEmpty(pMensagem))
                    {
                        pMensagem = ret.Usuario.StatusLogin.ToString();
                    }
                }
                return Json(new UserToken() { Success = false, Token = pMensagem, Expiry = DateTime.MinValue });
            }
        }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Não foi possível realizar o login -" + ex.Message
    });
            }
        }
    }


    // Se precisar registrar tem que gerar um token register função exemplo 
    //private UserToken GenerateTokenRegister(UserInfo user)
    //{
    //    var claims = new List<Claim>()
    //        {
    //            new Claim(JwtRegisteredClaimNames.UniqueName, user.Email),
    //            new Claim(ClaimTypes.Name, user.Email),
    //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    //        };
    //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
    //    var creds =
    //        new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //    var expiration = DateTime.UtcNow.AddHours(2);
    //    var message = "Token JWT criado com sucesso";

    //    JwtSecurityToken token = new JwtSecurityToken(
    //        issuer: null,
    //        audience: null,
    //        claims: claims,
    //        expires: expiration,
    //        signingCredentials: creds
    //        );
    //    return new UserToken()
    //    {
    //        Token = new JwtSecurityTokenHandler().WriteToken(token),
    //        Expiration = expiration,
    //        Message = message
    //    };
    //}
}
