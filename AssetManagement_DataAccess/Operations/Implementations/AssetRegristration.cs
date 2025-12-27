using AssetManagement_DataAccess.Operations.Interfaces;
using AssetManagement_EntityClass;
using System.Data;
using System.Text;

namespace AssetManagement_DataAccess.Operations.Implementations
{
    public class AssetRegristration : IAssetManager
    {
        private readonly SQL_DB _SQL_DB;

        public AssetRegristration(SQL_DB sQL_DB, AssetManagerEntity entity)
        {
            _SQL_DB = sQL_DB;
        }

        public async Task<int> AddAsset(AssetManagerEntity entity)
        {
            if (entity.Asset_Type == "L")
            {
                entity.Monitor_Asset_Code = entity.CPU_ASSET_CODE;
            }

            try
            {
                string checkQuery = $"SELECT COUNT(CPU_ASSET_CODE) FROM asset WHERE CPU_ASSET_CODE = '{entity.CPU_ASSET_CODE}'";
                var existingCount = await _SQL_DB.ExecuteScalar(checkQuery);
                if (Convert.ToInt32(existingCount) > 0)
                {
                    return -1;
                }

                string Query = "BEGIN TRANSACTION;";

                Query += $@"INSERT INTO asset( 
				           [Group], [Asset_Type], [PURCHASE_UNIT], [PURCHASE_DATE_YEAR], [Bill_No], [Bill_Date],
				           [INSTALLED_UNIT], [CPU_ASSET_CODE], [USER_NAME], [DEPT], [DEPARTMENT], [MachineCode],
				           [MAKE], [Model_Number], [RAM], [HDD], [PROCESSOR], [Monitor],
				           [Monitor_Asset_Code],[Monitor_Make], [Employee_Code], [Login_Id], [Ip_No], [WARRANTY_AMC],
				           [UPTO], [MONTH] ,[premises], [CompCode], [vendor], [SCCM],
				           [Temp_Data], [Status], [Reg_Status], [SpRemark], [File1], [ChkChecked], [U_Remarks],
				           [U_Sheet_No], [U_Engineer],[Install_Date], [LoginName], [HDD_Type], [RAM_Type],[Screen_Size],
                           [RemoteAllowed],CPU_V,Machine_Sl_No,Windows_Version1,Windows_Version_Installed,Windows_Key,
                           Windows_License_Type_Paper_OEM,Desktop_Laptop_Server,DTTM
				         )
				         VALUES
						        (
						         'RICHA','{entity.Asset_Type}', '{entity.PURCHASE_UNIT}', '{entity.PURCHASE_DATE_YEAR}', '{entity.Bill_No}', '{entity.Bill_Date}',
                                 '{entity.INSTALLED_UNIT}', '{entity.CPU_ASSET_CODE}', '{entity.USER_NAME}', '{entity.DEPT}', '{entity.DEPT}', '{entity.MachineCode}',
                                 '{entity.MAKE}', '{entity.Model_Number}', '{entity.RAM}', '{entity.HDD}', '{entity.PROCESSOR}', '{entity.Monitor}',
                                 '{entity.Monitor_Asset_Code}','{entity.Monitor_Make}', '{entity.Employee_Code}', '{entity.Login_Id}', '{entity.Ip_No}', '{entity.WARRANTY_AMC}',
                                 '{entity.WarrantyValidity}', '{entity.MONTH}' ,'{entity.Premises}', '{entity.CompCode}', '{entity.vendor}', '{entity.SCCM}',
                                 '{entity.Temp_Data}', '{entity.Status}', '{entity.Reg_Status}', '{entity.SpRemark}', '{entity.File1}', '{entity.ChkChecked}', '{entity.U_Remarks}', 
                                 '{entity.U_Sheet_No}', '{entity.U_Engineer}','{entity.Install_Date}', '{entity.LoginName}','{entity.HDD_Type}','{entity.RAM_Type}','{entity.Screen_Size}',
                                 '{entity.RemoteAllowed}','{entity.CPU_V}','{entity.SR_NO}','{entity.Software}','{entity.SoftwareVersion}',
                                 '{entity.SoftwareKey}','{entity.SoftwareLicence}','{entity.Desktop_Laptop_Server}', GETDATE()
						        )
 
                        INSERT INTO tbl_AddationalAsset_Info
									(
									  [AssetCode], [WarrantyValidity], [Warranty_Months], [Price], [Panel_Type],
									  [Printer_Type], [Rent_StartDate], [Screen_Resolution], [RentEndDate],
									  [RentedCompney_Name], [PrinterPrintSpeed], [PrinterPaper_Capacity], 
									  [PortCount],[Firewall_Type], [Throughput], [Recognition_Type], [User_Capacity], [Connectivity_Type],
                                      [Chair_Type],[ChairColor], [AC_CoolingCapacity_Ton], [EnergyRating], 
									  [AC_InstallationLocation], [VehicleType], [VehicleRegistration_No],
									  [VehicleEngine_No], [VehicleFuel_Type], [Generation], [UsedBy], [NoiseLevel]
									)
					    VALUES
						      (
                                '{entity.CPU_ASSET_CODE}','{entity.WarrantyValidity}','{entity.MONTH}','{entity.Price}','{entity.Panel_Type}',
                                '{entity.Printer_Type}','{entity.RentStartDate}', '{entity.Resolution}', '{entity.RentEndDate}',
                                '{entity.Rental_Company}','{entity.PrintSpeed}','{entity.PrinterPaper_Capacity}',
                                '{entity.Port_Count}','{entity.Firewall_Type}','{entity.Throughput}', '{entity.RecognitionType}','{entity.UserCapacity}','{entity.Connectivity}',
                                '{entity.ChairType}','{entity.Color}','{entity.CoolingCapacityTon}','{entity.EnergyRating}',
                                '{entity.InstallationLocation}','{entity.VehicleType}','{entity.RegistrationNo}',
                                '{entity.EngineNo}','{entity.FuelType}','{entity.Generation}','{entity.UsedBy}','{entity.NoiseLevel}'
						      )
                ";

                if (entity.With_Monitor)
                {
                    Query += @$"
                        INSERT INTO asset (  
                            CPU_ASSET_CODE, DEPT, MAKE, Model_Number,  
                            PURCHASE_UNIT, PURCHASE_DATE_YEAR, Bill_No, Bill_Date,  
                            vendor, Asset_Type, [Group], Screen_Size, INSTALLED_UNIT,  
                            WARRANTY_AMC, USER_NAME, Employee_Code,Monitor_Asset_Code,M_SR_no,[UPTO], [MONTH]                           
                        )

                        VALUES (  
                            '{entity.Monitor_Asset_Code}', '{entity.DEPT}', '{entity.Monitor_Make}', '{entity.Model_Number}',  
                            '{entity.PURCHASE_UNIT}', '{entity.PURCHASE_DATE_YEAR}', '{entity.Bill_No}', '{entity.Bill_Date}',  
                            '{entity.vendor}', 'M', 'RICHA', '{entity.Screen_Size}', '{entity.INSTALLED_UNIT}',  
                            '{entity.WARRANTY_AMC}', '{entity.USER_NAME}', '{entity.Employee_Code}','{entity.Monitor_Asset_Code}', 
                            '{entity.MonitorSrNo}','{entity.WarrantyValidity}','{entity.MONTH}'
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
                //Query.Replace(",,", ",");
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

        public async Task<int> UpdateAssetDetails(AssetManagerEntity entity)
        {
            try
            {
                string checkQuery = $"SELECT COUNT(CPU_ASSET_CODE) FROM asset WHERE CPU_ASSET_CODE = '{entity.CPU_ASSET_CODE}'";
                var existingCount = await _SQL_DB.ExecuteScalar(checkQuery);

                if (Convert.ToInt32(existingCount) == 0)
                {
                    // Asset not found
                    return -1;
                }
                if (entity.HDD == "NO HDD") entity.HDD = "";

                var Query = new StringBuilder();
                Query.AppendLine("BEGIN TRANSACTION;");

                // ---------------------------------------
                // MAIN ASSET TABLE UPDATE
                // ---------------------------------------
                var assetUpdates = new List<string>();

                void AddAssetField(string field, string? value)
                {
                    if (!string.IsNullOrEmpty(value))
                        assetUpdates.Add($"[{field}] = '{value.Replace("'", "''")}'");
                }

                AddAssetField("Group", "RICHA");
                AddAssetField("Asset_Type", entity.Asset_Type);
                AddAssetField("CPU_V", entity.CPU_V);
                AddAssetField("Machine_Sl_No", entity.Machine_Sl_No);
                AddAssetField("MonitorSrNo", entity.MonitorSrNo);
                AddAssetField("PURCHASE_UNIT", entity.PURCHASE_UNIT);
                AddAssetField("PURCHASE_DATE_YEAR", entity.PURCHASE_DATE_YEAR);
                AddAssetField("Bill_No", entity.Bill_No);
                AddAssetField("Bill_Date", entity.Bill_Date);
                AddAssetField("INSTALLED_UNIT", entity.INSTALLED_UNIT);
                AddAssetField("USER_NAME", entity.USER_NAME);
                AddAssetField("DEPT", entity.DEPT);
                AddAssetField("DEPARTMENT", entity.DEPARTMENT);
                AddAssetField("MachineCode", entity.MachineCode);
                AddAssetField("MAKE", entity.MAKE);
                AddAssetField("Model_Number", entity.Model_Number);
                AddAssetField("RAM", entity.RAM);
                AddAssetField("HDD", entity.HDD);
                AddAssetField("PROCESSOR", entity.PROCESSOR);
                AddAssetField("Monitor", entity.Monitor);
                AddAssetField("Monitor_Asset_Code", entity.Monitor_Asset_Code);
                AddAssetField("Monitor_Make", entity.Monitor_Make);
                AddAssetField("Employee_Code", entity.Employee_Code);
                AddAssetField("Login_Id", entity.Login_Id);
                AddAssetField("Ip_No", entity.Ip_No);
                AddAssetField("WARRANTY_AMC", entity.WARRANTY_AMC);
                AddAssetField("UPTO", entity.UPTO);
                AddAssetField("MONTH", entity.MONTH);
                AddAssetField("Windows_Version1", entity.Software); // Added
                AddAssetField("Windows_Version_Installed", entity.Windows_Version_Installed);
                AddAssetField("Windows_Key", entity.SoftwareKey); // Added
                AddAssetField("Windows_License_Type_Paper_OEM", entity.SoftwareLicence); // Added
                AddAssetField("Desktop_Laptop_Server", entity.Desktop_Laptop_Server); // Added
                AddAssetField("premises", entity.Premises);
                AddAssetField("CompCode", entity.CompCode);
                AddAssetField("vendor", entity.vendor);
                AddAssetField("SCCM", entity.SCCM);
                AddAssetField("Temp_Data", entity.Temp_Data);
                AddAssetField("Status", entity.Status);
                AddAssetField("Reg_Status", entity.Reg_Status);
                AddAssetField("SpRemark", entity.SpRemark);
                AddAssetField("File1", entity.File1);
                AddAssetField("ChkChecked", entity.ChkChecked);
                AddAssetField("U_Remarks", entity.U_Remarks);
                AddAssetField("U_Sheet_No", entity.U_Sheet_No);
                AddAssetField("U_Engineer", entity.U_Engineer);
                AddAssetField("Install_Date", entity.Install_Date);
                AddAssetField("LoginName", entity.LoginName);
                AddAssetField("HDD_Type", entity.HDD_Type);
                AddAssetField("RAM_Type", entity.RAM_Type);
                AddAssetField("Screen_Size", entity.Screen_Size);
                AddAssetField("RemoteAllowed", entity.RemoteAllowed);
                AddAssetField("Software", entity.Software);
                AddAssetField("SoftwareVersion", entity.SoftwareVersion);
                AddAssetField("SoftwareKey", entity.SoftwareKey);
                AddAssetField("SoftwareLicence", entity.SoftwareLicence);

                if (assetUpdates.Count > 0)
                {
                    Query.AppendLine($@"
                UPDATE asset 
                SET {string.Join(", ", assetUpdates)}
                WHERE CPU_ASSET_CODE = '{entity.CPU_ASSET_CODE.Replace("'", "''")}';
            ");
                }

                // ---------------------------------------
                // ADDITIONAL ASSET INFO UPDATE
                // ---------------------------------------
                var additionalUpdates = new List<string>();

                void AddAdditionalField(string field, string? value)
                {
                    if (!string.IsNullOrEmpty(value))
                        additionalUpdates.Add($"[{field}] = '{value.Replace("'", "''")}'");
                }

                AddAdditionalField("WarrantyValidity", entity.WarrantyValidity);
                AddAdditionalField("Warranty_Months", entity.MONTH);
                AddAdditionalField("Price", entity.Price);
                AddAdditionalField("Panel_Type", entity.Panel_Type);
                AddAdditionalField("Printer_Type", entity.Printer_Type);
                AddAdditionalField("Rent_StartDate", entity.RentStartDate);
                AddAdditionalField("Screen_Resolution", entity.Resolution);
                AddAdditionalField("RentEndDate", entity.RentEndDate);
                AddAdditionalField("RentedCompney_Name", entity.Rental_Company);
                AddAdditionalField("PrinterPrintSpeed", entity.PrintSpeed);
                AddAdditionalField("PrinterPaper_Capacity", entity.PrinterPaper_Capacity);
                AddAdditionalField("PortCount", entity.Port_Count);
                AddAdditionalField("Firewall_Type", entity.Firewall_Type);
                AddAdditionalField("Throughput", entity.Throughput);
                AddAdditionalField("Recognition_Type", entity.RecognitionType);
                AddAdditionalField("User_Capacity", entity.UserCapacity);
                AddAdditionalField("Connectivity_Type", entity.Connectivity);
                AddAdditionalField("Chair_Type", entity.ChairType);
                AddAdditionalField("ChairColor", entity.Color);
                AddAdditionalField("AC_CoolingCapacity_Ton", entity.CoolingCapacityTon);
                AddAdditionalField("EnergyRating", entity.EnergyRating);
                AddAdditionalField("AC_InstallationLocation", entity.InstallationLocation);
                AddAdditionalField("VehicleType", entity.VehicleType);
                AddAdditionalField("VehicleRegistration_No", entity.RegistrationNo);
                AddAdditionalField("VehicleEngine_No", entity.EngineNo);
                AddAdditionalField("VehicleFuel_Type", entity.FuelType);
                AddAdditionalField("Generation", entity.Generation);
                AddAdditionalField("UsedBy", entity.UsedBy);
                AddAdditionalField("NoiseLevel", entity.NoiseLevel);

                if (additionalUpdates.Count > 0)
                {
                    Query.AppendLine($@"
                UPDATE tbl_AddationalAsset_Info 
                SET {string.Join(", ", additionalUpdates)}
                WHERE AssetCode = '{entity.CPU_ASSET_CODE.Replace("'", "''")}';
            ");
                }

                // ---------------------------------------
                // MONITOR ASSET UPDATE
                // ---------------------------------------
                if (entity.With_Monitor && !string.IsNullOrEmpty(entity.Monitor_Asset_Code))
                {
                    var monitorUpdates = new List<string>();
                    void AddMonitorField(string field, string? value)
                    {
                        if (!string.IsNullOrEmpty(value))
                            monitorUpdates.Add($"[{field}] = '{value.Replace("'", "''")}'");
                    }

                    AddMonitorField("DEPT", entity.DEPT);
                    AddMonitorField("MAKE", entity.Monitor_Make);
                    AddMonitorField("Model_Number", entity.Model_Number);
                    AddMonitorField("PURCHASE_UNIT", entity.PURCHASE_UNIT);
                    AddMonitorField("PURCHASE_DATE_YEAR", entity.PURCHASE_DATE_YEAR);
                    AddMonitorField("Bill_No", entity.Bill_No);
                    AddMonitorField("Bill_Date", entity.Bill_Date);
                    AddMonitorField("vendor", entity.vendor);
                    AddMonitorField("Asset_Type", "M");
                    AddMonitorField("Group", "RICHA");
                    AddMonitorField("Screen_Size", entity.Screen_Size);
                    AddMonitorField("INSTALLED_UNIT", entity.INSTALLED_UNIT);
                    AddMonitorField("WARRANTY_AMC", entity.WARRANTY_AMC);
                    AddMonitorField("USER_NAME", entity.USER_NAME);
                    AddMonitorField("Employee_Code", entity.Employee_Code);
                    AddMonitorField("Monitor_Asset_Code", entity.Monitor_Asset_Code);

                    if (monitorUpdates.Count > 0)
                    {
                        Query.AppendLine($@"
                    UPDATE asset 
                    SET {string.Join(", ", monitorUpdates)} 
                    WHERE CPU_ASSET_CODE = '{entity.Monitor_Asset_Code.Replace("'", "''")}';
                ");
                    }

                    var monitorAdditionalUpdates = new List<string>();
                    AddAdditionalField("Price", entity.Price);
                    AddAdditionalField("UsedBy", entity.UsedBy);
                    AddAdditionalField("WarrantyValidity", entity.WarrantyValidity);
                    AddAdditionalField("Warranty_Months", entity.MONTH);
                    AddAdditionalField("Screen_Resolution", entity.Resolution);
                    AddAdditionalField("Panel_Type", entity.Panel_Type);

                    if (monitorAdditionalUpdates.Count > 0)
                    {
                        Query.AppendLine($@"
                    UPDATE tbl_AddationalAsset_Info 
                    SET {string.Join(", ", monitorAdditionalUpdates)} 
                    WHERE AssetCode = '{entity.Monitor_Asset_Code.Replace("'", "''")}';
                ");
                    }
                }

                Query.AppendLine("COMMIT;");
                return await _SQL_DB.ExecuteNonQuery(Query.ToString());
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message);
                return 0;
            }
        }


        //public async Task<int> UpdateAssetDetails(AssetManagerEntity entity)
        //{
        //    try
        //    {
        //        string checkQuery = $"SELECT COUNT(CPU_ASSET_CODE) FROM asset WHERE CPU_ASSET_CODE = '{entity.CPU_ASSET_CODE}'";
        //        var existingCount = await _SQL_DB.ExecuteScalar(checkQuery);

        //        if (Convert.ToInt32(existingCount) == 0)
        //        {
        //            // Asset not found
        //            return -1;
        //        }
        //        if (entity.HDD == "NO HDD") entity.HDD = "";

        //        var Query = new StringBuilder();
        //        Query.AppendLine("BEGIN TRANSACTION;");

        //        // ---------------------------------------
        //        // MAIN ASSET TABLE UPDATE
        //        // ---------------------------------------
        //        var assetUpdates = new List<string>();

        //        void AddAssetField(string field, string? value)
        //        {
        //            if (!string.IsNullOrEmpty(value))
        //                assetUpdates.Add($"[{field}] = '{value.Replace("'", "''")}'");
        //        }

        //        AddAssetField("Group", "RICHA");
        //        AddAssetField("Asset_Type", entity.Asset_Type);
        //        AddAssetField("CPU_V", entity.CPU_V);
        //        AddAssetField("MonitorSrNo", entity.MonitorSrNo);
        //        AddAssetField("PURCHASE_UNIT", entity.PURCHASE_UNIT);
        //        AddAssetField("PURCHASE_DATE_YEAR", entity.PURCHASE_DATE_YEAR);
        //        AddAssetField("Bill_No", entity.Bill_No);
        //        AddAssetField("Bill_Date", entity.Bill_Date);
        //        AddAssetField("INSTALLED_UNIT", entity.INSTALLED_UNIT);
        //        AddAssetField("USER_NAME", entity.USER_NAME);
        //        AddAssetField("DEPT", entity.DEPT);
        //        AddAssetField("DEPARTMENT", entity.DEPARTMENT);
        //        AddAssetField("MachineCode", entity.MachineCode);
        //        AddAssetField("MAKE", entity.MAKE);
        //        AddAssetField("Model_Number", entity.Model_Number);
        //        AddAssetField("Machine_Sl_No", entity.Machine_Sl_No);
        //        AddAssetField("RAM", entity.RAM);
        //        AddAssetField("HDD", entity.HDD);
        //        AddAssetField("PROCESSOR", entity.PROCESSOR);
        //        AddAssetField("Monitor", entity.Monitor);
        //        AddAssetField("Monitor_Asset_Code", entity.Monitor_Asset_Code);
        //        AddAssetField("Monitor_Make", entity.Monitor_Make);
        //        AddAssetField("Employee_Code", entity.Employee_Code);
        //        AddAssetField("Login_Id", entity.Login_Id);
        //        AddAssetField("Ip_No", entity.Ip_No);
        //        AddAssetField("WARRANTY_AMC", entity.WARRANTY_AMC);
        //        AddAssetField("UPTO", entity.UPTO);
        //        AddAssetField("MONTH", entity.MONTH);
        //        AddAssetField("Windows_Version_Installed", entity.Windows_Version_Installed);
        //        AddAssetField("premises", entity.Premises);
        //        AddAssetField("CompCode", entity.CompCode);
        //        AddAssetField("vendor", entity.vendor);
        //        AddAssetField("SCCM", entity.SCCM);
        //        AddAssetField("Temp_Data", entity.Temp_Data);
        //        AddAssetField("Status", entity.Status);
        //        AddAssetField("Reg_Status", entity.Reg_Status);
        //        AddAssetField("SpRemark", entity.SpRemark);
        //        AddAssetField("File1", entity.File1);
        //        AddAssetField("ChkChecked", entity.ChkChecked);
        //        AddAssetField("U_Remarks", entity.U_Remarks);
        //        AddAssetField("U_Sheet_No", entity.U_Sheet_No);
        //        AddAssetField("U_Engineer", entity.U_Engineer);
        //        AddAssetField("Install_Date", entity.Install_Date);
        //        AddAssetField("LoginName", entity.LoginName);
        //        AddAssetField("HDD_Type", entity.HDD_Type);
        //        AddAssetField("RAM_Type", entity.RAM_Type);
        //        AddAssetField("Screen_Size", entity.Screen_Size);
        //        AddAssetField("RemoteAllowed", entity.RemoteAllowed);

        //        if (assetUpdates.Count > 0)
        //        {
        //            Query.AppendLine($@"
        //                UPDATE asset 
        //                SET {string.Join(", ", assetUpdates)}
        //                WHERE CPU_ASSET_CODE = '{entity.CPU_ASSET_CODE.Replace("'", "''")}';
        //            ");
        //        }
        //        var additionalUpdates = new List<string>();

        //        void AddAdditionalField(string field, string? value)
        //        {
        //            if (!string.IsNullOrEmpty(value))
        //                additionalUpdates.Add($"[{field}] = '{value.Replace("'", "''")}'");
        //        }

        //        AddAdditionalField("WarrantyValidity", entity.WarrantyValidity);
        //        AddAdditionalField("Warranty_Months", entity.MONTH);
        //        AddAdditionalField("Price", entity.Price);
        //        AddAdditionalField("Panel_Type", entity.Panel_Type);
        //        AddAdditionalField("Printer_Type", entity.Printer_Type);
        //        AddAdditionalField("Rent_StartDate", entity.RentStartDate);
        //        AddAdditionalField("Screen_Resolution", entity.Resolution);
        //        AddAdditionalField("RentEndDate", entity.RentEndDate);
        //        AddAdditionalField("RentedCompney_Name", entity.Rental_Company);
        //        AddAdditionalField("PrinterPrintSpeed", entity.PrintSpeed);
        //        AddAdditionalField("PrinterPaper_Capacity", entity.PrinterPaper_Capacity);
        //        AddAdditionalField("PortCount", entity.Port_Count);
        //        AddAdditionalField("Firewall_Type", entity.Firewall_Type);
        //        AddAdditionalField("Throughput", entity.Throughput);
        //        AddAdditionalField("Recognition_Type", entity.RecognitionType);
        //        AddAdditionalField("User_Capacity", entity.UserCapacity);
        //        AddAdditionalField("Connectivity_Type", entity.Connectivity);
        //        AddAdditionalField("Chair_Type", entity.ChairType);
        //        AddAdditionalField("ChairColor", entity.Color);
        //        AddAdditionalField("AC_CoolingCapacity_Ton", entity.CoolingCapacityTon);
        //        AddAdditionalField("EnergyRating", entity.EnergyRating);
        //        AddAdditionalField("AC_InstallationLocation", entity.InstallationLocation);
        //        AddAdditionalField("VehicleType", entity.VehicleType);
        //        AddAdditionalField("VehicleRegistration_No", entity.RegistrationNo);
        //        AddAdditionalField("VehicleEngine_No", entity.EngineNo);
        //        AddAdditionalField("VehicleFuel_Type", entity.FuelType);
        //        AddAdditionalField("Generation", entity.Generation);
        //        AddAdditionalField("UsedBy", entity.UsedBy);
        //        AddAdditionalField("NoiseLevel", entity.NoiseLevel);

        //        if (additionalUpdates.Count > 0)
        //        {
        //            Query.AppendLine($@"
        //                UPDATE tbl_AddationalAsset_Info 
        //                SET {string.Join(", ", additionalUpdates)}
        //                WHERE AssetCode = '{entity.CPU_ASSET_CODE.Replace("'", "''")}';
        //            ");
        //        }

        //        // ---------------------------------------
        //        // MONITOR ASSET UPDATE  
        //        // ---------------------------------------
        //        if (entity.With_Monitor && !string.IsNullOrEmpty(entity.Monitor_Asset_Code))
        //        {
        //            var monitorUpdates = new List<string>();
        //            void AddMonitorField(string field, string? value)
        //            {
        //                if (!string.IsNullOrEmpty(value))
        //                    monitorUpdates.Add($"[{field}] = '{value.Replace("'", "''")}'");
        //            }

        //            AddMonitorField("DEPT", entity.DEPT);
        //            AddMonitorField("MAKE", entity.Monitor_Make);
        //            AddMonitorField("Model_Number", entity.Model_Number);
        //            AddMonitorField("PURCHASE_UNIT", entity.PURCHASE_UNIT);
        //            AddMonitorField("PURCHASE_DATE_YEAR", entity.PURCHASE_DATE_YEAR);
        //            AddMonitorField("Bill_No", entity.Bill_No);
        //            AddMonitorField("Bill_Date", entity.Bill_Date);
        //            AddMonitorField("vendor", entity.vendor);
        //            AddMonitorField("Asset_Type", "M");
        //            AddMonitorField("Group", "RICHA");
        //            AddMonitorField("Screen_Size", entity.Screen_Size);
        //            AddMonitorField("INSTALLED_UNIT", entity.INSTALLED_UNIT);
        //            AddMonitorField("WARRANTY_AMC", entity.WARRANTY_AMC);
        //            AddMonitorField("USER_NAME", entity.USER_NAME);
        //            AddMonitorField("Employee_Code", entity.Employee_Code);
        //            AddMonitorField("Monitor_Asset_Code", entity.Monitor_Asset_Code);

        //            if (monitorUpdates.Count > 0)
        //            {
        //                Query.AppendLine($@"
        //                    UPDATE asset 
        //                    SET {string.Join(", ", monitorUpdates)} 
        //                    WHERE CPU_ASSET_CODE = '{entity.Monitor_Asset_Code.Replace("'", "''")}';
        //                ");
        //            }

        //            var monitorAdditionalUpdates = new List<string>();
        //            AddAdditionalField("Price", entity.Price);
        //            AddAdditionalField("UsedBy", entity.UsedBy);
        //            AddAdditionalField("WarrantyValidity", entity.WarrantyValidity);
        //            AddAdditionalField("Warranty_Months", entity.MONTH);
        //            AddAdditionalField("Screen_Resolution", entity.Resolution);
        //            AddAdditionalField("Panel_Type", entity.Panel_Type);

        //            if (monitorAdditionalUpdates.Count > 0)
        //            {
        //                Query.AppendLine($@"
        //                    UPDATE tbl_AddationalAsset_Info 
        //                    SET {string.Join(", ", monitorAdditionalUpdates)} 
        //                    WHERE AssetCode = '{entity.Monitor_Asset_Code.Replace("'", "''")}';
        //                ");
        //            }
        //        }

        //        Query.AppendLine("COMMIT;");

        //        return await _SQL_DB.ExecuteNonQuery(Query.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        _SQL_DB.ExceptionLogs(ex.Message);
        //        return 0;
        //    }
        //}

    }
}
