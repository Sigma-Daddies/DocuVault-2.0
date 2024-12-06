using System.Windows.Input;

namespace DocuVault.Models
{
    public class Client
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public ICommand LockCommand { get; set; }
        public ICommand UnlockCommand { get; set; }
    }
}
