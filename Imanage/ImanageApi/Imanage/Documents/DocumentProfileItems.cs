using Imanage.Documents;
using System.Collections.Generic;
using System.IO;

namespace Imanage
{
    public abstract class DocumentProfileItems
    {
        private string _description;
        private string _extension;

        public readonly static Dictionary<string,ProfileApplicationType> ExtensionsToType = new Dictionary<string, ProfileApplicationType>()
        {
            { "TXT", ProfileApplicationType.ANSI },
            { "EFX", ProfileApplicationType.EFAX },
            { "FORM", ProfileApplicationType.FORM },
            { "VSD", ProfileApplicationType.VISIO },
            { "HTML", ProfileApplicationType.HTML },
            { "HTM", ProfileApplicationType.HTML },
            { "PPT", ProfileApplicationType.POWERPOINT },
            { "PPTX", ProfileApplicationType.PPTX },
            { "MIME", ProfileApplicationType.MIME },
            { "WAV", ProfileApplicationType.VOICE },
            { "TIF", ProfileApplicationType.TIFF },
            { "DOC", ProfileApplicationType.WORD },
            { "DOCX", ProfileApplicationType.WORDX },
            { "PRJ", ProfileApplicationType.PROJECT },
            { "MPP", ProfileApplicationType.PROJECT },
            { "XLS", ProfileApplicationType.EXCEL },
            { "XLSX", ProfileApplicationType.EXCELX },
            { "EML", ProfileApplicationType.EML },
            { "MSG", ProfileApplicationType.EMAIL },
            { "GIF", ProfileApplicationType.GIF },
            { "RTF", ProfileApplicationType.RTF },
            { "WDF", ProfileApplicationType.WDF },
            { "URL", ProfileApplicationType.URL },
            { "NRL", ProfileApplicationType.URL },
            { "XML", ProfileApplicationType.XML },
            { "DWF", ProfileApplicationType.AUTODESK },
            { "PNG", ProfileApplicationType.IMAGE },
            { "JPG", ProfileApplicationType.IMAGE },
            { "JPEG", ProfileApplicationType.IMAGE },
            { "BMP", ProfileApplicationType.IMAGE },
            { "PDF", ProfileApplicationType.ACROBAT },
            { "ZIP", ProfileApplicationType.ZIPFILE },
            { "KML", ProfileApplicationType.GOOGLEEARTH }
        };

        public enum ProfileClassType { DOC, EMAIL, UNKNOWN }
        public enum ProfileApplicationType
        {
            ANSI, AUTODESK, ACROBAT, EFAX, EMAIL, EML, EXCEL, EXCELX, FORM, GIF,
            GOOGLEEARTH, HTML, IMAGE, MIME, POWERPOINT, PPTX, PROJECT, RTF,
            TIFF, URL, VISIO, VOICE, WDF, WORD, WORDX, XML, ZIPFILE
        }
        public enum EmailFields
        {
            ToNames = 14,
            From = 13,
            CcNames = 15,
            //Bcc = 16,
            Sent = 21,
            Received = 22
        }

        public enum ProfileAttributeId
        {
            Author,
            Class,
            Comment,
            Description,
            Operator,
            Type,
            DeclareDate,
            Frozen,
            MarkedForArchive
        }

        public string Author { get; set; }
        public string Operator { get; set; }
        public ProfileClassType Class { get; private set; }
        public string Comment { get; set; }
        public string Description { get { return _description; } set { UpdateDescription(value); } }
        public EmailProfileItems EmailProfileItems { get; set; }
        public string Frozen { get; set; }
        public bool DeclareAsRecord { get; set; }
        public string Extension { get { return _extension; } set { UpdateClassFromExt(value); } }

        public static string GetExtension(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                fileName = string.Join("", fileName.Split(Path.GetInvalidFileNameChars()));
                return Path.GetExtension(fileName).ToUpper().Replace(".", "");
            }

            return string.Empty;
        }

        private void UpdateDescription(string description)
        {
            if (description != null && description.Length > 254)
                _description = description.Substring(0, 254);
            else
                _description = description;
        }

        private void UpdateClassFromExt(string extension)
        {
            _extension = extension;

            if (!string.IsNullOrEmpty(extension) && ExtensionsToType.ContainsKey(extension))
            {
                var type = ExtensionsToType[extension];
                if (type == ProfileApplicationType.EML || type == ProfileApplicationType.EMAIL)
                    Class = ProfileClassType.EMAIL;
                if (type == ProfileApplicationType.ANSI)
                    Class = ProfileClassType.UNKNOWN;
                else
                    Class = ProfileClassType.DOC;
            }
        }
    }
}
