namespace BeeMart.Api.Data.Entities;

public class InvoiceLock
{
    public Guid IdInvoiceLock { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public string IdUser { get; set; } = string.Empty;
    public DateTime LockDate { get; set; }
    public DateTime LockDateOnly { get; set; }
    public int RunningNumber { get; set; }
    public bool Stsrc { get; set; }
    public DateTime Datein { get; set; }
    public string Userin { get; set; } = string.Empty;
    public DateTime? Dateup { get; set; }
    public string? Userup { get; set; }
    public User User { get; set; } = null!;
}
