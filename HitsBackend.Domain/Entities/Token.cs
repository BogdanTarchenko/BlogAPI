namespace HitsBackend.Domain.Entities;

public class Token
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreateTime { get; set; }
}