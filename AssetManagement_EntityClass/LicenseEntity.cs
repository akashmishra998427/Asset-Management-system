using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement_EntityClass
{
    public class LicenseEntity
    {
        public string Make { get; set; }

        public string Software_Type { get; set; }

        public string Version { get; set; }

        public string Quantity {get; set;}

        public string License_Type {get; set;}

        public string PurchaseDate {get; set;}

        public string ValidUpTo {get; set;}

        public string PurchaseType {get; set;}

        public string CompneyName {get; set;}

        public string PurchaseUnit {get; set;}

        public string InviceNo {get; set;}

        public string InvoiceDate {get; set;}

        public string Price {get; set;}

        public string PoDate {get; set;}

        public string PoNo {get; set;}

        public string InvoiceFilePath { get; set;}

        public string InvoiceFileName { get; set;}

    }
}
