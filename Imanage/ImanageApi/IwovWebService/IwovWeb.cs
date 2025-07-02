using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IwovWebService.gsoimanage2;

namespace IwovWebService
{
    public class IwovWeb
    {
        private readonly IWOVServices _iwovServices = new IWOVServices();

        public IwovWeb() 
        {

        }

        public GetDatabasesOutput GetDatabases()
        {
            GetDatabasesOutput output = new GetDatabasesOutput();

            try
            {
                _iwovServices.GetDatabases();
            }
            catch (SystemException ex)
            { Console.WriteLine(ex.ToString()); }

            return output;
        }

        public GetDocumentsOutput GetDocuments(GetDocumentsInput getDocumentsInput)
        {
            return _iwovServices.GetDocuments(getDocumentsInput);
        }

    }
}
