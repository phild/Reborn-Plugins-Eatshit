
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Buddy.Coroutines;
using EatShit.Gui;
using EatShit.Settings;
using EatShit.Utilities;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Interfaces;
using ff14bot.Managers;
using ff14bot.RemoteWindows;
using TreeSharp;
using Newtonsoft.Json;

namespace EatShit
{
    public class EatShit : BotPlugin
    {

		private const bool DebugShit = false;

		private Composite _coroutine;
		private EatShitSettingsForm _settingsForm;
		private static int _buff = 48;

		public override string Name { get { return "EatShit"; } }
		public override string Author { get { return "Illindar"; } }
		public override Version Version { get { return new Version(0, 9, 0, 1); } }
		public override string Description { get { return "A very simple plugin, with a single focus, designed to extend rather than replace abilities of the Order Bot. Cobbled together (or at least inspired by) from work by others on the Buddy forums. Particular thanks to Evo99, GourmetGuy, Wheredidigo, Apoc, Deathdisguise, and Mastahg."; } }

	    private static async Task<bool> EatFood()
	    {
		    if (EatShitSettings.Instance.FoodName == "" ||
		        !InventoryManager.FilledSlots.ContainsFooditem(EatShitSettings.Instance.FoodName))
			    //ContainsFooditem(Settings.Instance.Id))
		    {
			    Logging.Write(Colors.Aquamarine, "[EatShit] No food selected, check your settings");
			    return false;
		    }

		    if (GatheringManager.WindowOpen)
		    {
			    Logging.Write(Colors.Aquamarine, "[EatShit] Waiting for gathering window to close");
			    return false;
		    }

		    var item = InventoryManager.FilledSlots.GetFoodItem(EatShitSettings.Instance.FoodName);

		    if (item == null) return false;

		    if (FishingManager.State != FishingState.None)
		    {
			    Logging.Write(Colors.Aquamarine, "[EatShit] Stop fishing");
			    Actionmanager.DoAction("Quit", Core.Me);
			    await Coroutine.Wait(5000, () => FishingManager.State == FishingState.None);
		    }

		    if (Core.Me.IsMounted)
		    {
			    Logging.Write(Colors.Aquamarine, "[EatShit] Dismounting to eat");
			    await CommonTasks.StopAndDismount();
		    }

		    Logging.Write(Colors.Aquamarine, "[EatShit] Eating " + item.EnglishName);
		    item.UseItem();
		    await Coroutine.Wait(5000, () => Core.Me.HasAura(_buff));

		    return true;

	    }

	    public override void OnInitialize()
		{
			_coroutine = new Decorator(c => !Core.Me.HasAura(_buff), new ActionRunCoroutine(r => EatFood()));
		}

		/// <summary>
		///     Add event handlers for the various events we want to use. Also add hooks if the bot is already running.
		/// </summary>
		public override void OnEnabled()
        {
            TreeRoot.OnStart += OnBotStart;
            TreeRoot.OnStop += OnBotStop;
            TreeHooks.Instance.OnHooksCleared += OnHooksCleared;
			

            //You only want to add the hook immediately when the plugin is enabled if the TreeRoot is currently running.
            if (TreeRoot.IsRunning)
            {
                AddHooks();
            }
        }

        /// <summary>
        ///     Remove event handlers for the various events we used. Also remove hooks from the main TreeRoot
        /// </summary>
        public override void OnDisabled()
        {
            TreeRoot.OnStart -= OnBotStart;
            TreeRoot.OnStop -= OnBotStop;
        //    TreeHooks.Instance.OnHooksCleared -= OnHooksCleared;
            RemoveHooks();
        }

		public override void OnShutdown()
		{
			TreeRoot.OnStart -= OnBotStart;
			TreeRoot.OnStop -= OnBotStop;
			RemoveHooks();
		}

		/// <summary>
		/// Tell RB that we want to have a Settings Button for this Plugin
		/// </summary>
		public override bool WantButton
        {
            get { return true; }
        }


        /// <summary>
        /// Text to diplay on the Settings Button in the RB Window
        /// </summary>
        public override string ButtonText
        {
            get { return "Eat Me"; }
        }


		#region Properties

		private EatShitSettingsForm _EatShitSettingsForm;

		private EatShitSettingsForm EatShitSettingsForm
		{
			get
			{
				if (_EatShitSettingsForm == null || _EatShitSettingsForm.IsDisposed || _EatShitSettingsForm.Disposing)
				{
					_EatShitSettingsForm = new EatShitSettingsForm();
					_EatShitSettingsForm.Closed += (sender, args) => _EatShitSettingsForm = null;
				}
				return _EatShitSettingsForm;
			}
		}

		#endregion

		#region Events


		/// <summary>
		/// Method that gets called when the Settings Button for this Plugin gets clicked
		/// </summary>
		public override void OnButtonPress()
		{
			EatShitSettingsForm.  ShowDialog();
		}

		/// <summary>
		///     Event Handler for when the event OnBotStart gets fired. We want to make sure that if our Plugin is still enabled
		///     that we add the logic back to the main TreeRoot
		/// </summary>
		private void OnBotStart(BotBase bot)
        {
            if (PluginManager.GetEnabledPlugins().Contains(Name))
            {
                AddHooks();
            }
        }

        /// <summary>
        ///     Event Handler for when the event OnBotStop gets fired. We want to make sure to remove our hooks when the bot stops.
        /// </summary>
        private void OnBotStop(BotBase bot)
        {
            RemoveHooks();
        }

        /// <summary>
        ///     Event Handler for when the event OnHooksCleared gets fired. We want to make sure that if our Plugin is still
        ///     enabled that we add the logic back to the main TreeRoot
        /// </summary>
        private void OnHooksCleared(object sender, EventArgs e)
        {
            if (PluginManager.GetEnabledPlugins().Contains(Name))
            {
                AddHooks();
            }
        }

		#endregion

		#region Hooks
		 
		/// <summary>
		///     Add everything that is in the Logic property to the main TreeRoot
		/// </summary>
		private void AddHooks()
		{
			Logging.Write(Colors.Aquamarine, "Adding EatShit Hook");
			TreeHooks.Instance.AddHook("TreeStart", _coroutine);
		}


		/// <summary>
		///     Remove everything that is in the Logic property to the main TreeRoot
		/// </summary>
		private void RemoveHooks()
        {
			Logging.Write(Colors.Aquamarine, "Removing EatShit Hook");
	        TreeHooks.Instance.RemoveHook("TreeStart", _coroutine);
        }

        #endregion

        //#region Logic

        //private IList<ILogic> _logic;

        //private IList<ILogic> Logic
        //{
        //    get { return _logic ?? (_logic = new List<ILogic>()); }
        //}

        //#endregion
    }
}