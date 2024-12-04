namespace HitsBackend.Domain.Entities;

public class AsHouse
{
    public required long id { get; set; }
    public required long objectid { get; set; }
    public required Guid objectguid { get; set; }
    public required long changeid { get; set; }
    public string? housenum { get; set; }
    public string? addnum1 { get; set; }
    public string? addnum2 { get; set; }
    public int? housetype { get; set; }
    public int? addtype1 { get; set; }
    public int? addtype2 { get; set; }
    public required int opertypeid { get; set; }
    public long? previd { get; set; }
    public long? nextid { get; set; }
    public required DateTime updatedate { get; set; }
    public required DateTime startdate { get; set; }
    public required DateTime enddate { get; set; }
    public required int isactual { get; set; }
    public required int isactive { get; set; }
}