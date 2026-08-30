namespace Havenly.BLL.ModelVMs.Account
{
    public class ExternalLoginResultVM
    {
        public bool Succeeded { get; set; }
        public string? ErrorMessage { get; set; }
    }
}