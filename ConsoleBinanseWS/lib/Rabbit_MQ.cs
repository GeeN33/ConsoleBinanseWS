using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBinanseWS.lib;


public class Rabbit_MQ
{
    private IConnection connection;

    private IModel channel;

    string queue_client = "";

    string ip = "";

    string userName = "";
   
    string password = "";

    public delegate void delegUp_CloseEvent();

    public event delegUp_CloseEvent evenClose;

    public Rabbit_MQ(string ip, string queue_client, string _userName, string _password)
    {
        userName = _userName;
        password = _password;
        this.queue_client = queue_client;
        this.ip = ip;
    }

    public void start()
    {

        var factory = new ConnectionFactory
        {
            HostName = ip,
            UserName = userName,
            Password = password
        };

        connection = factory.CreateConnection();

        channel = connection.CreateModel();
        channel.QueueDeclare(queue: queue_client,
                 durable: false,
                 exclusive: false,
                 autoDelete: false,
                 arguments: null);

    }

    public void send_Command(CommandFromClient command)
    {
        if (connection != null || connection.IsOpen || channel != null || channel.IsOpen)
        {
            var message = JsonConvert.SerializeObject(command, Formatting.Indented);

            var body = Encoding.UTF8.GetBytes(message);
            try
            {
                channel.BasicPublish(exchange: string.Empty,
                         routingKey: queue_client,
                         basicProperties: null,
                         body: body);
            }
            catch { }
        }
    }

    public void close()
    {
        channel?.Close();
        connection?.Close();
    }

    public void CheckConnect()
    {
        if (connection == null || !connection.IsOpen || channel == null || !channel.IsOpen)
        {
            evenClose?.Invoke();
        }
    }
}


