using System;

namespace todos.api.Exceptions.Repository
{
    public class UnauthorizedException: Exception
    {

        public UnauthorizedException() 
        {

        }

        public UnauthorizedException(string message):base(message)
        {

        }
    }
}
