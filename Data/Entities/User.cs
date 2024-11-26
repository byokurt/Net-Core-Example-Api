namespace NetCoreExampleApi.Data.Entities;

public class User
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Surename { get; set; }

    public bool IsDeleted { get; set; }
}