﻿using ConsoleBinanseWS.lib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace ConsoleBinanseWS;


public class WebSocketBnSpot
{
    WatchConnection watchConnection = new WatchConnection();

    WebSocket ws_tick;

    ServicRequest servicRequest;

    Rabbit_MQ rabbit;

    double last = 0;

    double deltaBuy5 = 0;
    double deltaSell5 = 0;

    double deltaBuy60 = 0;
    double deltaSell60 = 0;

    string side = "buy";

    Symbolscl symbolInfo;

    List<BarCoinLopped> barCoinLopped5 = new List<BarCoinLopped>();
    List<BarCoinLopped> barCoinLopped60 = new List<BarCoinLopped>();

    BarCoin barCoin5 = new BarCoin();

    BarCoin barCoin60 = new BarCoin();

    int indexMax5 = 0;
    int indexCurrent5 = 0;
    int indexMin5 = 0;

    int indexMax60 = 0;
    int indexCurrent60 = 0;
    int indexMin60 = 0;
    int indexCurrentSub60 = 1;
    bool debug = true;
    Config config;

    public WebSocketBnSpot(Symbolscl _symbolinfo, Config _config, Rabbit_MQ _rabbit)
    {
        this.rabbit = _rabbit;
        this.config = _config;
        this.symbolInfo = _symbolinfo;

        debug = config.Debug;

        servicRequest = new ServicRequest(config.UrlBar);

        barCoinLopped5 = servicRequest.GetBarCoinLopped5(config.UrlBarGetInfoEndpoint, symbolInfo.Symbol);

        Thread.Sleep(100);

        barCoinLopped60 = servicRequest.GetBarCoinLopped60(config.UrlBarGetInfoEndpoint, symbolInfo.Symbol);

        sortrdIndex();

        //if (debug) servicRequest.UpLogger("logger", $"Start {symbolInfo.Symbol}", symbolInfo.Symbol);

        //string Request = $"wss://fstream.binance.com/stream?streams={symbol.ToLower()}@miniTicker";

        string Request = $"wss://stream.binance.com:443/ws/{symbolInfo.Symbol.ToLower()}@aggTrade";

        ws_tick = new WebSocket(Request);

        if (config.Proxy_active && config.proxy != null)
        {
            var pxy = config.proxy;

            ws_tick.SetProxy($"http://{pxy.Ip}:{pxy.Port}", pxy.Login, pxy.Password);
        }

        ws_tick.OnMessage += Up_tick;

        ws_tick.OnError += Ws_tick_OnError;

        ServiceEvent.evenSTOP += evenSTOP;

        ServiceEvent.evenTimed += evenTimed;

        ServiceEvent.evenReConnection += evenReConnection;

    }

    public void Start()
    {

        ws_tick.Connect();
    }

    void Ws_tick_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
    {
        Re_Socket_tick();
    }

    void Up_tick(object sender, MessageEventArgs e)
    {
        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        var data = JObject.Parse(e.Data);
        var lastprice = Convert.ToDouble(data?["p"].ToString()); // Цена
        var quantity = Convert.ToDouble(data?["q"].ToString()); // Количество

        if (lastprice > last)
        {
            side = "buy";
        }

        if (lastprice < last)
        {
            side = "sell";
        }


        last = lastprice;


        if (side == "buy")
        {
            deltaBuy5 += quantity;
            deltaBuy60 += quantity;
        }

        if (side == "sell")
        {
            deltaSell5 += quantity;
            deltaSell60 += quantity;
        }

        barCoin5.SortedBarCoin(last, deltaBuy5, deltaSell5);
        barCoin60.SortedBarCoin(last, deltaBuy60, deltaSell60);

        watchConnection.CurrentUpTik = true;
        watchConnection.UpTikNot = 0;
        //Console.WriteLine($"Symbol: {symbol}, Price: {last}, Quantity: {quantity}, Side: {side}");

    }

    void evenTimed(DateTime dateTime, int id)
    {
        if (symbolInfo.Id == id && symbolInfo.Status == "TRADING")
        {

            if (!watchConnection.CurrentUpTik && watchConnection.UpTikNot < 3)
            {
                Re_Socket_tick();

                watchConnection.UpTikNot++;

                string log = $"ERROR id = {id}, {symbolInfo.Symbol} dateTime {dateTime}, UpTikNot = {watchConnection.UpTikNot}";

                Log logg = new Log { Type = "ERROR", Content = log };

                rabbit.send_Command(new CommandFromClient
                {
                    Id = id,
                    Type = "Logg",
                    Logg = logg
                });

                //servicRequest.UpLogger("logger", "ERROR Connection", log);
            }
            else
            {
                upBar5(dateTime);

                upBar60(dateTime);
            }


            watchConnection.CurrentUpTik = false;
        }

    }

