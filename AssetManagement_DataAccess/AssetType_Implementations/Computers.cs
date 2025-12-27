using System.Data;
using AssetManagement_DataAccess.Interfaces;
using AssetManagement_EntityClass;

namespace AssetManagement_DataAccess.Implementations
{
    public class Computers : IAssetManager
    {
        private readonly SQL_DB _SQL_DB;

        public Computers(SQL_DB sQL_DB, AssetManagerEntity entity)
        {
            _SQL_DB = sQL_DB;
        }

        public async Task<int> AddAsset(AssetManagerEntity entity)
        {
            try
            {
                string checkQuery = $"SELECT COUNT(CPU_ASSET_CODE) FROM asset WHERE CPU_ASSET_CODE = '{entity.CPU_ASSET_CODE}'";
                var existingCount = await _SQL_DB.ExecuteScalar(checkQuery);
                if (Convert.ToInt32(existingCount) > 0)
                {
                    return -1;
                }

                string Query = "BEGIN TRANSACTION;";

                Query += @$"
                    INSERT INTO asset (  
                        CPU_ASSET_CODE, DEPT, MAKE, Model_Number,  
                        PURCHASE_UNIT, PURCHASE_DATE_YEAR, Bill_No, Bill_Date,  
                        vendor, Ip_No, RemoteAllowed, [Host_Name], PROCESSOR,  
                        RAM, HDD, Asset_Type, [Group], Screen_Size, INSTALLED_UNIT, HDD_Type, RAM_Type,  
                        WARRANTY_AMC, USER_NAME, Employee_Code, Monitor_Make, Monitor_Asset_Code
                    )  

                    VALUES (  
                        '{entity.CPU_ASSET_CODE}', '{entity.DEPT}', '{entity.MAKE}', '{entity.Model_Number}',  
                        '{entity.PURCHASE_UNIT}', '{entity.PURCHASE_DATE_YEAR}', '{entity.Bill_No}', '{entity.Bill_Date}',  
                        '{entity.vendor}', '{entity.Ip_No}', '{entity.RemoteAllowed}', '{entity.Host_Name}', '{entity.PROCESSOR}',  
                        '{entity.RAM}', '{entity.HDD}', '{entity.Asset_Type}', 'RICHA', '{entity.Screen_Size}', '{entity.INSTALLED_UNIT}',  
                        '{entity.HDD_Type}', '{entity.RAM_Type}', '{entity.WARRANTY_AMC}', '{entity.USER_NAME}', '{entity.Employee_Code}',  
                        {(entity.With_Monitor ? $"'{entity.Monitor_Make}'" : "NULL")},  
                        {(entity.With_Monitor ? $"'{entity.Monitor_Asset_Code}'" : "NULL")}  
                    );
                ";

                Query += @$"
                    INSERT INTO tbl_AddationalAsset_Info 
                        (AssetCode, Price, UsedBy, WarrantyValidity, Warranty_Months)  
                    VALUES 
                        ('{entity.CPU_ASSET_CODE}', '{entity.Price}', '{entity.UsedBy}', '{entity.WarrantyValidity}', '{entity.MONTH}');
                ";

                if (entity.With_Monitor)
                {
                    Query += @$"
                        INSERT INTO asset (  
                            CPU_ASSET_CODE, DEPT, MAKE, Model_Number,  
                            PURCHASE_UNIT, PURCHASE_DATE_YEAR, Bill_No, Bill_Date,  
                            vendor, Asset_Type, [Group], Screen_Size, INSTALLED_UNIT,  
                            WARRANTY_AMC, USER_NAME, Employee_Code,Monitor_Asset_Code
                        )

                        VALUES (  
                            '{entity.Monitor_Asset_Code}', '{entity.DEPT}', '{entity.Monitor_Make}', '{entity.Model_Number}',  
                            '{entity.PURCHASE_UNIT}', '{entity.PURCHASE_DATE_YEAR}', '{entity.Bill_No}', '{entity.Bill_Date}',  
                            '{entity.vendor}', 'M', 'RICHA', '{entity.Screen_Size}', '{entity.INSTALLED_UNIT}',  
                            '{entity.WARRANTY_AMC}', '{entity.USER_NAME}', '{entity.Employee_Code}','{entity.Monitor_Asset_Code}'  
                        );
                    ";

                    Query += @$"
                        INSERT INTO tbl_AddationalAsset_Info (
                            AssetCode, Price, UsedBy, WarrantyValidity, Warranty_Months,  
                            Screen_Resolution, Panel_Type
                        )  
                        VALUES (  
                            '{entity.Monitor_Asset_Code}', '{entity.Price}', '{entity.UsedBy}', '{entity.WarrantyValidity}', '{entity.MONTH}',  
                            '{entity.Resolution}', '{entity.Panel_Type}'
                        );
                    ";
                }
                Query += "COMMIT;";
                return await _SQL_DB.ExecuteNonQuery(Query);
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message);
                return 0;
            }
        }

        public Task<DataTable> GetAssetDetails(AssetManagerEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<DataTable> GetDetailsByID(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAssetDetails(AssetManagerEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
