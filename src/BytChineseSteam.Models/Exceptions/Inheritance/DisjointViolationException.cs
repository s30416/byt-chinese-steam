namespace BytChineseSteam.Models.Exceptions.Inheritance;

public class DisjointViolationException() : Exception("This operation violates the disjoint restriction for this inheritance.")
{ }