namespace BytChineseSteam.Repository.Extent;

public class ExtentModel<T>
{
    public static readonly Extent<T> Extent = new ();

    // protected ExtentModel()
    // {
    // }
    //
    // public static T Create(Action<T> initialize)
    // {
    //     ArgumentNullException.ThrowIfNull(initialize);
    //     var item = new T();
    //     initialize.Invoke(item);
    //     Extent.Add(item);
    //     return item;
    // }
}