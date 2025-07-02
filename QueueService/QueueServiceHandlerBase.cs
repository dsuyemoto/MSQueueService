namespace QueueService
{
    public abstract class QueueServiceHandlerBase
    {
        protected IQueueService _queueService;
        public abstract void Start();
        public abstract void Stop();
    }
}