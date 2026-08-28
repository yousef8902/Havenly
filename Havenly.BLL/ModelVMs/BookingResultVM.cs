namespace Havenly.BLL.ModelVMs
{
    public class BookingResultVM
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public long? BookingID { get; set; }
    }
}
