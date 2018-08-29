using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using WebApi_ElGas.Controllers;
using WebApi_ElGas.Models;
using WebApi_ElGas.Utils;

namespace WebApi_ElGas.Plugins
{
    public class Correo
    {

        public async Task<Response> Enviar(PasswordRequest contrasenaRequest)
        {
            try
            {

                //Configuring webMail class to send emails  
                //gmail smtp server  
                WebMail.SmtpServer = CorreoUtil.SmtpServer;
                //gmail port to send emails  
                WebMail.SmtpPort = Convert.ToInt32(CorreoUtil.Port);
                WebMail.SmtpUseDefaultCredentials = true;
                //sending emails with secure protocol  
                WebMail.EnableSsl = true;
                //EmailId used to send emails from application  
                WebMail.UserName = CorreoUtil.UserName;
                WebMail.Password = CorreoUtil.Password;

                //Sender email address.  
                WebMail.From = CorreoUtil.UserName;


                //Send email  
                WebMail.Send(to: contrasenaRequest.Email, subject: "Código de Seguridad: " + contrasenaRequest.Codigo, body: "Código de seguridad: " + contrasenaRequest.Codigo, isBodyHtml: true);
                return new Response { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new Response { IsSuccess = false, Message = ex.Message };
                throw;
            }
        }

        public int GenerarCodigo()
        {

            var rdm = new Random();
            var numeroAleatorio = rdm.Next(1000, 9999);
            return numeroAleatorio;

        }
    }

    

}