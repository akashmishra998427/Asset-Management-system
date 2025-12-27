using AssetManagement_EntityClass;
using System.Data;

namespace AssetManagement_DataAccess.Reports
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
                    AssetID, ActionType, Remarks,  EntryDate, UserName, ChangesDate, MaintainType
                )
                VALUES (
                    @AssetID, @ActionType, @Remark, @ChangeDate, @LoginName, @ChangeDate, @MType
                )"
            ;
            return await _SQL_DB.ExecuteNonQuery(Query, param);
        }

        public async Task<DataTable> GetStorageData(AssetManagerEntity Entity)
        {
            Query = @"select 
                         ISNULL(HDD_Type,'') HDD_Type, ISNULL(RAM_Type,'') RAM_Type, RAM,HDD,HddUsage from asset 
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

            string Query = $@"
                INSERT INTO SoftwareLicence( 
                    SrNo, Make, SoftwareType, OfficeVer, Qty, LicenceType, PurchaseDate, ValidUpto, PurchaseType, Invoice_No, Invoice_Date, Company, Unit,
                    Rate, PoNo, PoDate,AttachFileName
                ) VALUES (
                    '{SrNo}', '{Entity.Make}', '{Entity.Software_Type}', '{Entity.SoftwareCategory}', '{Entity.Quantity}', '{Entity.License_Type}', 
                    '{Entity.PurchaseDate}', '{Entity.ValidUpTo}', '{Entity.PurchaseType}', '{Entity.InviceNo}', '{Entity.InvoiceDate}', '{Entity.CompneyName}',
                    '{Entity.PurchaseUnit}', '{Entity.Price}', '{Entity.PoNo}', '{Entity.PoDate}','{Entity.InvoiceFileName}'
                )
            ";
            int result = await _SQL_DB.ExecuteNonQuery(Query);
            return result > 0;
        }

        public async Task<DataTable> AssetTracDetails(string AssetCode)
        {
            Query = $@"select * from AssetHWDtls where AssetID in (select id from Asset where CPU_ASSET_CODE ='{AssetCode}') Order by ID";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> GetAssetCodeDetails(string AssetType)
        {
            Query = $@"SELECT 
                    MAX(TRY_CAST(
                        SUBSTRING(
                            CPU_ASSET_CODE,
                            CHARINDEX('/', CPU_ASSET_CODE, CHARINDEX('/', CPU_ASSET_CODE) + 1) + 2,
                            LEN(CPU_ASSET_CODE)
                        ) AS INT)
                    ) AS max_number
                FROM asset
                WHERE Asset_Type = '{AssetType}';
            ";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> GetAssetType()
        {
            Query = "SELECT DISTINCT CallType , Alias FROM CallType";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> BindDetailsData(string AssetType)
        {
            Query = $@"SELECT 
                [Group],Asset_Type,PURCHASE_UNIT,PURCHASE_DATE_YEAR,CPU_V,
                Bill_No,Bill_Date,INSTALLED_UNIT,CPU_ASSET_CODE,[USER_NAME],
                DEPT,MAKE,Model_Number,Machine_Sl_No,Employee_Code,WARRANTY_AMC,
                CompCode,Reporting_Head,[Status],MobileNo,Install_Date,LoginName,
                RemoteAllowed,Invoice_Tag

                FROM asset

                WHERE Asset_Type = '{AssetType}'"
            ;
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> RetriveDataFromAssetCode(string AssetCode)
        {
            Query = $@" SELECT * FROM asset WHERE CPU_ASSET_CODE = '{AssetCode}'";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> BindSoftwareCategory(string SoftwareType)
        {
            Query = $@" SELECT SwName FROM Software_Name WHERE [Type] = '{SoftwareType}'";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> BindSuggestions(string Type)
        {
            switch (Type)
            {
                case "CPU":
                    Query = @"SELECT DISTINCT CPU_V AS DataValue FROM asset 
                      WHERE CPU_V <> ''";
                    break;

                case "MAKE":
                    Query = @"SELECT DISTINCT MAKE AS DataValue FROM asset 
                      WHERE MAKE <> ''";
                    break;

                case "Licence":
                    Query = @"SELECT DISTINCT Windows_License_Type_Paper_OEM AS DataValue 
                      FROM asset WHERE Windows_License_Type_Paper_OEM <> ''";
                    break;

                case "SoftwareInstall":
                    Query = @"SELECT DISTINCT Windows_Version_Installed AS DataValue 
                      FROM asset WHERE Windows_Version_Installed <> ''";
                    break;

                case "Software":
                    Query = @"SELECT DISTINCT Windows_Version1 AS DataValue 
                      FROM asset WHERE Windows_Version1 <> ''";
                    break;

                case "ScreenSize":
                    Query = @"SELECT DISTINCT Screen_Size AS DataValue 
                      FROM asset WHERE Screen_Size <> ''";
                    break;

                case "LapTopServer":
                    Query = @"SELECT DISTINCT Desktop_Laptop_Server AS DataValue 
                      FROM asset WHERE Desktop_Laptop_Server <> ''";
                    break;

                case "Model_Number":
                    Query = @"SELECT DISTINCT Model_Number AS DataValue 
                      FROM asset WHERE Model_Number <> ''";
                    break;

                case "Bill_No":
                    Query = @"SELECT TOP 3 Bill_No AS DataValue 
                      FROM asset WHERE Bill_No <> '' ORDER BY ID DESC";
                    break;

                case "PROCESSOR":
                    Query = @"SELECT DISTINCT PROCESSOR AS DataValue 
                      FROM asset WHERE PROCESSOR <> ''";
                    break;

                case "RAM":
                    Query = @"SELECT DISTINCT RAM AS DataValue 
                      FROM asset WHERE RAM <> ''";
                    break;

                case "HDD":
                    Query = @"SELECT DISTINCT HDD AS DataValue 
                      FROM asset WHERE HDD <> ''";
                    break;

                case "ADAPTER_SR_NO":
                    Query = @"SELECT DISTINCT ADAPTER_SR_NO AS DataValue 
                      FROM asset WHERE ADAPTER_SR_NO <> ''";
                    break;

                case "Vendor":
                    Query = @"SELECT TOP 3 vendor AS DataValue 
                      FROM asset WHERE vendor <> '' ORDER BY ID DESC";
                    break;

                default:
                    return new DataTable();
            }

            return await _SQL_DB.ExecuteQuerySelect(Query);
        }
    }
}

