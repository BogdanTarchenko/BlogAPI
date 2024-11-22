namespace HitsBackend.Domain.Entities;

public class AsAddrObj
{
    public required long Id { get; set; }
    public required long ObjectId { get; set; }
    public required Guid ObjectGuid { get; set; }
    public required long ChangeId { get; set; }
    public required string Name { get; set; }
    public required string TypeName { get; set; }
    public required string Level { get; set; }
    public required int OperTypeId { get; set; }
    public long? PrevId { get; set; }
    public long? NextId { get; set; }
    public required DateTime UpdateDate { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public required int IsActual { get; set; }
    public required int IsActive { get; set; }
}