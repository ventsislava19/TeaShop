using System.Text.Json;
 
namespace TeaShop.Helpers;
 
// ASP.NET sessions can only store strings and byte arrays natively.
// These extension methods let us store any object by serializing it to JSON.
// Usage: HttpContext.Session.Set("cart", myCartList);
//        var cart = HttpContext.Session.Get<List<CartItem>>("cart");
public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }
 
    public static T? Get<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
}