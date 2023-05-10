using IPA;
using TaserSaber.Configuration;
using IPA.Config;
using IPA.Config.Stores;
using System;
using TaserSaber.Installers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using Zenject;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace TaserSaber
{
	[Plugin(RuntimeOptions.DynamicInit)]
	public class Plugin
	{
		public const string HarmonyId = "com.ShyAssassin.TaserSaber";
		internal static readonly Harmony harmony = new Harmony(HarmonyId);

		internal static Plugin Instance { get; private set; }
		internal static IPALogger Logger { get; private set; }

		[Init]
		public Plugin(IPALogger logger, Config conf, Zenjector zenjector)
		{
			Instance = this;
			Plugin.Logger = logger;
			Plugin.Logger?.Debug("Logger initialized.");

			PluginConfig.Instance = conf.Generated<PluginConfig>();
			Plugin.Logger?.Debug("Config loaded");

			zenjector.UseHttpService();
			zenjector.UseLogger(logger);
			zenjector.Install<PluginAppInstaller>(Location.App, PluginConfig.Instance);
			zenjector.Install<PluginGameInstaller>(Location.Player | Location.MultiPlayer);
			Plugin.Logger?.Debug("Installers installed");
		}

		/// <summary>
		/// Called when the plugin is enabled (including when the game starts if the plugin is enabled).
		/// </summary>
		[OnEnable]
		public void OnEnable()
		{
			ApplyHarmonyPatches();
		}

		/// <summary>
		/// Called when the plugin is disabled and on Beat Saber quit. It is important to clean up any Harmony patches, GameObjects, and Monobehaviours here.
		/// The game should be left in a state as if the plugin was never started.
		/// Methods marked [OnDisable] must return void or Task.
		/// </summary>
		[OnDisable]
		public void OnDisable()
		{
			RemoveHarmonyPatches();
		}

		/// <summary>
		/// Attempts to apply all the Harmony patches in this assembly.
		/// </summary>
		internal static void ApplyHarmonyPatches()
		{
			try
			{
				Plugin.Logger?.Debug("Applying Harmony patches.");
				harmony.PatchAll(Assembly.GetExecutingAssembly());
			}
			catch (Exception ex)
			{
				Plugin.Logger?.Error("Error applying Harmony patches: " + ex.Message);
				Plugin.Logger?.Debug(ex);
			}
		}

		/// <summary>
		/// Attempts to remove all the Harmony patches that used our HarmonyId.
		/// </summary>
		internal static void RemoveHarmonyPatches()
		{
			try
			{
				harmony.UnpatchSelf();
			}
			catch (Exception ex)
			{
				Plugin.Logger?.Error("Error removing Harmony patches: " + ex.Message);
				Plugin.Logger?.Debug(ex);
			}
		}
	}
}
