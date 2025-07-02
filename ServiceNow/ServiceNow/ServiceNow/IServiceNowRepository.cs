namespace ServiceNow
{
    public interface IServiceNowRepository
    {
        SnResultBase CreateTicket(SnTicketCreate snTicketCreate);
        SnResultBase GetTicket(SnTicketGet snTicketGet);
        SnResultBase UpdateTicket(SnTicketUpdate snTicketUpdate);
        SnResultBase CreateAttachment(SnAttachmentCreate snTicketAttachmentCreate);
        SnResultBase DeleteAttachment(SnAttachmentDelete snTicketAttachmentDelete);
        SnResultBase DeleteTicket(SnTicketDelete snTicketDelete);
        SnResultBase GetUser(SnUserGet snUserGet);
        SnResultBase GetGroup(SnGroupGet snGroupGet);
    }
}
