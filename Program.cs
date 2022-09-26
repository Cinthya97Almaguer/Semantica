//BRIONES ALMAGUER CINTHYA CRISTINA


using System;
using System.IO;

namespace Semantica
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Lenguaje a = new Lenguaje();

                //a.Programa();
                byte x = 255;
                Console.WriteLine(x);
                x++;
                x++;
                Console.WriteLine(x);

                //a.cerrar();
            }
            catch (Exception e)
            {
                //Console.WriteLine("Fin de compilacion");
                Console.WriteLine(e.Message);
            }
        }
    }
}
