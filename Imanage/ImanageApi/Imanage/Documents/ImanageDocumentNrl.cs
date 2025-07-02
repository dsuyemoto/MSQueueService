using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Imanage
{
    public class ImanageDocumentNrl
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }

        public ImanageDocumentNrl()
        {

        }
        public ImanageDocumentNrl(IImanageDocumentOutput imanageDocumentOutput)
        {
            CreateNrlLink(imanageDocumentOutput);
        }

        private void CreateNrlLink(IImanageDocumentOutput imanageDocumentOutput)
        {
            Data = Encoding.UTF8.GetBytes(
                    imanageDocumentOutput.DocumentObjectId.Session + Environment.NewLine +
                    imanageDocumentOutput.DocumentObjectId.GetObjectId() + Environment.NewLine +
                    "[Version]" + Environment.NewLine +
                    "Latest=Y"
                    );

            var fileName = imanageDocumentOutput.DocumentProfileItems.Description;
            var pattern = "[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]";
            var matches = Regex.Matches(fileName, pattern);

            foreach (Match match in matches)
                fileName = fileName.Replace(match.Value, "");
            FileName = fileName.Trim() + ImanageDocument.EXTENSION_NRL;
        }
    }
}
