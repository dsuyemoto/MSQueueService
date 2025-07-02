namespace ServiceNow
{
    public interface IServiceNowRest
    {
        SnResultBase CreateTicket(SnTicketCreate snTicketCreate);
        SnResultBase UpdateTicket(SnTicketUpdate snTicketUpdate);
        SnResultBase GetTicket(SnTicketGet snTicketGet);
        SnResultBase DeleteTicket(SnTicketDelete snTicketDelete);
        SnResultBase CreateAttachment(SnAttachmentCreate snTicketAttachmentCreate);
        SnResultBase DeleteAttachment(SnAttachmentDelete snTicketAttachmentDelete);
        SnResultBase GetUser(SnUserGet snUserGet);
        SnResultBase GetGroup(SnGroupGet snGroupGet);
    }
}