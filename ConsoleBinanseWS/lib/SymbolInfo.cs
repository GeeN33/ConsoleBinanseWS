using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBinanseWS.lib;

public class SymbolInfo
{
    public int Id { get; set; }
    
    public List<Symbolscl> Symbols { get; set; }
    
    public string Name { get; set; }

    public Proxy proxy { get; set; }

    public bool Proxy_active { get; set; } = false;

}

public class Symbolscl
{
    public int Id { get; set; }
    public string Symbol { get; set; }
    public string Pair { get; set; }
    public string ContractType { get; set; }
    public long DeliveryDate { get; set; }
    public long OnboardDate { get; set; }
    public string Status { get; set; }
    public string MaintMarginPercent { get; set; }
    public string RequiredMarginPercent { get; set; }
    public string BaseAsset { get; set; }
    public string QuoteAsset { get; set; }
    public string MarginAsset { get; set; }
    public int PricePrecision { get; set; }
    public int QuantityPrecision { get; set; }
    public int BaseAssetPrecision { get; set; }
    public int QuotePrecision { get; set; }
    public string UnderlyingType { get; set; }
    public List<string> UnderlyingSubType { get; set; }
    public int SettlePlan { get; set; }
    public string TriggerProtect { get; set; }
    public string LiquidationFee { get; set; }
    public string MarketTakeBound { get; set; }
    public int MaxMoveOrderLimit { get; set; }
    public List<string> OrderTypes { get; set; }
    public List<string> TimeInForce { get; set; }
    public List<Filters> Filters { get; set; }

}

public class Filters
{
    public string FilterType { get; set; }
    public string MaxPrice { get; set; }
    public string MinPrice { get; set; }
    public string TickSize { get; set; }
    public string StepSize { get; set; }
    public string MaxQty { get; set; }
    public string MinQty { get; set; }
    public int Limit { get; set; }
    public string Notional { get; set; }
    public string MultiplierUp { get; set; }
    public string MultiplierDown { get; set; }
    public string MultiplierDecimal { get; set; }

}

public class Proxy
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Ip { get; set; }
    public int Port { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
}