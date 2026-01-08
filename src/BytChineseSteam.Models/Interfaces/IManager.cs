namespace BytChineseSteam.Models.Interfaces;

public interface IManager
{
    
    IReadOnlyCollection<Promotion> Promotions { get; }
    void AddPromotion(Promotion promotion);
    void RemovePromotion(Promotion promotion);
}