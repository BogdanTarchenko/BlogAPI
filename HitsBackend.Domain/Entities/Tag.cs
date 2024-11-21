namespace HitsBackend.Domain.Entities;

public class Tag
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreateTime { get; set; }
}