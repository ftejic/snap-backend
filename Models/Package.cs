namespace snap_backend.Models
{
    public class Package
    {
        public Guid Id { get; set; }
        public required string TrackingNumber { get; set; }
        public required string SenderAddress { get; set; }
        public required string RecipientAddress { get; set; }
        public required string RecipientName { get; set; }
        public required string Status { get; set; }
        public DateTime SentDate { get; set; }
        public DateTime? DeliveredDate { get; set; }

        public Guid CourierId { get; set; }
        public required User Courier { get; set; }

    }

    public enum PackageStatus
    {
        Pending,
        Assigned,
        InTransit,
        Delivered,
        Returned  
    }
}
