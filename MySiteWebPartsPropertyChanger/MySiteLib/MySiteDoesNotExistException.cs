using System;
using System.Collections.Generic;
using System.Text;

namespace MySiteLib
{
    public class MySiteDoesNotExistException:SystemException
    {
        public override string Message
        {
            get
            {
                return "The MySite Does not exist for the current user. Please specify the Account to be used in the configuration file";
            }
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
