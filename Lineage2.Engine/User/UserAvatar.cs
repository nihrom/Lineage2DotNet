using Lineage2.Engine.User.Controllers;
using Lineage2.Engine.User.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Engine.User
{
    public class UserAvatar
    {
        public UserAvatar(IMainOutput mainOutput)
        {
            MainOutput = mainOutput;
        }
        public IMainOutput MainOutput { get; set; }
    }
}
