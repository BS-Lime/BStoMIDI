﻿using BeatSaberMarkupLanguage.Attributes;
using BS_Utils.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BStoMidi
{
    class Settings : PersistentSingleton<Settings>
    {
        public static Config config;
        public static SerialPort arduinoPort = null;
        public static Settings settings;

        [UIValue("boolEnable")]
        public bool IsModEnabled
        {
            get => config.GetBool("BStoMidi", "_isModEnabled", true, true);
            set => config.SetBool("BStoMidi", "_isModEnabled", value);
        }

        [UIValue("list-options")]
        private List<object> options = SerialPort.GetPortNames().ToList<object>();

        [UIValue("list-choice")]
        public string ListChoice
        {
            get => config.GetString("BStoMidi", nameof(ListChoice), "");
            set => config.SetString("BStoMidi", nameof(ListChoice), value);
        }

        [UIValue("baud-options")]
        private List<object> rates = new object[] { 1200, 2400, 4800, 9600, 19200, 38400, 57600, 74880, 115200, 230400, 250000, 500000, 1000000 }.ToList<object>();

        [UIValue("baud-choice")]
        public int BaudChoice
        {
            get => config.GetInt("BStoMidi", nameof(BaudChoice), 115200);
            set => config.SetInt("BStoMidi", nameof(BaudChoice), value);
        }

        [UIValue("event-options")]
        private List<object> events = new object[] { "noteCuts", "lightEvents" }.ToList<object>();

        [UIValue("event-choice")]
        public string EventChoice
        {
            get => config.GetString("BStoMidi", "eventChoice", "");
            set => config.SetString("BStoMidi", "eventChoice", value);
        }

        [UIValue("rainbowMode")]
        public bool RainbowMode
        {
            get => config.GetBool("BStoMidi", "rainbowMode", false, true);
            set => config.SetBool("BStoMidi", "rainbowMode", value);
        }

        [UIAction("#apply")]
        public void UpdateConnection()
        {
            if (IsModEnabled)
            {
                OpenConnection();
            }
            else
            {
                if (arduinoPort != null && arduinoPort.IsOpen)
                {
                    CloseConnection();
                }
            }

            Plugin.thisPlugin.BSEvents_menuSceneLoaded();
        }

        public void OpenConnection()
        {
            if (arduinoPort != null) //is instantiated
            {
                CloseConnection();
                if (!arduinoPort.IsOpen)
                {
                    StartCoroutine(Connect());
                }
            }
            else
            {
                arduinoPort = new SerialPort(ListChoice, BaudChoice, Parity.None, 8);
                StartCoroutine(Connect());
            }
        }

        public IEnumerator Connect()
        {
            bool portIsOpen = false;
            int incomingByte = 0;

            try
            {
                arduinoPort.Open();
                arduinoPort.ReadTimeout = 3000;
                Plugin.log.Notice("Port opened successfully.");
                portIsOpen = true;
            }
            catch (Exception e)
            {
                CloseConnection();
                Plugin.log.Error("Couldn't open the port, check connection and settings.");
                Plugin.log.Error(e);
                portIsOpen = false;
            }

            if (portIsOpen)
            {
                char[] x = new char[1] { '-' };
                arduinoPort.Write(x, 0, 1);
                yield return new WaitForSeconds(3);
                incomingByte = arduinoPort.ReadByte();
                if (incomingByte == (int)'a')
                {
                    Plugin.log.Notice("Connection established.");
                    arduinoPort.Write($"C{LightState.RightTurnOn.AsNumberString()}");
                    arduinoPort.Write("##");
                }
                else
                {
                    Plugin.log.Error("There was some error in two-way communication.");
                }
            }

            yield return null;
        }

        //MODAL
        [UIComponent("modified-text")]
        private TextMeshProUGUI modifiedText;

        [UIAction("refresh-btn-action")]
        private void RefreshStatus()
        {
            if (arduinoPort.IsOpen)
            {
                modifiedText.text = "Connection with arduino has been established.";
            }
            else
            {
                modifiedText.text = "There was some error connecting to arduino.";
            }
        }

        public void Awake()
        {
            config = new Config("BStoMidi");
        }

        public void CloseConnection()
        {
            if (arduinoPort != null)
            {
                if (arduinoPort.IsOpen)
                {
                    Plugin.log.Notice("Disconnecting...");
                    try
                    {
                        arduinoPort.Write("@@");
                        arduinoPort.Close();
                        Plugin.log.Notice("Disconnecting succesful.");
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
