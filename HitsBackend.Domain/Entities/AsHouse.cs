namespace HitsBackend.Domain.Entities;

public class AsHouse
{
    public required long Id { get; set; }
    public required long ObjectId { get; set; }
    public required Guid ObjectGuid { get; set; }
    public required long ChangeId { get; set; }
    public string? HouseNum { get; set; }
    public string? AddNum1 { get; set; }
    public string? AddNum2 { get; set; }
    public int? HouseType { get; set; }
    public int? AddType1 { get; set; }
    public int? AddType2 { get; set; }
    public required int OperTypeId { get; set; }
    public long? PrevId { get; set; }
    public long? NextId { get; set; }
    public required DateTime UpdateDate { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public required int IsActual { get; set; }
    public required int IsActive { get; set; }
}