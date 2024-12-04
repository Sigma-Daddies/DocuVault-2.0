using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuVault.Models
{
    public class Document
    {
        public int DocumentId { get; set; }
        public int UserId { get; set; }
        public string DocumentName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
