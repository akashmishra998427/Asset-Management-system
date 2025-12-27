using AssetManagement_EntityClass;
using System.Data;

namespace AssetManagement_DataAccess.Operations.Interfaces
{
    public interface IAssetManager
    {
        Task<int> AddAsset(AssetManagerEntity Entity);

        Task<int> UpdateAssetDetails(AssetManagerEntity Entity);

        Task<DataTable> GetAssetDetails(AssetManagerEntity Entity);

        Task<DataTable> GetDetailsByID(int id);

    }
}
