namespace NelbrizWeb_Api.Logging
{
    public class Logging : ILogging
    {

        //  Just implementing a simple custom logging.
        public void Log(string message, string type)
        {
            if(type == "error")
            {
                Console.WriteLine("ERROR - " +  message);
            }
            else
            {
                Console.WriteLine(message);
            }
        }


        public void Log2(string message, string type)
        {
            if (type == "error")
            {
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("ERROR - " + message);
                Console.BackgroundColor = ConsoleColor.Black;
            }
            else
            {
                if (type == "warning")
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.WriteLine("ERROR - " + message);
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                }
                else
                {
                    Console.WriteLine(message);
                }
            }
        }


    }
}
