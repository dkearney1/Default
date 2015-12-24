using System;
using System.Collections.Generic;

namespace EF7Test
{
    public partial class Client
    {
        public Client()
        {
            ClientReportingGroup = new HashSet<ClientReportingGroup>();
        }

        public Guid Id { get; set; }
        public string ClientId { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string Creator { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public string LastUpdater { get; set; }
        public string Name { get; set; }
        public int RowVersion { get; set; }

        public virtual ICollection<ClientReportingGroup> ClientReportingGroup { get; set; }
    }
}
