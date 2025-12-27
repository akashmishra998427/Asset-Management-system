namespace AssetManagement_EntityClass.Assets
{
    public class ProductionMachineEntity
    {
        public string CompCode { get; set; }
        public string LineID { get; set; }
        public string ModelNo { get; set; }
        public string MachineSrNo { get; set; }
        public string ImageURL { get; set; }
        public string MachineID { get; set; }
        public string Premises { get; set; }
        public string Data { get; set; }
        public string Search { get; set; }
        public string PageNumber { get; set; }
        public string MachineType { get; set; }
        public string Make { get; set; }
        public string MachineCode { get; set; }
        public List<ProductionMachineEntity> ManageExcelData { get; set; }
        public string Status { get; set; }
        public string Department { get; set; }
        public string Value { get; set; }
        public string Action { get; set; }
        public string Tv_Floor { get; set; }
        public string Floor { get; set; }
        public string Result { get; set; }
        public string UserName { get; set; }
        public Boolean Active { get; set; }
        public int PageSize { get; set; } = 10;
        public string TargetPremises { get; set; }
        //public string MachineCode { get; set; }
        public string transferDate { get; set; }
        public List<string> Code { get; set; }

    }
}
