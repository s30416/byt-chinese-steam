namespace BytChineseSteam.Models;

public class GooglePay : PaymentMethod
{
    public string Account { get; set; }

    public GooglePay(string name, string account) : base(name)
    {
        if (string.IsNullOrWhiteSpace(account))
            throw new ArgumentException("Account is required for Google Pay.", nameof(account));
            
        Account = account;
    }
}