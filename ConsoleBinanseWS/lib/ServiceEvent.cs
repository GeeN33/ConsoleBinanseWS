using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBinanseWS.lib;

public static class ServiceEvent
{
    public delegate void delegUp_event(); // 1. Объявляем делегат

    static public event delegUp_event evenSTOP;

    public delegate void delegUp_TimedEvent(DateTime dateTime, int id); // 1. Объявляем делегат

    static public event delegUp_TimedEvent evenTimed;

    public delegate void delegUp_ReConnectionEvent(int id); // 1. Объявляем делегат

    static public delegUp_ReConnectionEvent evenReConnection;

    public static void STOP()
    {
        evenSTOP?.Invoke();
    }

    public static void Timed(DateTime dateTime, int id)
    {
        evenTimed?.Invoke(dateTime, id);
    }

    public static void ReConnection(int id)
    {
        evenReConnection?.Invoke(id);
    }
}
