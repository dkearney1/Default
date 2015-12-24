using System;
using System.Collections.Generic;

namespace EF7Test
{
    public partial class TestUDT
    {
        public Guid Id { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
