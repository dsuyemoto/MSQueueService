using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportToImanageApplication
{
    public class Document
    {
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public byte[] Content { get; set; }
        public string DocumentId { get; set; }
        public string Table { get; set; }
        public string FilePath { get; set; }

        public Document() { }

    }
}
