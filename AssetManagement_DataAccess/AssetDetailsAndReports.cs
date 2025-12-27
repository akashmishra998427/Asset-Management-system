using AssetManagement_EntityClass;
using System.Data;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AssetManagement_DataAccess
{
    public class AssetDetailsAndReports
    {
        private readonly SQL_DB _SQL_DB;

        public AssetDetailsAndReports(SQL_DB sQl_DB)
        {
            _SQL_DB = sQl_DB;
        }

        string Query = @"";

        public async Task<DataTable> AssetUpdations()
        {
            Query = @"SELECT 
                      CPU_ASSET_CODE,INSTALLED_UNIT,USER_NAME,DEPARTMENT,Employee_Code,Asset_Type,ID AS Status 
                      FROM Asset where Asset_Type in ('C')"
            ;
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> BindAssetDetals(Dictionary<string, object> parameters = null)
        {
            Query = @"Select 
                      CPU_ASSET_CODE,Employee_Code,Asset_Type,FORMAT(Last_Patch_Update ,'yyyy-MM-dd' ) AS Last_Patch_Update,
                      FORMAT(Last_Anti_Virus ,'yyyy-MM-dd' )Last_Anti_Virus,
                      FORMAT(Last_Archive ,'yyyy-MM-dd' )Last_Archive from Asset where ID = @ID"
            ;
            return await _SQL_DB.ExecuteQuerySelect(Query, parameters);
        }

        public async Task<DataTable> GetDepartments()
        {
            Query = "Select DEPT from asset Group by DEPT ORDER BY DEPT";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> GetModel_No()
        {
            Query = @"SELECT Model_Number FROM asset GROUP BY Model_Number ORDER BY Model_Number";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> BindMoles()
        {
            Query = @"SELECT MAKE FROM asset GROUP BY MAKE ORDER BY MAKE";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> ReportingHead()
        {
            Query = @"SELECT ISNULL(Reporting_Head,'') AS Reporting_Head FROM asset  GROUP BY Reporting_Head  ORDER BY Reporting_Head";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> BindUnits()
        {
            Query = @"SELECt INSTALLED_UNIT  FROM asset  GROUP BY INSTALLED_UNIT";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> BindReports(AssetEntity Entity)
        {
            if (Entity.Fillter == "Computer")
            {
                Entity.Type = "3";
            }
            var Parameters = new Dictionary<string, object>
            {
                {"@Fillter",Entity.Fillter},
                {"@ReportType",Entity.ReportType} ,
                {"@GROUPBy" ,Entity.GROUPBy} ,
                {"@UNIT " ,Entity.UNIT} ,
                {"@Dept" ,Entity.Dept},
                {"@MAKE ",Entity.MAKE} ,
                {"@MODELNO",Entity.MODELNO},
                {"@REPORTINGHEAD",Entity.REPORTINGHEAD},
                {"Detailed",Entity.Type} ,
                {"@AssetType",Entity.AssetType}
            };
            return await _SQL_DB.ExecuteProcedureAsync("Sp_SoftwareReport", Parameters);
        }

        public async Task<DataTable> BindReportDetail(AssetEntity Entity)
        {
            if (Entity.Fillter == "Computer")
            {
                Entity.GroupCPU_V = Entity.SwName;
                Entity.SwName = "";
                Entity.Fillter = "";
                Entity.AssetDetail = "4";
            }
            if (Entity.Scrap == "1")
            {
                Entity.AssetDetail = "6";
            }
            if (Entity.ColName != "")
            {
                Entity.AssetDetail = "8";
            }
            var Parameters = new Dictionary<string, object>
            {
               {"@Fillter",Entity.Fillter},
               {"@ReportType",Entity.ReportType},
               {"@GROUPBy",Entity.GROUPBy},
               {"@UNIT",Entity.UNIT},
               {"@Dept",Entity.Dept},
               {"@MAKE",Entity.MAKE},
               {"@MODELNO",Entity.MODELNO},
               {"@REPORTINGHEAD",Entity.REPORTINGHEAD},
               {"@Detailed",Entity.AssetDetail},
               {"@SwName",Entity.SwName},
               {"@GroupUnit",Entity.GroupUnit},
               {"@GroupDept",Entity.GroupDept},
               {"@GroupUser",Entity.GroupUser},
               {"@GroupAsset",Entity.GroupAsset},
               {"@GroupCPU_V",Entity.GroupCPU_V},
               {"@AssetType",Entity.AssetType},
            };
            return await _SQL_DB.ExecuteProcedureAsync("Sp_SoftwareReport", Parameters);
        }

        public async Task<int> UpdateAssetDetal(AssetEntity Entity)
        {
            var Parameters = new Dictionary<string, object>
            {
                {"@Last_Patch_Update", Entity.Last_Patch},
                {"@Last_Anti_Virus", Entity.ILast_Anti_VirusD},
                {"@Last_Archive", Entity.Last_Archive},
                {"@ID",Entity.ID}
            };
            Query = @"UPDATE Asset SET Last_Patch_Update = @Last_Patch_Update, 
                      Last_Anti_Virus = @Last_Anti_Virus,
                      Last_Archive = @Last_Archive
                      WHERE ID = @ID"
            ;
            return await _SQL_DB.ExecuteNonQuery(Query, Parameters);
        }

        public async Task<DataTable> BindDepartment()
        {
            Query = @"select DISTINCT Dept AS[DEPT] from asset";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> BindPuchaseUnit()
        {
            Query = @"select distinct compcode, N_compcode from PREMISES";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<int> AddMaster(AssetManagerEntity Entity)
        {
            if (Entity == null)
                throw new ArgumentNullException(nameof(Entity));

            Query = @"INSERT INTO Master_Make (Asset_Type,[Name],Master_Type,Active)
                                  VALUES(@Asset_Type,@Name,@Master_Type,@Active)"
            ;

            var Param = new Dictionary<string, object>
            {
                {"@Asset_Type", Entity.Asset_Type ?? (object)DBNull.Value},
                {"@Name", Entity.Asset_Name ?? (object)DBNull.Value},
                {"@Master_Type", Entity.MAKE ?? (object)DBNull.Value},
                {"@Active", Entity.Active}
            };

            return await _SQL_DB.ExecuteNonQuery(Query, Param);
        }

        public async Task<DataTable> BuindMakeAndModel(AssetManagerEntity Entity)
        {
            Query = @"SELECT DISTINCT * FROM Master_Make WHERE Asset_Type = @AssetType";
            var parameters = new Dictionary<string, object>
            {
                { "@AssetType", Entity.Asset_Type }
            };
            return await _SQL_DB.ExecuteQuerySelect(Query, parameters);
        }

        public async Task<int> UpdateAssetAsync(AssetManagerEntity Entity)
        {
            var param = new Dictionary<string, object>
            {
                { "@AssetCode", Entity.AssetCode },
                { "@Type", Entity.RAM_Type },
                { "@Capicity", Entity.Capacity },
                { "@MType", Entity.MType },
                { "@EngineerName", Entity.U_Engineer },
                { "@ActionType", Entity.ActionType },
                { "@HddUsage", Entity.HddUsage },
                { "@Remark", Entity.Remark },
                { "@LoginName", Entity.LoginName },
                { "@ChangeDate", Entity.ChangeDate }
            };

            _SQL_DB.ExecuteProcedureAsync("Ol_UpdateAssets", param);

            string AssetIDQuery = @"SELECT TOP 1 ID FROM Asset WHERE CPU_ASSET_CODE = @AssetCode";
            DataTable asset = await _SQL_DB.ExecuteQuerySelect(AssetIDQuery, param);

            if (asset.Rows.Count == 0)
                throw new Exception("Asset not found for the provided AssetCode.");

            param.Add("@AssetID", asset.Rows[0]["ID"]);

            string Query = @"
                INSERT INTO AssetHWDtls (
                    AssetID,
                    ActionType,
                    Remarks,
                    EntryDate,
                    UserName,
                    ChangesDate,
                    MaintainType
                )
                VALUES (
                    @AssetID,
                    @ActionType,
                    @Remark,
                    @ChangeDate,
                    @LoginName,
                    @ChangeDate,
                    @MType
                )"
            ;
            return await _SQL_DB.ExecuteNonQuery(Query, param);
        }

        public async Task<DataTable> GetStorageData(AssetManagerEntity Entity)
        {
            Query = @"select 
                        ISNULL(HDD_Type,'') HDD_Type,
                        ISNULL(RAM_Type,'') RAM_Type ,
                        RAM,HDD,HddUsage from asset 
                        where CPU_ASSET_CODE =@AssetCode
            ";
            var parameters = new Dictionary<string, object>
            {
                { "@AssetCode", Entity.CPU_ASSET_CODE }
            };
            return await _SQL_DB.ExecuteQuerySelect(Query, parameters);
        }

        public async Task<DataTable> BindDetailedData(string UserName, string ScrapTransfer, string Location)
        {

            if (Location == "0")
            {
                Query = "Select  CPU_ASSET_CODE DataId,Asset_Type , case when Asset_Type ='C' then 'CPU'  ";
                Query += " when Asset_Type ='P' then 'PRINTER'  ";
                Query += " when  Asset_Type ='M'  then 'MONITOR'  ";
                Query += " when  Asset_Type ='L'  then 'LAPTOP'  ";
                Query += " Else  'OTHER'  end DataValue";
                Query += " from Asset where Employee_COde='" + UserName + "' group by Asset_Type, CPU_ASSET_CODE";
            }
            else if (Location == "2")
            {
                Query = "Select  CPU_ASSET_CODE DataId,Asset_Type , case when Asset_Type ='C' then 'CPU'  ";
                Query += " when Asset_Type ='P' then 'PRINTER'  ";
                Query += " when  Asset_Type ='M'  then 'MONITOR'  ";
                Query += " when  Asset_Type ='L'  then 'LAPTOP'  ";
                Query += " Else  'OTHER'  end DataValue";
                Query += " from Asset_transaction where Employee_COde='" + UserName + "' group by Asset_Type, CPU_ASSET_CODE";
            }
            else if (Location == "1")
            {
                Query = "Select CPU_ASSET_CODE DataId,Asset_Type , case when Asset_Type ='C' then 'CPU' ";
                Query += " when Asset_Type ='P' then 'PRINTER'";
                Query += " when  Asset_Type ='M'  then 'MONITOR'";
                Query += " when  Asset_Type ='L'  then 'LAPTOP'";
                Query += " Else  'OTHER' end DataValue";
                Query += " from Asset_Scrap where Employee_COde='" + UserName + "' group by Asset_Type, CPU_ASSET_CODE";
            }
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<int> TransferAsset(AssetManagerEntity Entity)
        {
            var Param = new Dictionary<string, object>
            {
                {"@AssetCode",Entity.AssetCode},
                {"@FromUser",Entity.FromUser},
                {"@TargetUser",Entity.TargetUser},
                {"@Premises",Entity.Premises},
                {"@LoginId",Entity.Login_Id},
                {"@Email_ID",Entity.Email_ID},
                {"@GIMS_ID",Entity.GIMS_ID},
                {"@VFM_ID",Entity.VFM_ID},
                {"@Pads_ID",Entity.PADS_ID},
                {"@ReportingTo",Entity.Reporting_TO},
                {"@MobileNO",Entity.MobileNo},
                {"@Department",Entity.DEPARTMENT},
                {"@Designation",Entity.Designation},
                {"@EngineerName",Entity.EngineerName},
                {"@IPAddress",Entity.Ip_No},
                {"@ToUserName",Entity.ToUserName},
                {"@Install_Date",Entity.Install_Date},
                {"@ScrapTransfer",Entity.ScrapTransfer},
                {"@ScrapRemark",Entity.ScrapRemark},
                {"@ContactExtNo",Entity.ContactExtNo}
            };
            return await _SQL_DB.ExecuteProcedureNonQueryAsync("Ol_TransferAssets", Param);
        }

        public async Task<DataTable> CheckAssetPresent(string AssetCode)
        {
            Query = @$"select * from asset where CPU_ASSET_CODE = '{AssetCode}'";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<bool> UploadLicenceBulk(DataTable dt)
        {
            string ChkQuery = "SELECT ISNULL(MAX(SrNo), 0) FROM SoftwareLicence";
            int maxSrNo = await _SQL_DB.ExecuteScalar(ChkQuery);

            if (!dt.Columns.Contains("SrNo"))
            {
                dt.Columns.Add("SrNo", typeof(int));
            }

            int counter = maxSrNo;
            foreach (DataRow row in dt.Rows)
            {
                counter++;
                row["SrNo"] = counter;
            }

            string[] colName = {"SrNo","Make","SoftwareType","OfficeVer","Qty","LicenceType",
                                    "PurchaseDate","ValidUpto","PurchaseType","Invoice_No",
                                    "Invoice_Date","Company","Unit","Rate" }
            ;

            return await _SQL_DB.BulkInsertAsync(dt, "SoftwareLicence", colName);
        }

        public async Task<bool> RegisterLicense(LicenseEntity Entity)
        {
            string ChkQuery = "SELECT ISNULL(MAX(SrNo), 0) FROM SoftwareLicence";
            int SrNo = await _SQL_DB.ExecuteScalar(ChkQuery);
            SrNo += 1;

            string Query = @"
                INSERT INTO SoftwareLicence( 
                    SrNo, Make, SoftwareType, OfficeVer, Qty, LicenceType,
                    PurchaseDate, ValidUpto, PurchaseType, Invoice_No,
                    Invoice_Date, Company, Unit, Rate, PoNo, PoDate,AttachFileName
                ) VALUES (
                    @SrNo, @Make, @Software_Type, @Version, @Quantity, @License_Type,
                    @PurchaseDate, @ValidUpTo, @PurchaseType, @InviceNo,
                    @InvoiceDate, @Company, @Unit, @Price, @PoNo, @PoDate,@AttachFileName
                )
            ";

            var Params = new Dictionary<string, object>
            {
                {"@SrNo", SrNo},
                {"@Make", Entity.Make},
                {"@Software_Type", Entity.Software_Type},
                {"@Version", Entity.Version},
                {"@Quantity", Entity.Quantity},
                {"@License_Type", Entity.License_Type},
                {"@PurchaseDate", Entity.PurchaseDate},
                {"@ValidUpTo", Entity.ValidUpTo},
                {"@PurchaseType", Entity.PurchaseType},
                {"@InviceNo", Entity.InviceNo},
                {"@InvoiceDate", Entity.InvoiceDate},
                {"@Company", Entity.CompneyName},
                {"@Unit", Entity.PurchaseUnit},
                {"@Price", Entity.Price},
                {"@PoNo", Entity.PoNo},
                {"@PoDate", Entity.PoDate},
                {"@AttachFileName", Entity.InvoiceFileName}
            };

            int result = await _SQL_DB.ExecuteNonQuery(Query, Params);
            return result > 0;
        }

    }
}

