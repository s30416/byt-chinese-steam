using System.ComponentModel.DataAnnotations;

namespace BytChineseSteam.Tests;

public class EmployeeTests
{
    private static void ValidateModel(object model)
    {
        var context = new ValidationContext(model);

        Validator.ValidateObject(model, context, true);
    }
    
    // todo: write employee tests when the Employee -> User inheritance is done
}
