using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VizoMenuAPIv3.Models
{
    internal class email
    {
        public class SendEmailRequest
        {
            public string To { get; set; } = default!;
            public string ToName { get; set; } = default!;
            public string Subject { get; set; } = default!;
            public string htmlBody { get; set; } = default!;
            public string textBody { get; set; } = default!;
        }
    }

}
