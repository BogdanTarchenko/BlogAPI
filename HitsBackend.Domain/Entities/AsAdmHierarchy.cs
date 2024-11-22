namespace HitsBackend.Domain.Entities;

public class AsAdmHierarchy
{
    public required long Id { get; set; }
    public required long ObjectId { get; set; }
    public long? ParentObjectId { get; set; }
    public required long ChangeId { get; set; }
    public string? RegionCode { get; set; }
    public string? AreaCode { get; set; }
    public string? CityCode { get; set; }
    public string? PlaceCode { get; set; }
    public string? PlanCode { get; set; }
    public string? StreetCode { get; set; }
    public long? PrevId { get; set; }
    public long? NextId { get; set; }
    public required DateTime UpdateDate { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public required int IsActive { get; set; }
    public string? Path { get; set; }
}