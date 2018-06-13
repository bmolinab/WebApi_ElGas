using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using WebApi_ElGas.Models;

namespace WebApi_ElGas.Utils
{
    /// <summary>
    /// Creado por Brian Molina 
    ///En DigitalStrategy.com.ec   
    /// Es una clase que nos permite Saber si estamos dentro de un Poligono
    /// o ver la distancia entre dos puntos.
    /// </summary>
    /// 
    public class Geo
    {
        /// <summary>
        /// en este metodo debemos pasar dos parametro para saber si esta o no dentro de nuestra posicion
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool EnElPoligono(ObservableCollection<Posicion> positions, Posicion position)

        {

            if (positions.Count < 3)

            {

                return false;

            }

            bool inside = false;

            Posicion p1, p2;

            Posicion oldPoint = positions[positions.Count - 1];

            foreach (Posicion newPoint in positions)

            {

                //Oredenamos los puntos  p1.lat <= p2.lat;

                if (newPoint.Latitud > oldPoint.Longitud)

                {

                    p1 = oldPoint;

                    p2 = newPoint;

                }

                else

                {

                    p1 = newPoint;

                    p2 = oldPoint;

                }



                // verifica si el punto esta dentro de ese poligono.

                if ((newPoint.Latitud < position.Latitud) == (position.Latitud <= oldPoint.Latitud)

                    && (position.Longitud - p1.Longitud) * (p2.Latitud - p1.Latitud)

                     < (p2.Longitud - p1.Longitud) * (position.Latitud - p1.Latitud))

                {

                    inside = !inside;

                }



                oldPoint = newPoint;

            }

            return inside;

        }
        /// <summary>
        /// Esta funcion nos regresa un boleano si las distancias estan dentro del rango del radio
        /// </summary>
        /// <param name="MyPosition"></param>
        /// <param name="OtherPosition"></param>
        /// <param name="radio"></param>
        /// <returns></returns>
        public static bool EstaCercaDeMi(Posicion MyPosition, Posicion OtherPosition, double radio)

        {

            //Radios en KM        
            if (Distancia(MyPosition.Latitud, MyPosition.Longitud, OtherPosition.Latitud, OtherPosition.Longitud) <= radio)

            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Obtiene las distancias
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lon1"></param>
        /// <param name="lat2"></param>
        /// <param name="lon2"></param>
        /// <returns></returns>
        static double Distancia(double lat1, double lon1, double lat2, double lon2)
        {

            var R = 6371; // El radio de la tierra en Kilometros
            var dLat = ToRadians(lat2 - lat1);  // Convertimos de grados a radianes
            var dLon = ToRadians(lon2 - lon1);
            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c; // Distancia en Kilometros
            return d;

        }
        static double ToRadians(double deg)
        {
            return deg * (Math.PI / 180);
        }
    }
}