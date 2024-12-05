using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Models;
using HitsBackend.Domain.Entities;
using HitsBackend.Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly ApplicationDbContext _context;
    private Dictionary<long, List<SearchAddressModel>> _addressCache;

    public AddressRepository(ApplicationDbContext context)
    {
        _context = context;
        _addressCache = new Dictionary<long, List<SearchAddressModel>>();
    }

    public async Task<List<SearchAddressModel>> SearchAddressesAsync(long? parentObjectId, string query)
    {
        var lowerQuery = query?.ToLower();
        
        var defaultResult = new List<SearchAddressModel>
        {
            new SearchAddressModel(
                1281271,
                Guid.Parse("889b1f3a-98aa-40fc-9d3d-0f41192758ab"),
                "обл Томская",
                GarAddressLevel.Region,
                "Субъект РФ"
            )
        };
        
        if (!parentObjectId.HasValue)
        {
            return string.IsNullOrEmpty(lowerQuery)
                ? defaultResult
                : defaultResult.Where(d => d.Text!.ToLower().Contains(lowerQuery)).ToList();
        }

        if (!_addressCache.ContainsKey(parentObjectId.Value))
        {
            var hierarchyEntries = await _context.AsAdmHierarchy
                .Where(h => h.parentobjid == parentObjectId.Value)
                .ToListAsync();

            var addresses = new List<SearchAddressModel>();

            foreach (var entry in hierarchyEntries)
            {
                var addrObjects = await _context.AsAddrObjs
                    .Where(a => a.objectid == entry.objectid)
                    .ToListAsync();

                addresses.AddRange(addrObjects.Select(a => new SearchAddressModel(
                    a.objectid,
                    a.objectguid,
                    $"{a.typename} {a.name}",
                    Enum.TryParse<GarAddressLevel>(a.level, out var level) ? level - 1 : GarAddressLevel.Region,
                    GetLevelText(level - 1))));
                
                var houseObjects = await _context.AsHouses
                    .Where(h => h.objectid == entry.objectid)
                    .ToListAsync();

                addresses.AddRange(houseObjects.Select(h => new SearchAddressModel(
                    h.objectid,
                    h.objectguid,
                    BuildHouseText(h),
                    GarAddressLevel.Building,
                    "Здание (сооружение)")));
            }

            _addressCache[parentObjectId.Value] = addresses;
        }

        var addressesFromCache = _addressCache[parentObjectId.Value];

        if (!string.IsNullOrEmpty(lowerQuery))
        {
            addressesFromCache = addressesFromCache.Where(a => a.Text.ToLower().Contains(lowerQuery)).ToList();
        }

        return addressesFromCache.OrderBy(a => a.Text).ToList();
    }

    public async Task<List<SearchAddressModel>> GetChainAsync(Guid objectGuid)
    {
        var addrObj = await _context.AsAddrObjs
            .FirstOrDefaultAsync(a => a.objectguid == objectGuid);

        if (addrObj == null)
        {
            var houseObj = await _context.AsHouses
                .FirstOrDefaultAsync(h => h.objectguid == objectGuid);

            if (houseObj == null)
            {
                return new List<SearchAddressModel>();
            }

            var hierarchy = await _context.AsAdmHierarchy
                .FirstOrDefaultAsync(h => h.objectid == houseObj.objectid);

            if (hierarchy == null)
            {
                return new List<SearchAddressModel>();
            }

            return await BuildChain(hierarchy.path);
        }

        var hierarchyFromAddrObj = await _context.AsAdmHierarchy
            .FirstOrDefaultAsync(h => h.objectid == addrObj.objectid);

        if (hierarchyFromAddrObj == null)
        {
            return new List<SearchAddressModel>();
        }

        return await BuildChain(hierarchyFromAddrObj.path);
    }

    private async Task<List<SearchAddressModel>> BuildChain(string path)
    {
        var chain = new List<SearchAddressModel>();
        var pathItems = path.Split('.');

        foreach (var item in pathItems)
        {
            if (long.TryParse(item, out var objectId))
            {
                var address = await _context.AsAddrObjs
                    .FirstOrDefaultAsync(a => a.objectid == objectId);

                if (address != null)
                {
                    if (int.TryParse(address.level, out var levelValue))
                    {
                        var level = (GarAddressLevel)(levelValue - 1);
                        var text = $"{address.typename} {address.name}";
                        var levelText = GetLevelText(level);

                        chain.Add(new SearchAddressModel(
                            address.objectid,
                            address.objectguid,
                            text,
                            level,
                            levelText));
                    }
                }
                else
                {
                    var house = await _context.AsHouses
                        .FirstOrDefaultAsync(h => h.objectid == objectId);

                    if (house != null)
                    {
                        var text = BuildHouseText(house);
                        chain.Add(new SearchAddressModel(
                            house.objectid,
                            house.objectguid,
                            text,
                            GarAddressLevel.Building,
                            "Здание (сооружение)"));
                    }
                }
            }
        }

        return chain;
    }

    private string BuildHouseText(AsHouse house)
    {
        var addType1Text = house.addtype1.HasValue ? GetAddType1Text(house.addtype1.Value) : string.Empty;
        var addType2Text = house.addtype2.HasValue ? GetAddType2Text(house.addtype2.Value) : string.Empty;

        return $"{house.housenum}{addType1Text}{house.addnum1}{addType2Text}{house.addnum2}".Trim();
    }

    private string GetAddType1Text(int addType1)
    {
        return addType1 switch
        {
            1 => " к. ",
            2 => " стр. ",
            3 => " соор. ",
            4 => " ",
            _ => string.Empty
        };
    }

    private string GetAddType2Text(int addType2)
    {
        return addType2 switch
        {
            2 => " стр. ",
            3 => " соор. ",
            _ => string.Empty
        };
    }

    private string GetLevelText(GarAddressLevel level)
    {
        return level switch
        {
            GarAddressLevel.Region => "Субъект РФ",
            GarAddressLevel.AdministrativeArea => "Административный район",
            GarAddressLevel.MunicipalArea => "Муниципальный район",
            GarAddressLevel.RuralUrbanSettlement => "Сельское/городское поселение",
            GarAddressLevel.City => "Город",
            GarAddressLevel.Locality => "Населенный пункт",
            GarAddressLevel.ElementOfPlanningStructure => "Элемент планировочной структуры",
            GarAddressLevel.ElementOfRoadNetwork => "Элемент улично-дорожной сети",
            GarAddressLevel.Land => "Земельный участок",
            GarAddressLevel.Building => "Здание (сооружение)",
            GarAddressLevel.Room => "Помещение",
            GarAddressLevel.RoomInRooms => "Помещения в пределах помещения",
            GarAddressLevel.AutonomousRegionLevel => "Уровень автономного округа (устаревшее)",
            GarAddressLevel.IntracityLevel => "Уровень внутригородской территории (устаревшее)",
            GarAddressLevel.AdditionalTerritoriesLevel => "Уровень дополнительных территорий (устаревшее)",
            GarAddressLevel.LevelOfObjectsInAdditionalTerritories => "Уровень объектов на дополнительных территориях (устаревшее)",
            GarAddressLevel.CarPlace => "Машиноместо",
            _ => "Неизвестный уровень"
        };
    }
}