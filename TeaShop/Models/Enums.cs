namespace TeaShop.Models;

public enum CaffeineType
{
    NotApplicable,
    Caffeinated,
    CaffeineFree
}

public enum OrderStatus
{
    AwaitingPayment,
    Paid,
    Processing,
    Shipped,
    Completed,
    Cancelled
}