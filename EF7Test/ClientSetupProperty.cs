using System;
using System.Collections.Generic;

namespace EF7Test
{
    public partial class ClientSetupProperty
    {
        public ClientSetupProperty()
        {
            ClientSetup = new HashSet<ClientSetup>();
        }

        public Guid Id { get; set; }
        public string Code { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string Creator { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public string LastUpdater { get; set; }
        public int RowVersion { get; set; }
        public string ShortDescription { get; set; }

        public virtual ICollection<ClientSetup> ClientSetup { get; set; }
    }
}
