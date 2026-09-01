
namespace Havenly.BLL.ModelVMs.Admin
{
    public class MemberVM
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public int BookingsCount { get; set; }
    }
}
