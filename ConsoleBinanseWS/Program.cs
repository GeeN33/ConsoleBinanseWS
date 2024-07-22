using ConsoleBinanseWS;
using ConsoleBinanseWS.lib;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Timers;
using DotNetEnv;


// Загрузить переменные окружения из файла .env
Env.Load();

System.Timers.Timer aTimer;

Config config = new Config();

config.LoadConfig();

WatchConnection watchConnection = new WatchConnection();

Console.CancelKeyPress += new ConsoleCancelEventHandler(OnExit);

ServicRequest servicRequest = new ServicRequest(config.UrlInfo);

SymbolInfo symbolInfo = new SymbolInfo();

var symbolsInfos = servicRequest.GetSymbolInfo($"f-binance/group/{config.IdGroup}/?format=json");

List<Thread> Threadlists = new List<Thread>();

void RUNS()
{

    foreach (var item in symbolsInfos)
    {
        if (item.Status == "TRADING")
        {
            WebSocketBn socketBn = new WebSocketBn(item, config);
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

await Task.Run(() =>
{
    SetTimer();
    RUNS();
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

    DateTime dateTime = GetNowMSK();

    int hour = dateTime.Hour;

    if (watchConnection.CurrentHour && watchConnection.TargetHour.Contains(hour))
    {
        servicRequest.UpLogger("f-binance/update-log/", "ReConnection", $"ReConnection hour {hour}");
       
        foreach (var item in symbolsInfos)
        {
            ServiceEvent.ReConnection(item.Id);
            Thread.Sleep(300);
        }

        watchConnection.CurrentHour = false;
    }
    
    if (!watchConnection.TargetHour.Contains(hour) && !watchConnection.CurrentHour)
    {
        watchConnection.CurrentHour = true;
    }

    foreach (var item in symbolsInfos)
    {
        ServiceEvent.Timed(dateTime, item.Id);
        Thread.Sleep(600);
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



DateTime GetNowMSK()
{
    DateTime utcTime = DateTime.UtcNow;

    // Получаем информацию о часовом поясе MSK
    TimeZoneInfo moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");

    // Преобразуем время к московскому времени
    DateTime moscowTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, moscowTimeZone);

    return moscowTime;
}
