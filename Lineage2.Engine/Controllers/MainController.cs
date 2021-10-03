using Lineage2.Engine.Repositories;
using Lineage2.Engine.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Engine.Controllers
{
    public class MainController
    {
        IL2PlayersRepository playersRepository;

        public MainController(IL2PlayersRepository playersRepository)
        {
            this.playersRepository = playersRepository;
        }

        public void EnterWorld(UserAvatar avatar)
        {
            var player = playersRepository.Get2Player();
        }
    }
}
