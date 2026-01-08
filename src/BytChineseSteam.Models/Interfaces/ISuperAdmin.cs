namespace BytChineseSteam.Models.Interfaces;

public interface ISuperAdmin
{
    
    IReadOnlyCollection<Employee> CreatedEmployees { get; }
    void AddCreatedEmployee(Employee employee);
    void RemoveCreatedEmployee(Employee employee);
}