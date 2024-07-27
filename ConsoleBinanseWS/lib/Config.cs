using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetEnv;
using Microsoft.VisualBasic;
using static System.Net.WebRequestMethods;

namespace ConsoleBinanseWS.lib
{
    public class Config
    {
        public string SecretKey { get; set; } = "";

        public string UrlInfo { get; set; } = "";

        public string UrlInfoEndpoint { get; set; } = "";
        
        public string UrlInfoLog { get; set; } = "";
       
        public string UrlBar { get; set; } = "";

        public string UrlBarCreateEndpoint { get; set; } = "";

        public string UrlBarGetInfoEndpoint { get; set; } = "";
        public string Rabbit_MQ_IP { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
        public string Rabbit_MQ_Queue { get; set; } = "";

        public int IdGroup { get; set; } = 3;
       
        public bool Debug { get; set; } = true;
       
        public bool Spot { get; set; } = false;

        public Proxy proxy { get; set; }

        public bool Proxy_active { get; set; } = false;

        public void LoadConfig()
        {
            try
            {

                string SecretKey = Environment.GetEnvironmentVariable("SecretKey");
                string UrlInfo = Environment.GetEnvironmentVariable("UrlInfo");
                string UrlInfoEndpoint = Environment.GetEnvironmentVariable("UrlInfoEndpoint");
                string UrlInfoLog = Environment.GetEnvironmentVariable("UrlInfoLog");
                string UrlBar = Environment.GetEnvironmentVariable("UrlBar");
                string UrlBarCreateEndpoint = Environment.GetEnvironmentVariable("UrlBarCreateEndpoint");
                string UrlBarGetInfoEndpoint = Environment.GetEnvironmentVariable("UrlBarGetInfoEndpoint");

                string Rabbit_MQ_IP = Environment.GetEnvironmentVariable("Rabbit_MQ_IP");
                string UserName = Environment.GetEnvironmentVariable("UserName");
                string Password = Environment.GetEnvironmentVariable("Password");
                string Rabbit_MQ_Queue = Environment.GetEnvironmentVariable("Rabbit_MQ_Queue");

                string IdGroup = Environment.GetEnvironmentVariable("IdGroup");
                string Debug = Environment.GetEnvironmentVariable("Debug");
                string Spot = Environment.GetEnvironmentVariable("Spot");

                this.SecretKey = SecretKey;
                this.UrlInfo = UrlInfo;
                this.UrlInfoEndpoint = UrlInfoEndpoint;
                this.UrlInfoLog = UrlInfoLog;
                this.UrlBar = UrlBar;
                this.UrlBarCreateEndpoint = UrlBarCreateEndpoint;
                this.UrlBarGetInfoEndpoint = UrlBarGetInfoEndpoint;

                this.Rabbit_MQ_IP = Rabbit_MQ_IP;
                this.UserName = UserName;
                this.Password = Password;
                this.Rabbit_MQ_Queue = Rabbit_MQ_Queue;

                this.IdGroup = Convert.ToInt32(IdGroup);

               
                
                if (Debug == "yes") { this.Debug = true; }else{ this.Debug = false; }
                if (Spot == "yes") { this.Spot = true; } else { this.Spot = false; }

                Console.WriteLine("Use Config");
            }
            catch
            {
                Console.WriteLine("Not Config Use Default");
            }
        }
    }

}
