
using System.Security.Cryptography.X509Certificates;

namespace WarehouseMenager.Model
{
    public class userModel: employeeModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
