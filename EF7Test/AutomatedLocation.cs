using System;
using System.Collections.Generic;

namespace EF7Test
{
    public partial class AutomatedLocation
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string Creator { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public string LastUpdater { get; set; }
        public string LocalFolder { get; set; }
        public string Machine { get; set; }
        public int RowVersion { get; set; }
    }
}
