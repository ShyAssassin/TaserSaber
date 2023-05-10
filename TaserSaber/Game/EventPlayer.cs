using System;
using Zenject;
using SiraUtil.Logging;
using TaserSaber.Configuration;
using IPA.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TaserSaber.Game
{
	internal class EventPlayer : IDisposable
	{
		[Inject] private readonly GameEnergyCounter _GameEnergyCounter = null;
		[Inject] private readonly IComboController _IComboController = null;
		[Inject] private readonly BeatmapObjectManager _BeatmapObjectManager = null;
		[Inject] private readonly SiraLog Logger = null;
		[Inject] private readonly PluginConfig Config = null;
		private int PrevCombo = 0;

		[Inject]
		public void BindEvents()
		{
			if (Config.Enable)
			{
				Logger.Info("Binding Events");
				_GameEnergyCounter.gameEnergyDidReach0Event += OnLevelFailed;
				_BeatmapObjectManager.noteWasMissedEvent += OnNoteMiss;
				_BeatmapObjectManager.noteWasCutEvent += OnNoteCut;
				// we arent using `IComboController.comboBreakingEventHappenedEvent` because it will fire even if the player has no combo
				// instead we manually keep track of it see if the player breaks their combo
				_IComboController.comboDidChangeEvent += OnComboChange;
			}
		}

		public void Dispose()
		{
			if (Config.Enable)
			{
				Logger.Info("Unbinding Events");
				_GameEnergyCounter.gameEnergyDidReach0Event -= OnLevelFailed;
				_BeatmapObjectManager.noteWasMissedEvent -= OnNoteMiss;
				_BeatmapObjectManager.noteWasCutEvent -= OnNoteCut;
				_IComboController.comboDidChangeEvent -= OnComboChange;
			}
		}

		public void OnLevelFailed()
		{
			Logger.Info("Died :)");
		}

		public void OnNoteMiss(NoteController noteController)
		{
			// make sure we actually missed a noted and not a bomb
			if (noteController.noteData.colorType != ColorType.None)
			{
				Logger.Info("Missed");
			}
		}


		public void OnNoteCut(NoteController _NoteController, in NoteCutInfo _NoteCutInfo)
		{
			if (_NoteController.noteData.gameplayType == NoteData.GameplayType.Bomb)
			{
				Logger.Info("bomb hit");
			}

			// check for all other miss events other than not actually hitting the block
			if (!_NoteCutInfo.allIsOK) {
				Logger.Info("Missed");
			}
		}

		public void OnComboChange(int Combo)
		{
			// we dont want to electrocute the player
			if (PrevCombo > Combo)
			{
				Logger.Info("Combo Break");
			}
			PrevCombo = Combo;
		}
	}
}
