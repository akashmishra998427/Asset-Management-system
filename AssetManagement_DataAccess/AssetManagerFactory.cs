using AssetManagement_DataAccess;
//using AssetManagement_DataAccess.AssetType_Implementations;
using AssetManagement_DataAccess.Implementations;
using AssetManagement_DataAccess.Interfaces;
using AssetManagement_EntityClass;

public class AssetManagerFactory
{
    private readonly SQL_DB _SQL_DB;

    public AssetManagerFactory(SQL_DB sQL_DB)
    {
        _SQL_DB = sQL_DB;
    }

    public IAssetManager GetManager(AssetManagerEntity entity)
    {
        return entity.Asset_Name switch
        {
            "Computer" => new Computers(_SQL_DB, entity),
            //"Laptop" => new Laptop(_SQL_DB, entity),
            //"Monitor" => new Monitors(_SQL_DB, entity),
            //"Tab" => new Tabs(_SQL_DB, entity),
            //"TV" => new Tv(_SQL_DB, entity),
            //"Printer (Rented)" => new RentedPrinter(_SQL_DB, entity),
            //"Printer" => new Printer(_SQL_DB, entity),
            //"Switch" => new Switchs(_SQL_DB, entity),
            //"Firewall" => new Firewall(_SQL_DB, entity),
            //"Face Machine" => new Face_Machine(_SQL_DB, entity),
            //"Chair" => new Chair(_SQL_DB, entity),
            //"Vehicle" => new Vehicle(_SQL_DB, entity),
            //"AC" => new AC(_SQL_DB, entity),
            //"RAM" => new RAM(_SQL_DB, entity),
            //"HDD" => new HDD(_SQL_DB, entity),
            _ => throw new NotImplementedException($"Asset type '{entity.Asset_Name}' is not implemented.")
        };
    }
}
