namespace HitsBackend.Domain.Entities;

public class AsAdmHierarchy
{
    public required long id { get; set; }
    public required long objectid { get; set; }
    public long? parentobjid { get; set; }
    public required long changeid { get; set; }
    public string? regioncode { get; set; }
    public string? areacode { get; set; }
    public string? citycode { get; set; }
    public string? placecode { get; set; }
    public string? plancode { get; set; }
    public string? streetcode { get; set; }
    public long? previd { get; set; }
    public long? nextid { get; set; }
    public required DateTime updatedate { get; set; }
    public required DateTime startdate { get; set; }
    public required DateTime enddate { get; set; }
    public required int isactive { get; set; }
    public string? path { get; set; }
}