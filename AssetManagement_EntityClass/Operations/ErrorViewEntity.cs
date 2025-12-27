namespace AssetManagement_EntityClass.Operations
{
    public class ErrorViewEntity
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
