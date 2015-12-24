using System;
using System.Collections.Generic;

namespace EF7Test
{
    public partial class ClientReportingGroup
    {
        public ClientReportingGroup()
        {
            ClientSetup = new HashSet<ClientSetup>();
        }

        public Guid Id { get; set; }
        public string ClientId { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string Creator { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public string LastUpdater { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
        public int RowVersion { get; set; }

        public virtual ICollection<ClientSetup> ClientSetup { get; set; }
        public virtual Client Client { get; set; }
        public virtual ClientReportingGroup Parent { get; set; }
        public virtual ICollection<ClientReportingGroup> InverseParent { get; set; }
    }
}
