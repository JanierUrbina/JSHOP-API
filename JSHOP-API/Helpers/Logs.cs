using System.IO;
using System.Text;

namespace Api_ChatN.Helpers
{
    public class Logs
    {
        public static void LogErrores(string Error)
        {
            var path = @"C:\ErroresStock";
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var pathfield = @"C:\\ErroresStock\\Stock " + DateTime.Now.ToString("dd-MMMM-yyyyy") +".txt";
            StreamWriter sw = new StreamWriter(pathfield, true, ASCIIEncoding.ASCII);
            sw.WriteLine("Error a las: " + DateTime.Now.ToString("hh:mm:ss"));
            sw.WriteLine("Razón: "+Error);
            sw.Close();
        }
    }
}
