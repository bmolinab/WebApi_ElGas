using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WebApi_ElGas.Hubs
{
    public class AzureHubUtils
    {
        private static NotificationHubClient hub;
        public const string ListenConnectionString = "Endpoint=sb://notificacionesdesarrollo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=B10SdmICrxrk4RAZ3abyGBB3SdrntdmW+iKImvByLIQ=";
        public const string NotificationHubName = "notificacionesds";


        public static async Task<bool> SendNotificationAsync(string message, System.Collections.Generic.IEnumerable<string> tags, string to, string tipo, int idCompra, string idDistribuidor)
        {
            hub = NotificationHubClient.CreateClientFromConnectionString(ListenConnectionString, NotificationHubName);
            var notif = ("{\"data\":{\"message\":\"" + message + "\"}}");

            if (to!="" && to != null)
            {

           
                 notif = ("{\"to\":\"" +to.ToString()+
                    "\",\"data\":" +
                    "{\"message\":\"" + message + "\",\"tipo\":\""+tipo+"\",\"idCompra\":\""+idCompra+ "\",\"idDistribuidor\":\"" + idDistribuidor + "\"}" +
                    "}");

            }
             hub.SendGcmNativeNotificationAsync(notif,tags);
             return true;
        }
    }
}