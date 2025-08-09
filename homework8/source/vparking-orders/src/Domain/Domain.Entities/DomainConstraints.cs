namespace Domain.Entities;

public static class DomainConstraints
{
    public static int OrderDataMaxLength => 10*1024;
    public static int ClientIDMaxLength => 500;
    public static int EmailMaxLength => 1000;
}