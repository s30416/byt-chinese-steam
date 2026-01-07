using System.Collections.ObjectModel;
using BytChineseSteam.Repository.Extent;

namespace BytChineseSteam.Models
{
    public enum PaymentMethodType
    {
        GooglePay,
        ApplePay,
        CreditCard
    }

    public class PaymentMethod
    {
        private static readonly Extent<PaymentMethod> Extent = new();

        // Common Attribute
        public string Name { get; set; }

        // Discriminator
        public PaymentMethodType Type { get; private set; }

        // Flattened Attributes
        private string? _account; // Shared by GooglePay and ApplePay
        private string? _cardNumber;
        private string? _cvv;
        private DateTime? _expiryDate;

        // Association Management
        private HashSet<Order> _orders = new();

        // Constructor for GooglePay and ApplePay (They share the same structure: 'Account')
        public PaymentMethod(PaymentMethodType type, string name, string account)
        {
            if (type == PaymentMethodType.CreditCard)
                throw new ArgumentException("This constructor is for GooglePay or ApplePay only.");

            if (string.IsNullOrWhiteSpace(account))
                throw new ArgumentException("Account is required.");

            Type = type;
            Name = name;
            _account = account;

            Extent.Add(this);
        }
        
        public PaymentMethod(string name, string cardNumber, string cvv, DateTime expiryDate)
        {
            Type = PaymentMethodType.CreditCard;
            Name = name;

            if (string.IsNullOrWhiteSpace(cardNumber) || string.IsNullOrWhiteSpace(cvv))
                throw new ArgumentException("Card details are required.");

            _cardNumber = cardNumber;
            _cvv = cvv;
            _expiryDate = expiryDate;

            Extent.Add(this);
        }
        
        // Properties with Validation (Enforcing Disjoint Constraints)
        
        // for GooglePay and ApplePay
        public string Account
        {
            get
            {
                if (Type == PaymentMethodType.CreditCard)
                    throw new InvalidOperationException("Account property is not available for Credit Cards.");
                return _account!;
            }
            set
            {
                if (Type == PaymentMethodType.CreditCard)
                    throw new InvalidOperationException("Cannot set Account for a Credit Card.");
                _account = value;
            }
        }

        // for CreditCard
        public string CardNumber
        {
            get
            {
                if (Type != PaymentMethodType.CreditCard)
                    throw new InvalidOperationException($"CardNumber is not available for {Type}.");
                return _cardNumber!;
            }
            set
            {
                if (Type != PaymentMethodType.CreditCard)
                    throw new InvalidOperationException($"Cannot set CardNumber for {Type}.");
                _cardNumber = value;
            }
        }

        // for CreditCard
        public string Cvv
        {
            get
            {
                if (Type != PaymentMethodType.CreditCard)
                    throw new InvalidOperationException($"CVV is not available for {Type}.");
                return _cvv!;
            }
            set
            {
                if (Type != PaymentMethodType.CreditCard)
                    throw new InvalidOperationException($"Cannot set CVV for {Type}.");
                _cvv = value;
            }
        }

        // for CreditCard
        public DateTime ExpiryDate
        {
            get
            {
                if (Type != PaymentMethodType.CreditCard)
                    throw new InvalidOperationException($"ExpiryDate is not available for {Type}.");
                return _expiryDate!.Value;
            }
            set
            {
                if (Type != PaymentMethodType.CreditCard)
                    throw new InvalidOperationException($"Cannot set ExpiryDate for {Type}.");
                _expiryDate = value;
            }
        }
        
        // Association Methods
        public bool ContainsOrder(Order order) => _orders.Contains(order);
        
        public ReadOnlyCollection<Order> GetOrders() => _orders.ToList().AsReadOnly();

        public void AddOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (_orders.Contains(order)) return;

            _orders.Add(order);

            // Reverse connection management logic
            if (order.PaymentMethod != this)
                order.AddPaymentMethod(this);
        }

        public void RemoveOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!_orders.Contains(order)) return;

            _orders.Remove(order);

            if (order.PaymentMethod == this)
                order.RemovePaymentMethod();
        }

        public static ReadOnlyCollection<PaymentMethod> GetAll()
        {
            return Extent.All();
        }
    }
}