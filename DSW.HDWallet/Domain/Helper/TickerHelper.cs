namespace DSW.HDWallet.Domain.Helper;

public class TickerHelper
{
    public static string GetNameByTicker(string ticker)
    {
        var enumType = typeof(CoinTicker);
        var enumValues = Enum.GetValues(enumType);

        foreach (var enumValue in enumValues)
        {
            var coin = (CoinTicker)enumValue;
            if (GetId(coin) == ticker)
            {
                return GetName(coin);
            }
        }

        return string.Empty;
    }

    public static string GetId(CoinTicker coin)
    {
        var fieldInfo = coin.GetType().GetField(coin.ToString());
        var attribute = (IdAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(IdAttribute));
        return attribute != null ? attribute.Id : coin.ToString();
    }

    public static string GetName(CoinTicker coin)
    {
        var fieldInfo = coin.GetType().GetField(coin.ToString());
        var attribute = (NameAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(NameAttribute));
        return attribute != null ? attribute.Name : coin.ToString();
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class IdAttribute : Attribute
{
    public string Id { get; }

    public IdAttribute(string id)
    {
        Id = id;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class NameAttribute : Attribute
{
    public string Name { get; }

    public NameAttribute(string name)
    {
        Name = name;
    }
}

public enum CoinTicker
{
    [Name("Azzure")]
    [Id("AZR")]
    AZR,

    [Name("Beacon")]
    [Id("BECN")]
    BECN,

    [Name("Birake")]
    [Id("BIR")]
    BIR,

    [Name("CryptoFlow")]
    [Id("CFL")]
    CFL,

    [Name("CryptoSaga")]
    [Id("SAGA")]
    SAGA,

    [Name("Dash Diamond")]
    [Id("DASHD")]
    DASHD,

    [Name("EskaCoin")]
    [Id("ESK")]
    ESK,

    [Name("Flits")]
    [Id("FLS")]
    FLS,

    [Name("Jackpot")]
    [Id("777")]
    _777,

    [Name("Kyanite")]
    [Id("KYAN")]
    KYAN,

    [Name("MobilityCoin")]
    [Id("MOBIC")]
    MOBIC,

    [Name("Monk")]
    [Id("MONK")]
    MONK,

    [Name("OneWorld Coin")]
    [Id("OWO")]
    OWO,

    [Name("Peony")]
    [Id("PNY")]
    PNY,

    [Name("Sapphire")]
    [Id("SAPP")]
    SAPP,

    [Name("Suvereno")]
    [Id("SUV")]
    SUV,

    [Name("Ultra Clear")]
    [Id("UCR")]
    UCR
}
