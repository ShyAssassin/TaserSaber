using System;
using TaserSaber.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zenject;
using SiraUtil.Zenject;

namespace TaserSaber.Installers
{
	internal class PluginAppInstaller : Installer
	{
		private readonly PluginConfig Config;

		private PluginAppInstaller(PluginConfig config)
		{
			Config = config;
		}

		public override void InstallBindings()
		{
			Container.BindInstance(Config).AsSingle();
		}
	}
}
