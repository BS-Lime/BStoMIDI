using BeatSaberMarkupLanguage.Settings;
using BS_Utils.Utilities;
using IPA;
using System;
using UnityEngine;

namespace BStoMidi
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public string Name => "BStoMIDI";
        public string Version => "1.8.1";

        internal static bool gameCoreJustLoaded = false;
        public static IPA.Logging.Logger log;
        public bool eventAdded = false;
        public static Plugin thisPlugin;

        [Init]
        public Plugin(IPA.Logging.Logger logger)
        {
            log = logger;
            log.Info("Initialized BStoMIDI");
            thisPlugin = this;
        }

        [OnStart]
        public void OnStart()
        {
            log.Info("Starting BStoMIDI");
            BSEvents.menuSceneLoaded += BSEvents_menuSceneLoaded;
            BSMLSettings.instance.AddSettingsMenu("BStoMidi", "BStoMidi.UI.settings.bsml", Settings.instance);
            Settings.instance.UpdateConnection();
        }

        public void BSEvents_menuSceneLoaded()
        {
            log.Info("Loaded Menu");
            if (Settings.instance._isModEnabled && Settings.arduinoPort.IsOpen && !eventAdded) //Only happens once per game restart if mod is always enabled.
            {
                log.Info("Adding event for gameSceneLoaded.  Level Event Listener will be spawned when starting a level.");
                BSEvents.gameSceneLoaded += AddEventListener;
                eventAdded = true;
            }
            else if (eventAdded && (!Settings.instance._isModEnabled || !Settings.arduinoPort.IsOpen))
            {
                log.Info("Removing event for gameSceneLoaded. Level Event Listener won't be spawned anymore.");
                BSEvents.gameSceneLoaded -= AddEventListener;
                eventAdded = false;
            }

        }

        [OnExit]
        public void OnExit()
        {
            Settings.instance.CloseConnection();
        }

        private void AddEventListener()
        {
            new GameObject("EventListener").AddComponent<EventListener>();
        }
    }
}