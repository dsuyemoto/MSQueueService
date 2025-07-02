namespace ServiceNow
{
    public class SnResponseGetTicketsDTO
    {
        public SnResultGetTicketDTO[] Result { get; set; }
        public SnErrorResult Error { get; set; }
    }
}