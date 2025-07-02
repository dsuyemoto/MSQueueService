namespace Imanage
{
    public class ImanageDocumentsOutput : ImanageOutput
    {
        public ImanageDocumentOutput[] Documents { get; set; }

        public ImanageDocumentsOutput()
        {

        }

        public ImanageDocumentsOutput(ImanageDocumentOutput[] imanageDocumentOutputs, string[] errors)
        {
            Documents = imanageDocumentOutputs;
            Errors = errors;
        }
    }
}
