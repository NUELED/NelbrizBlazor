namespace NelbrizWeb_Api.Logging
{
    public interface ILogging
    {
        public void Log(string message, string type);    
        public void Log2(string message, string type);    
    }
}
