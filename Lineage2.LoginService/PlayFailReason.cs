using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.LoginService
{
    public enum PlayFailReason
    {
        ReasonSystemError = 0x01,
        ReasonUserOrPassWrong = 0x02,
        Reason3 = 0x03,
        Reason4 = 0x04,
        ReasonTooManyPlayers = 0x0f
    }
}
