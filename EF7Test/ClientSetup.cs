using System;
using System.Collections.Generic;

namespace EF7Test
{
    public partial class ClientSetup
    {
        public Guid Id { get; set; }
        public Guid ClientReportingGroup { get; set; }
        public Guid ClientSetupProperty { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string Creator { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? GuidValue { get; set; }
        public int? IntValue { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public string LastUpdater { get; set; }
        public decimal? MoneyValue { get; set; }
        public int RowVersion { get; set; }
        public string StringValue { get; set; }

        public virtual ClientReportingGroup ClientReportingGroupNavigation { get; set; }
        public virtual ClientSetupProperty ClientSetupPropertyNavigation { get; set; }
    }
}