    void Re_Socket_tick()
    {
        if (ws_tick != null) ws_tick.Close();
        ws_tick.Connect();
    }

    void ws_Closing()
    {
        if (ws_tick != null) ws_tick.Close();
    }

    void evenSTOP()
    {
        Console.WriteLine($"закрывается {symbolInfo.Symbol}");
        ws_Closing();
    }

    void sortrdIndex()
    {
        DateTime dateTime = DateTime.Now;
        DateTime newDateTime = dateTime.AddDays(-50);

        if (barCoinLopped5.Count > 0) indexMin5 = barCoinLopped5[barCoinLopped5.Count - 1].Index;

        foreach (var item in barCoinLopped5)
        {
            if (newDateTime <= item.Datetime)
            {
                newDateTime = item.Datetime;

            }

            if (indexMax5 <= item.Index)
            {
                indexMax5 = item.Index;
            }

            if (indexMin5 >= item.Index)
            {
                indexMin5 = item.Index;
            }
        }

        foreach (var item in barCoinLopped5)
        {
            if (newDateTime == item.Datetime)
            {
                indexCurrent5 = item.Index;

            }
        }

        newDateTime = dateTime.AddDays(-50);

        if (barCoinLopped60.Count > 0) indexMin60 = barCoinLopped60[barCoinLopped60.Count - 1].Index;

        foreach (var item in barCoinLopped60)
        {
            if (newDateTime <= item.Datetime)
            {
                newDateTime = item.Datetime;

            }

            if (indexMax60 <= item.Index)
            {
                indexMax60 = item.Index;
            }

            if (indexMin60 >= item.Index)
            {
                indexMin60 = item.Index;
            }
        }

        foreach (var item in barCoinLopped60)
        {
            //Console.WriteLine($"id = {item.Id}, per = 60, {symbolInfo.Symbol}");

            if (newDateTime == item.Datetime)
            {
                indexCurrent60 = item.Index;

            }
        }
    }

    void upBar5(DateTime dateTime)
    {
        int id = 0;

        foreach (var item in barCoinLopped5)
        {
            if (indexCurrent5 == item.Index)
            {
                id = item.Id;

            }
        }

        barCoin5.Datetime = dateTime;

        // var res = servicRequest.UpBarCoin(config.UrlBarCreateEndpoint, id, barCoin5);

        rabbit.send_Command(new CommandFromClient
        {
            Id = id,
            Type = "BarCoin",
            Barcoin = barCoin5,
        });

        barCoin5.ZeroBarCoin(last);
        deltaBuy5 = 0;
        deltaSell5 = 0;
        if (debug) Console.WriteLine($"per = 5, id = {id}, {symbolInfo.Symbol} dateTime {dateTime}, indexCurrent5 = {indexCurrent5}");


        if (indexCurrent5 >= indexMax5)
        {
            indexCurrent5 = indexMin5;

        }
        else
        {
            indexCurrent5++;
        }
    }

    void upBar60(DateTime dateTime)
    {
        if (indexCurrentSub60 >= 12)
        {
            int id = 0;

            foreach (var item in barCoinLopped60)
            {
                if (indexCurrent60 == item.Index)
                {
                    id = item.Id;

                }
            }

            barCoin60.Datetime = dateTime;

            // var res = servicRequest.UpBarCoin(config.UrlBarCreateEndpoint, id, barCoin60);

            rabbit.send_Command(new CommandFromClient
            {
                Id = id,
                Type = "BarCoin",
                Barcoin = barCoin60,
            });


            barCoin60.ZeroBarCoin(last);
            deltaBuy60 = 0;
            deltaSell60 = 0;
            if (debug) Console.WriteLine($"per = 60, id = {id}, {symbolInfo.Symbol} dateTime {dateTime}, indexCurrent60 = {indexCurrent60}, indexCurrentSub60 = {indexCurrentSub60}");


            if (indexCurrent60 >= indexMax60)
            {
                indexCurrent60 = indexMin60;

            }
            else
            {
                indexCurrent60++;
            }

            indexCurrentSub60 = 1;
        }
        else
        {
            indexCurrentSub60++;
        }

    }

    private void evenReConnection(int id)
    {
        if (symbolInfo.Id == id && symbolInfo.Status == "TRADING")
        {
            Re_Socket_tick();
        }
    }
}
