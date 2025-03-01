
using System.Security.Cryptography.X509Certificates;

namespace WarehouseMenager.Model
{
    internal class userModel: employeeModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
