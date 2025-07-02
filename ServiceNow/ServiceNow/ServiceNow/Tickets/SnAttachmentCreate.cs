namespace ServiceNow
{
    public class SnAttachmentCreate : SnAttachment
    {
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public byte[] ContentBytes { get; set; }

        public SnAttachmentCreate(
            string tablename,
            string instanceurl,
            string username, 
            string password,
            string fileName, 
            string mimeType,
            string sysId, 
            byte[] content) : base(instanceurl, username, password)
        {
            FileName = fileName;
            MimeType = mimeType;
            ContentBytes = content;
            TableName = tablename;
            SysId = sysId;
        }
    }
}
