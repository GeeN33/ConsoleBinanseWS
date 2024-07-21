using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetEnv;

namespace ConsoleBinanseWS.lib
{
    public class Config
    {
        public string UrlInfo { get; set; } = "";
        public string UrlBar { get; set; } = "";
        public int IdGroup { get; set; } = 3;
        public bool Debug { get; set; } = true;

        public void LoadConfig()
        {
            try
            {
                string UrlInfo = Environment.GetEnvironmentVariable("UrlInfo");
                string UrlBar = Environment.GetEnvironmentVariable("UrlBar");
                string IdGroup = Environment.GetEnvironmentVariable("IdGroup");
                string Debug = Environment.GetEnvironmentVariable("Debug");

                this.UrlInfo = UrlInfo;
                this.UrlBar = UrlBar;
                this.IdGroup = Convert.ToInt32(IdGroup);

                if(Debug == "yes") { this.Debug = true; }else{ this.Debug = false; }

                Console.WriteLine("Use Config");
            }
            catch
            {
                Console.WriteLine("Not Config Use Default");
            }
        }
    }

}
