namespace BytChineseSteam.Models;

public abstract class Employee // extends User class in the future
{
    
    public decimal? Salary { get; set; }

    public decimal CollectedBonuses { get; set; } = 0;

    protected Employee(decimal? salary, decimal collectedBonuses)
    {
        Salary = salary;
        CollectedBonuses = collectedBonuses;
    }

    protected Employee(decimal? salary)
    {
        Salary = salary;
    }

    protected Employee() {}
}