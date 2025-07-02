namespace ServiceNow
{
    public class ServiceNowRepository : IServiceNowRepository
    {
        static IServiceNowRest _serviceNowRest;
        
        
        public ServiceNowRepository(IServiceNowRest serviceNowRest)
        {
            _serviceNowRest = serviceNowRest;
        }

        public ServiceNowRepository()
        {
            _serviceNowRest = new ServiceNowRest();
        }

        public SnResultBase CreateTicket(SnTicketCreate snTicketCreate)
        {
            return _serviceNowRest.CreateTicket(snTicketCreate);
        }

        public SnResultBase UpdateTicket(SnTicketUpdate snTicketUpdate)
        {
            return _serviceNowRest.UpdateTicket(snTicketUpdate);
        }

        public SnResultBase GetTicket(SnTicketGet snTicketGet)
        {
            return _serviceNowRest.GetTicket(snTicketGet);
        }

        public SnResultBase DeleteTicket(SnTicketDelete snTicketDelete)
        {
            return _serviceNowRest.DeleteTicket(snTicketDelete);
        }

        public SnResultBase CreateAttachment(SnAttachmentCreate snTicketAttachmentCreate)
        {
            return _serviceNowRest.CreateAttachment(snTicketAttachmentCreate);
        }

        public SnResultBase DeleteAttachment(SnAttachmentDelete snTicketAttachmentDelete)
        {
            return _serviceNowRest.DeleteAttachment(snTicketAttachmentDelete);
        }

        public SnResultBase GetUser(SnUserGet snUserGet)
        {
            return _serviceNowRest.GetUser(snUserGet);
        }

        public SnResultBase GetGroup(SnGroupGet snGroupGet)
        {
            return _serviceNowRest.GetGroup(snGroupGet);
        }
    }
}
