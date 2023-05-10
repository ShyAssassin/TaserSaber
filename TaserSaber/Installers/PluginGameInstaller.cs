using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaserSaber.Game;
using Zenject;

namespace TaserSaber.Installers
{
	internal class PluginGameInstaller : Installer
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<EventPlayer>().AsTransient();
		}
	}
}
