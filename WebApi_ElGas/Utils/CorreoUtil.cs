﻿    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_ElGas.Utils
{
    public static class CorreoUtil
    {
        public static string SmtpServer { get; set; }
        public static string Port { get; set; }
        public static bool EnableSsl { get; set; }
        public static string UserName { get; set; }
        public static string Password { get; set; }
    }
}