using System.Data;
using AssetManagement_EntityClass;

namespace AssetManagement_DataAccess
{
    public class ProductionMachine
    {
        private readonly SQL_DB _SQL_DB;
        public ProductionMachine(SQL_DB db)
        {
            _SQL_DB = db;
        }

        public string Query = string.Empty;

        public async Task<DataTable> BindModelNo(ProductionMachineEntity Entity)
        {
            string Query = @"select ModelNo from ol_MachineDetails where CompCode = @CompCode AND ModelNo <> ''  
                             AND (@LineID IS NULL OR @LineID = '' OR LineID = @LineID)   GROUP BY ModelNo ORDER BY ModelNo"
            ;
            var parameters = new Dictionary<string, object>
            {
                { "@CompCode", Entity.CompCode },
                { "@LineID", Entity.LineID }
            };
            return await _SQL_DB.ExecuteQuerySelect(Query, parameters);
        }

        public async Task<DataTable> GetMachineList(ProductionMachineEntity Entity)
        {

            if (Entity.MachineSrNo == null || Entity.MachineSrNo == "")
            {
                Query = "Select ID,MachineCode,ModelNo,MachineSrNo from ol_MachineDetails where CompCode ='" + Entity.CompCode + "'" +
                      (Entity.LineID == "" || Entity.LineID == null ? "" : "and LineiD = " + Entity.LineID) + " " +
                     (Entity.ModelNo == "" || Entity.ModelNo == null ? "" : " and ModelNo ='" + Entity.ModelNo + "' ") +
                    " Order by MachineCode ";
            }
            else
            {
                Query = @"Select ID,MachineCode,ModelNo,MachineSrNo 
                            from ol_MachineDetails where MachineSrNo ='" + Entity.MachineSrNo + "'"
                ;
            }

            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> GetMachineDataForBarcode(ProductionMachineEntity Entity)
        {
            //if (Entity.MachineSrNo == null || Entity.MachineSrNo == "")
            //{
            Query = @" Select M.*,D.Barcode from ol_MachineDetails M 
                            Left join ol_MachineDetail D on M.MachineCode = D.MachineCode 
                            where M.id in (Select item from dbo.split('" + string.Join(",", Entity.MachineID) + "',',')) "
            ;
            //}
            //else
            //{
            //    Query = @"select MachineCode from ol_MachineDetails  where MachineSrNo ='" + Entity.MachineSrNo + "'";
            //}
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> GetPremises()
        {
            Query = "Select * from PREMISES where Work_Prod = 'Y'";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> GetCompCode()
        {
            Query = "select Distinct CompCode,N_CompCode from PREMISES  ";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> GetMachineMaster(ProductionMachineEntity Entity)
        {
            Query = @"IF @Data = 'GetMachineData'                
                        BEGIN           
                            SELECT M.*, L.[lineNo], P.N_CompCode, COUNT(*) OVER () AS TotalCount 
                            FROM ol_MachineDetails M               
                            LEFT JOIN Premises P ON P.CompCode = M.CompCode              
                            LEFT JOIN [lineNo] L ON L.id = M.LineId               
                            ORDER BY MachineCode              
                        END                
                        ELSE IF @Data = 'GetPremisesFilterdata'              
                        BEGIN           
                            SELECT M.*, L.[lineNo], P.N_CompCode, COUNT(m.id) OVER () AS TotalCount 
                            FROM ol_MachineDetails M               
                            LEFT JOIN Premises P ON P.CompCode = M.CompCode              
                            LEFT JOIN [lineNo] L ON L.id = M.LineId 
                            WHERE M.CompCode = @Premises              
                            ORDER BY M.CompCode              
                        END                
                        ELSE                
                        BEGIN           
                            SELECT Machine_Type, ModelNo, Make, MachineSrNo, MachineCode, id, CompCode, LineId, Active 
                            FROM ol_MachineDetails 
                            WHERE ID = @Data                
                        END 
                        "
            ;
            var Parameters = new Dictionary<string, object>
            {
                {"@Data", Entity.Data},
                {"@Premises", Entity.Premises},
            };
            return await _SQL_DB.ExecuteQuerySelect(Query, Parameters);
        }
        public async Task<DataTable> downloadExcelData(ProductionMachineEntity Entity)
        {
            var Parameters = new Dictionary<string, object>
            {
                {"@Action","DownloadExcelData"},
                {"@Premises",Entity.CompCode}
            };
            return await _SQL_DB.ExecuteProcedureAsync("Ol_Proc_MachineData", Parameters);
        }

        public async Task<DataTable> GetCode()
        {
            var Params = new Dictionary<string, object>
            {
                {"@Action","GetCode" }
            };
            return await _SQL_DB.ExecuteProcedureAsync("Ol_Proc_MachineData", Params);
        }

        public async Task<DataTable> SetMachineData(ProductionMachineEntity Entity)
        {
            var Params = new Dictionary<string, object>
            {
                {"@Action","SetMachineData"},
                {"@Data",Entity.Data}
            };
            return await _SQL_DB.ExecuteProcedureAsync("Ol_Proc_MachineData", Params);
        }

        public async Task<DataTable> GetAutoCompleteData(ProductionMachineEntity Entity)
        {
            var Params = new Dictionary<string, object>
            {
                {"@Action",Entity.Action},
                {"@Value",Entity.Value}
            };
            return await _SQL_DB.ExecuteProcedureAsync("Ol_Proc_MachineData", Params);
        }

        public async Task<DataTable> GetLineNo(ProductionMachineEntity Entity)
        {
            Query = @"SELECT id, [lineNo], StitchToProduction 
               FROM [lineNo] 
               WHERE Process = @department 
                 AND compcode = @compcode 
                 AND ISNULL(LineClose, 0) = 0"
            ;
            if (!string.IsNullOrEmpty(Entity.Floor))
            {
                Query += " AND Floor = @Floor";
            }
            else if (!string.IsNullOrEmpty(Entity.Tv_Floor))
            {
                Query += " AND Tv_Floor = @Tv_Floor";
            }
            Query += " ORDER BY sno";
            var Params = new Dictionary<string, object>
            {
                {"@compcode",Entity.CompCode},
                {"@department",Entity.Department},
                {"@Floor",Entity.Floor},
                {"@Tv_Floor",Entity.Tv_Floor }
            };
            return await _SQL_DB.ExecuteQuerySelect(Query, Params);
        }

        public async Task<string> ManageMachineData(ProductionMachineEntity entity)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@Action", "ManageMachineData"},
                    {"@MachineType", entity.MachineType ?? string.Empty},
                    {"@ModelNo", entity.ModelNo ?? string.Empty},
                    {"@Make", entity.Make ?? string.Empty},
                    {"@MachineCode", entity.MachineCode ?? string.Empty},
                    {"@MachineSrNo", entity.MachineSrNo ?? string.Empty},
                    {"@CompCode", entity.CompCode ?? string.Empty},
                    {"@Line", entity.LineID},
                    {"@Active", entity.Active},
                    {"@MachineID", entity.MachineID},
                    {"@UserName", entity.UserName ?? string.Empty},
                    {"@Result", string.Empty}
                };

                var result = await _SQL_DB.ExecuteProcedureWithOutputAsync("Ol_Proc_MachineData", parameters, "@Result");
                return result.OutputValue ?? "No result returned from procedure";
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs($"Error in ManageMachineData: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<string> ManageExcelData(List<ProductionMachineEntity> manageExcelData, string status, string username)
        {
            try
            {
                var excelData = _SQL_DB.ConvertToDataTable(manageExcelData.Select(x => new
                {
                    x.CompCode,
                    x.MachineType,
                    x.ModelNo,
                    x.Make,
                    x.MachineSrNo,
                    x.LineID
                }).ToList());
                var param = new Dictionary<string, object>
                {
                    { "@Action", "ManageExcelData" },
                    { "@Data", string.IsNullOrEmpty(status) ? "" : status },
                    { "@UserName", username ?? "" }
                };
                param.Add("@GetExcelTempValue", excelData);

                var result = await _SQL_DB.ExecuteProcedureWithOutputAsync("Ol_Proc_MachineData", param, "ReturnValue");

                return result.OutputValue ?? "No return value";
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs($"Error in ManageExcelData: {ex.Message}, StackTrace: {ex.StackTrace}");
                return "An error occurred while processing the request.";
            }
        }

        //private async Task CreateBarCodeAsync(string barcodeData)
        //{
        //    await Task.CompletedTask;
        //}
        public async Task<DataTable> GetmachineTransferList(string compCode)
        {
            Query = @$"
                    SELECT 
		                    m.id,m.Machine_Type , m.ModelNo , m.Make ,
		                    m.MachineSrNo,m.MachineCode,m.CompCode,
		                    ISNULL([LineId],'') AS [Line ID],
		                    CASE 
			                    WHEN Active = 1 THEN 'Active'
			                    ELSE 'Inactive'
		                    END AS [Status],
		                    ISNULL(EnterBy,'') AS [Registred By] ,
		                    ISNULL(EntryDate,'') AS [Date]
                    FROM ol_MachineDetails m 
                    --INNER JOIN [lineNo] l on l.id = m.LineId
                    WHERE m.CompCode = '{compCode}'
            ";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<int> TransferMachine(ProductionMachineEntity Entity)
        {
            int totalAffectedRows = 0;

            foreach (var item in Entity.Code)
            {
                var query = @$"
                    UPDATE ol_MachineDetails
                    SET CompCode = '{Entity.TargetPremises}',
                        UpdateDate = '{Entity.transferDate}',
                        UpdatedBy = '{Entity.UserName}'
                    WHERE id = {item};
                ";

                int affectedRows = await _SQL_DB.ExecuteNonQuery(query);
                totalAffectedRows += affectedRows;
            }
            return totalAffectedRows;
        }
    }
}


