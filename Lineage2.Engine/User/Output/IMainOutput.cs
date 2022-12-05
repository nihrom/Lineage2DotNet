using Lineage2.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lineage2.Engine.User.Output
{
    public interface IMainOutput
    {
        Task SendAccountCharList(List<L2Player> l2Players, int playKey1);
        Task AccountCharacterSelected(L2Player l2Player, int keyOk);
        Task Logout();
        Task ProtocolSend();
        Task MoveBackwardToLocation(L2Object l2Object, Vector3 current, Vector3 destination);
        Task ExSendManorList();
        Task RequestShowMiniMap();
        Task UserInfo(L2Player l2Player);
        Task NpcInfo(List<L2Npc> npcs);
        Task StatusUpdate(L2Object l2Object);
        Task MyTargetSelected(L2Object l2Object);
    }
}
