using ConsoleBinanseWS;
using ConsoleBinanseWS.lib;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Timers;
using DotNetEnv;


Env.Load();

System.Timers.Timer aTimer;

Config config = new Config();

config.LoadConfig();

WatchConnection watchConnection = new WatchConnection();

Rabbit_MQ rabbit = new Rabbit_MQ(config.Rabbit_MQ_IP, config.Rabbit_MQ_Queue, config.UserName, config.Password);

rabbit.evenClose += Rabbit_evenClose;

Console.CancelKeyPress += new ConsoleCancelEventHandler(OnExit);

ServicRequest servicRequest = new ServicRequest(config.UrlInfo);

SymbolInfo symbolInfo = new SymbolInfo();

var symbolsInfos = servicRequest.GetSymbolInfo($"{config.UrlInfoEndpoint}{config.IdGroup}/?format=json");


List<Thread> Threadlists = new List<Thread>();

void RUNS()
{
    rabbit.start();

    StartInfo();

    foreach (var item in symbolsInfos.Symbols)
    {
        if (item.Status == "TRADING")
        {
            WebSocketBn socketBn = new WebSocketBn(item, config, rabbit);
            Threadlists.Add(new Thread(socketBn.Start));
            if (config.Debug) Console.WriteLine($"Start {item.Symbol}");
        }
    }

    foreach (var thd in Threadlists)
    {
        thd.Start();
        Thread.Sleep(500);
    }

}

void RUNS_spot()
{
    rabbit.start();

    StartInfo();

    foreach (var item in symbolsInfos.Symbols)
    {
        if (item.Status == "TRADING")
        {
            WebSocketBnSpot socketBn = new WebSocketBnSpot(item, config, rabbit);
            Threadlists.Add(new Thread(socketBn.Start));
            if (config.Debug) Console.WriteLine($"Start {item.Symbol} spot");
        }
    }

    foreach (var thd in Threadlists)
    {
        thd.Start();
        Thread.Sleep(500);
    }

}

await Task.Run(() =>
{
    SetTimer();

    if (config.Spot)
    {
        RUNS_spot();
    }
    else
    {
        RUNS();
    }

    
});

await Task.Delay(-1);

void SetTimer()
{
    // Create a timer with a two second interval.
    aTimer = new System.Timers.Timer(300000);
    // Hook up the Elapsed event for the timer. 
    aTimer.Elapsed += OnTimedEvent;
    aTimer.AutoReset = true;
    aTimer.Enabled = true;
}

void OnTimedEvent(Object source, ElapsedEventArgs e)
{
    rabbit.CheckConnect();

    DateTime dateTime = GetNowMSK();

    int hour = dateTime.Hour;
   
    if (watchConnection.Go && watchConnection.TargetHour.Contains(hour))
    {
        string sf = "futures";
        
        if (config.Spot)
        {
            sf = "spot";
        }

        string log = $"{dateTime} ReConnection hour {hour}, {sf}, Group {config.IdGroup}";

        Log logg = new Log { Type = "ReConnection", Content = log };

        rabbit.send_Command(new CommandFromClient
        {
            Id = 0,
            Type = "Logg",
            Logg = logg
        });


        foreach (var item in symbolsInfos.Symbols)
        {
            ServiceEvent.ReConnection(item.Id);
            Thread.Sleep(300);
        }

        watchConnection.Go = false;

    }
    
    if (!watchConnection.TargetHour.Contains(hour) && !watchConnection.Go)
    {
        watchConnection.Go = true;
    }

    foreach (var item in symbolsInfos.Symbols)
    {
        ServiceEvent.Timed(dateTime, item.Id);
        Thread.Sleep(100);
    }

}

static void OnExit(object sender, ConsoleCancelEventArgs args)
{
    // Сообщаем, что событие обработано, чтобы предотвратить немедленный выход
    args.Cancel = true;

    // Выполняем завершающие действия
    Console.WriteLine("Программа закрывается...");
    ServiceEvent.STOP();
    // Завершаем выполнение
    Environment.Exit(0);
}

void Rabbit_evenClose()
{
    // Выполняем завершающие действия
    Console.WriteLine("Программа закрывается...");
    rabbit.close();
    ServiceEvent.STOP();
    // Завершаем выполнение
    Environment.Exit(0);
}

void StartInfo()
{
    DateTime dateTime = GetNowMSK();

    string log = $"{dateTime} Start Group {config.IdGroup}, queue - {config.Rabbit_MQ_Queue}";

    Log logg = new Log { Type = "Start", Content = log };

    rabbit.send_Command(new CommandFromClient
    {
        Id = 0,
        Type = "Logg",
        Logg = logg
    });



}

DateTime GetNowMSK()
{
    DateTime utcTime = DateTime.UtcNow;

    // Получаем информацию о часовом поясе MSK
    TimeZoneInfo moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");

    // Преобразуем время к московскому времени
    DateTime moscowTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, moscowTimeZone);

    return moscowTime;
}