using company.world.Crosscutting.Constants;

namespace company.world.Crosscutting.Exceptions
{
    public class EmailAlreadyUsedException : BadRequestAlertException
    {
        public EmailAlreadyUsedException() : base(ErrorConstants.EmailAlreadyUsedType, "Email is already in use!",
            "userManagement", "emailexists")
        {
        }
    }
}
