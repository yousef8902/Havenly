namespace Havenly.BLL.ModelVMs.Payment
{
    public class PaymobSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string IntegrationId { get; set; } = string.Empty;
        public string IframeId { get; set; } = string.Empty;
        public string HmacSecret { get; set; } = string.Empty;
        public bool IsTestMode { get; set; } = true;
    }
}
