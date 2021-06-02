using company.world.Crosscutting.Constants;

namespace company.world.Crosscutting.Exceptions
{
    public class EmailNotFoundException : BaseException
    {
        public EmailNotFoundException() : base(ErrorConstants.EmailNotFoundType, "Email address not registered")
        {
        }
    }
}
