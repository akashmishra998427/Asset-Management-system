using System.Data;
using AssetManagement_EntityClass.Reports;

namespace AssetManagement_DataAccess.Reports
{
    public class MachineReport
    {
        private readonly SQL_DB _SQL_DB;
        string Query = string.Empty;

        public MachineReport(SQL_DB sQL_DB)
        {
            _SQL_DB = sQL_DB;
        }

        public async Task<DataTable> GetMachineDetails(MachineReportEntity Entity)
        {
            var Params = new Dictionary<string, object>
            {
                {"@Action", "GetData"},
                {"@CompCode",Entity.CompCode },
                {"@LineId", Entity.LineId},
                {"@MachineType",Entity.MachineType },
                {"@Make",Entity.Make}
            };
            return await _SQL_DB.ExecuteProcedureAsync("GetMachineData", Params);
        }
        public async Task<DataTable> BindPremises()
        {
            Query = @"select distinct compcode, N_compcode from PREMISES";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }
        public async Task<DataTable> BindLineID()
        {
            Query = @"SELECT DISTINCT LineId FROM ol_MachineDetails";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }
    }
}
