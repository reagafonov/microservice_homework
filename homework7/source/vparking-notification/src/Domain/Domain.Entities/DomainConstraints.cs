namespace Domain.Entities;

public static class DomainConstraints
{
    public static int ContentLength => 10*1024;
    public static int ClientIDLength => 100;
}