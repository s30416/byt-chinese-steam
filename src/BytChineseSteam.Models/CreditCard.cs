namespace BytChineseSteam.Models;

public class CreditCard : PaymentMethod
{
    public string CardNumber { get; set; }
    public string Cvv { get; set; }
    public DateTime ExpiryDate { get; set; }

    public CreditCard(string name, string cardNumber, string cvv, DateTime expiryDate) : base(name)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            throw new ArgumentException("Card Number is required.", nameof(cardNumber));
        if (string.IsNullOrWhiteSpace(cvv))
            throw new ArgumentException("CVV is required.", nameof(cvv));

        CardNumber = cardNumber;
        Cvv = cvv;
        ExpiryDate = expiryDate;
    }
}