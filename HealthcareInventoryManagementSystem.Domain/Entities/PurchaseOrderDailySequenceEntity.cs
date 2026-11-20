
using System;

namespace HealthcareInventoryManagementSystem.Domain.Entities
{
    public class PurchaseOrderDailySequenceEntity
    {
        // Pakistan local calendar date for this sequence.
        public DateTime OrderDate { get; set; }

        // Last sequence number assigned on this date.
        public int LastSequence { get; set; }
    }
}
