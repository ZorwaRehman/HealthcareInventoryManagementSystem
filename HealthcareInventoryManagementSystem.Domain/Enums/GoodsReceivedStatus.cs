
namespace HealthcareInventoryManagementSystem.Domain.Enums
{
    public enum GoodsReceivedStatus
    {
        // Receipt is being prepared and has not affected stock.
        Draft = 1,

        // Receipt has been confirmed and its accepted quantities processed.
        Confirmed = 2,

        // Receipt has been cancelled.
        Cancelled = 3
    }
}
