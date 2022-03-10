using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SendgridProcessor.Configuration
{
    public class EmailRule
    {
        public string To { get; set; }

        public string Subject { get; set; }

        public string AttachmentName { get; set; }
        public string FolderName { get; set; }
    }
}