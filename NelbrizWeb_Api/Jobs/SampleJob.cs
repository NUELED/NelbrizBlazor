namespace NelbrizWeb_Api.Jobs
{
    public class SampleJob
    {
        private readonly ILogger _logger;

        public SampleJob(ILogger<SampleJob> logger) => _logger = logger;    
     

        public void WriteLog(string message)
        {
            _logger.LogInformation($"{DateTime.Now: yyyy-mm-dd hh-mm-ss tt}{message}");    
        }






    }
}
