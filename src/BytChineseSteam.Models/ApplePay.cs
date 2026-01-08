namespace BytChineseSteam.Models;

public class ApplePay : PaymentMethod
{
    public string Account { get; set; }

    public ApplePay(string name, string account) : base(name)
    {
        if (string.IsNullOrWhiteSpace(account))
            throw new ArgumentException("Account is required for Apple Pay.", nameof(account));

        Account = account;
    }
}