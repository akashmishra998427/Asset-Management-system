using AssetManagement_DataAccess;
using AssetManagement_DataAccess.Operations.Implementations;
using AssetManagement_DataAccess.Operations.Interfaces;
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
        return new AssetRegristration(_SQL_DB, entity);
    }

}
