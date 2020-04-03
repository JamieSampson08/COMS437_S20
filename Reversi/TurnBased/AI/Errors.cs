using System;
namespace AI
{
    public class Errors
    {
        private string message;

        public Errors() {  }
        
        public Errors(string errorMessage)
        {
            message = errorMessage;
        }
    }
}
