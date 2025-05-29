namespace Persic.EF.Postgres.Playground;

public static class Seeds
{
    public const string HelloWorld = "Hello World!";
    public const string HelloJohn = "Hello John!";
    public const string ByeJohn = "Bye, John!";
    public const string Bye = "Bye!";
    public const string JackBlack = "Jack Black";
    public const string JackCustome = "Jack Custome";
    public const string JackBrown = "Jack Brown";

    public static readonly string[] All =
    [
        HelloWorld,
        HelloJohn,
        ByeJohn,
        Bye,
        JackBlack,
        JackCustome,
        JackBrown
    ];

    public static T[] As<T>(Func<string, T> factory) => [.. All.Select(factory)];
}