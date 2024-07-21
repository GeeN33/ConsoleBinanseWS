using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBinanseWS.lib
{
    public  class ServicRequest
    {
        static RestClient client;

        public ServicRequest(string endpoint_base)
        {
            client = new RestClient(endpoint_base);
        }

        public List<Symbolscl> GetSymbolInfo(string endpoint)
        {
            var request = new RestRequest(endpoint);

            var response2 = client.Get<Infobodi>(request);

            return response2.Symbols;

        }

        public List<BarCoinLopped> GetBarCoinLopped5(string endpoint,string symbol)
        {
            var request = new RestRequest(endpoint);

            request.AddParameter("symbol", symbol);

            request.AddParameter("per", 5);

            var response2 = client.Get<List<BarCoinLopped>>(request);

            return response2;

        }

        public List<BarCoinLopped> GetBarCoinLopped60(string endpoint, string symbol)
        {
            var request = new RestRequest(endpoint);

            request.AddParameter("symbol", symbol);

            request.AddParameter("per", 60);

            var response2 = client.Get<List<BarCoinLopped>>(request);

            return response2;

        }

        public bool UpBarCoin(string endpoint, int id, BarCoin barCoin)
        {
            bool res = false;

            var request = new RestRequest($"{endpoint}/{id}", Method.Put);

            request.AddJsonBody(barCoin);

            // Отправляем запрос и получаем ответ
            var response = client.Execute(request);

            // Обрабатываем ответ
            if (response.IsSuccessful)
            {
                res = true;
            }
            else
            {
                res = false;
            }
           
            return res;
        }

        public void UpLogger(string endpoint, string type, string description)
        {

            var request = new RestRequest(endpoint, Method.Post);

            request.AddJsonBody(new
            {
                type,
                description,
                createdat = DateTime.Now
            });

            client.Execute(request);

        }

    }
}
