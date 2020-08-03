using BS_Utils.Utilities;
using BStoMidi;
using System;
using System.IO.Ports;
using System.Linq;
using UnityEngine;

namespace BStoMidi
{
    public class EventListener : MonoBehaviour
    {
        private const float maxArduinoDelay = 5; //Time in miliseconds in which arduino goes through one read cycle. You can calculate your own time by

        private BeatmapObjectCallbackController Ec;
        private ColorManager Cm;
        private BeatmapLevelSO BMD;
        private SongController FI;
        private BeatmapObjectManager _spawnController;
        private int BPM;
        private Color C1;
        private Color C2;
        private float lastEventTime;

        public int redLeft;
        public int greenLeft;
        public int blueLeft;
        public int redRight;
        public int greenRight;
        public int blueRight;
        // printing char each cycle -> going into serial monitor (Arduino IDE) -> enabling timestamps => Subtract earlier time from newer time.

        void Awake()
        {
            _spawnController = FindObjectOfType<BeatmapObjectManager>();

            Plugin.log.Notice("Initializing..");
            Plugin.log.Notice(Settings.instance.eventChoice);

            BSEvents.gameSceneActive += OnGameSceneActive;
            BSEvents.songPaused += OnSongPaused;
            BSEvents.songUnpaused += OnSongUnpaused;

            Ec = Resources.FindObjectsOfTypeAll<BeatmapObjectCallbackController>().FirstOrDefault();
            Cm = Resources.FindObjectsOfTypeAll<ColorManager>().LastOrDefault();
            BMD = Resources.FindObjectsOfTypeAll<BeatmapLevelSO>().FirstOrDefault();
            FI = Resources.FindObjectsOfTypeAll<SongController>().FirstOrDefault();
            if (Settings.instance.eventChoice == "noteCuts")
            {
                Plugin.log.Info("Adding event listner to noteCuts");
                _spawnController.noteWasCutEvent += OnNoteCut;  //Flash on note cuts
            }

            if (Settings.instance.eventChoice == "lightEvents")
            {
                Plugin.log.Info("Adding event listner to lightEvents");
                Ec.beatmapEventDidTriggerEvent += EventHappened; //Flash on map's light events
            }

            BSEvents.menuSceneActive += menuSceneActive;

            C1 = Cm.ColorForNoteType(NoteType.NoteB);
            redLeft = Mathf.RoundToInt(C1.r * 255);
            greenLeft = Mathf.RoundToInt(C1.g * 255);
            blueLeft = Mathf.RoundToInt(C1.b * 255);
            C2 = Cm.ColorForNoteType(NoteType.NoteA);
            redRight = Mathf.RoundToInt(C2.r * 255);
            greenRight = Mathf.RoundToInt(C2.g * 255);
            blueRight = Mathf.RoundToInt(C2.b * 255);

            BPM = (int)BMD.beatsPerMinute; //Not used, may come useful in future

            Plugin.log.Info(" BPM = " + BPM.ToString());

            if (Settings.arduinoPort.IsOpen)
            {
                if (Settings.instance.rainbowMode)
                {
                    StartRainbowMode(Settings.arduinoPort);
                }
                else
                {
                    Plugin.log.Info("Sending Color to arduino...");
                    SendColorToArduino(Settings.arduinoPort);
                }
            }
        }


        private void OnGameSceneActive()
        {
            Settings.arduinoPort.Write("E0");
        }

        private void menuSceneActive()
        {
            Settings.arduinoPort.Write("E1");
        }

        private void OnSongPaused()
        {
            Settings.arduinoPort.Write("E2");
        }

        private void OnSongUnpaused()
        {
            Settings.arduinoPort.Write("E3");
        }

        void OnDestroy()
        {
            if (Settings.arduinoPort.IsOpen)
            {
                Settings.arduinoPort.Write("##");
                Plugin.log.Info("Removing Eventlistener");
            }
        }


