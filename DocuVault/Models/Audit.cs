using System;

namespace DocuVault.Models
{
    // Representing the Audit record with relevant properties
    public class Audit
    {
        public int AuditId { get; set; }        // Primary key for the audit log
        public int UserId { get; set; }         // ID of the user who performed the action
        public string Action { get; set; }      // Description of the action performed
        public DateTime Timestamp { get; set; } // Date and time when the action occurred
    }
}
