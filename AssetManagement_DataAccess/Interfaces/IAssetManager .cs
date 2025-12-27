using System.Data;
using AssetManagement_EntityClass;

namespace AssetManagement_DataAccess.Interfaces
{
    public interface IAssetManager
    {
        Task<int> AddAsset(AssetManagerEntity Entity);

        Task<int> UpdateAssetDetails(AssetManagerEntity Entity);

        Task<DataTable> GetAssetDetails(AssetManagerEntity Entity);

        Task<DataTable> GetDetailsByID(int id);

    }
}
