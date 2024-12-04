namespace HitsBackend.Domain.Entities;

public class AsAddrObj
{
    public required long id { get; set; }
    public required long objectid { get; set; }
    public required Guid objectguid { get; set; }
    public required long changeid { get; set; }
    public required string name { get; set; }
    public required string typename { get; set; }
    public required string level { get; set; }
    public required int opertypeid { get; set; }
    public long? previd { get; set; }
    public long? nextid { get; set; }
    public required DateTime updatedate { get; set; }
    public required DateTime startdate { get; set; }
    public required DateTime enddate { get; set; }
    public required int isactual { get; set; }
    public required int isactive { get; set; }
}