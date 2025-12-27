using System.Data;
using AssetManagement_EntityClass;

namespace AssetManagement_DataAccess
{
    public class Login
    {
        private readonly SQL_DB _SQL_DB;
        public Login(SQL_DB sQ_DB)
        {
            _SQL_DB = sQ_DB;
        }
        string Query = string.Empty;
        public async Task<DataTable> BindMenu(LoginEntity Entity)
        {
            Query = @"   Declare @MenuID varchar(max)
                Select @MenuID = dbo.fn_DistinctWords(STRING_AGG(Show,',')) from ol_tblmainmenu 
                  where type = 'Assets' and ID in (Select MenuID from TBL_USER_Permissions_New T Left Join USER_RIGHT U on T.[LOGIN_NAME] = U.[LOGIN] 
                                  where T.VIEW_PAGE = 1 and T.LOGIN_NAME = '" + Entity.employeeCode + @"' Or U.[Paycode] = '" + Entity.employeeCode + @"')
                 Select * from ol_tblmainmenu where id in (Select Item from dbo.Split(@MenuID,',')) and type = 'Assets'"
            ;
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }
    }
}