        private void EventHappened(BeatmapEventData Data)
        {
            int value = Data.value;
            Int32.TryParse(Data.type.ToString().Replace("Event", string.Empty), out int Event);
            Plugin.log.Info(Event + value.ToString());
            if (value < 2000000000)
            {
                if (Event == LightElement.LeftLasers.AsInt() || Event == LightElement.RightLasers.AsInt())
                {
                    if (Data.time > (lastEventTime + (maxArduinoDelay * 0.001)))
                    {
                        Plugin.log.Info("Event happened: " + value.ToString());
                        if (Settings.instance.rainbowMode)
                        {
                            Settings.arduinoPort.Write("E9");
                        }
                        if (Event == LightElement.LeftLasers.AsInt())
                        {
                            Settings.arduinoPort.Write($"A{value}");
                        }
                        else
                        {
                            Settings.arduinoPort.Write($"B{value}");
                        }

                        lastEventTime = Data.time;
                    }
                }
            }
        }

        private void OnNoteCut(INoteController spawnController, NoteCutInfo info)
        {
            if (!info.allIsOK)
            {
                return;
            }

            Plugin.log.Notice("Note Cut Event!");
            if (Settings.instance.rainbowMode)
            {
                Settings.arduinoPort.Write("E9");
            }

            if (info.saberType == SaberType.SaberA)
            {
                Settings.arduinoPort.Write($"A{LightState.LeftFlashAndLeaveOn.AsNumberString()}"); //LeftTurnOn
            }
            else
            {
                Settings.arduinoPort.Write($"B{LightState.RightFlashAndLeaveOn.AsNumberString()}"); //RightTurnOn
            }
        }

        private void StartRainbowMode(SerialPort port)
        {
            Settings.arduinoPort.Write("E9"); //Starts rainbow mode
        }

        private void SendColorToArduino(SerialPort port)
        {
            Plugin.log.Debug($"Before color boost: {redLeft} {greenLeft} {blueLeft}");
            Plugin.log.Debug($"Before color boost: {redRight} {greenRight} {blueRight}");
            System.Drawing.Color colorLeft = System.Drawing.Color.FromArgb(redLeft, greenLeft, blueLeft);
            System.Drawing.Color colorRight = System.Drawing.Color.FromArgb(redRight, greenRight, blueRight);

            double h1 = colorLeft.GetHue();
            double h2 = colorRight.GetHue();

            Plugin.log.Debug($"double h1 = {h1}");
            Plugin.log.Debug($"double h2 = {h2}");

            HsvToRgb(h1, 1, 1, out int r1, out int g1, out int b1);
            HsvToRgb(h2, 1, 1, out int r2, out int g2, out int b2);

            Plugin.log.Debug($"After color boost: {r1} {g1} {b1}");
            Plugin.log.Debug($"After color boost: {r2} {g2} {b2}");

            var rightColor = new Color(r1, g1, b1);
            var leftColor = new Color(r2, g2, b2);

            decimal PrimaryRed = Convert.ToDecimal(redLeft.ToString());
            decimal PrimaryGreen = Convert.ToDecimal(greenLeft.ToString());
            decimal PrimaryBlue = Convert.ToDecimal(blueLeft.ToString());
            decimal SecondaryRed = Convert.ToDecimal(redRight.ToString());
            decimal SecondaryGreen = Convert.ToDecimal(greenRight.ToString());
            decimal SecondaryBlue = Convert.ToDecimal(blueRight.ToString());

            byte[] colorsByte = new byte[8];
            colorsByte[0] = Convert.ToByte('$');
            colorsByte[1] = Convert.ToByte(PrimaryRed);
            colorsByte[2] = Convert.ToByte(PrimaryGreen);
            colorsByte[3] = Convert.ToByte(PrimaryBlue);
            colorsByte[4] = Convert.ToByte(SecondaryRed);
            colorsByte[5] = Convert.ToByte(SecondaryGreen);
            colorsByte[6] = Convert.ToByte(SecondaryBlue);
            colorsByte[7] = Convert.ToByte('%');

            for (int i = 0; i < colorsByte.Length; i++)
            {
                try
                {
                    Plugin.log.Info("Writing: " + colorsByte[i]);
                }
                catch (Exception e)
                {
                    Plugin.log.Error(e);
                }

            }
            port.Write(colorsByte, 0, 8);
        }

        public static void HsvToRgb(double h, double S, double V, out int r, out int g, out int b)
        {
            // ######################################################################
            // T. Nathan Mundhenk
            // mundhenk@usc.edu
            // C/C++ Macro HSV to RGB

            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { 
                R = G = B = 0; 
            }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }
            r = Clamp((int)(R * 255.0));
            g = Clamp((int)(G * 255.0));
            b = Clamp((int)(B * 255.0));
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        public static int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }
    }
}
