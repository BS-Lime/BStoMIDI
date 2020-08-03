# BStoMidi

This thing allows you to control lighting control software through an external device over Midi.

Doing so removes the need for loopback software.

It is heavily based on [LightToSerialRelight](https://github.com/MyLegIsPotato/LightToSerialRelight) by [MyLegIsPotato](https://github.com/MyLegIsPotato)

The Arduino part is written with Visual Micro. I recommend installing that if you want to edit the Arduino code but using the Arduino IDE works aswell.

## Getting Started

### Mod Install
Simply download the .dll file and drop it into your BeatSaber folder/Plugins.

The Default Baud in the arduino is set to 115200.

Other required mods include [BeatSaberMarkupLanguage](https://github.com/monkeymanboy/BeatSaberMarkupLanguage) and [BeatSaberUtils](https://github.com/Kylemc1413/Beat-Saber-Utils).

### Getting the Arduino ready

You'll need an Arduino Leonardo or anything compareable that supports USB HID emulation for the MIDI Device.

Flash the Arduino sketch onto the Arduino.

### The Light software

Everything done on the left-hand side will get sent on notes 50-59, while the right-hand side will be sent on 60-69 (currently set to notes or the back lasers left and right or the last cut block depending on your setting in the ingame menu, nothing else), and other events come in on 40-49.

You'll get a Note-On immediately followed by a Note-Off for each of those.

The Lightstates are as Followed:

```
        1 TurnOff
        2 RightTurnOn
        3 RightFlashAndLeaveOn
        4 RightFlashAndTurnOff
        5 TurnOff2
        6 LeftTurnOn
        7 LeftFlashAndLeaveOn
        8 LeftFlashAndTurnOff
```

I.e. if the left laser turns on you'll get a Note-On followed by an immediate Note-Off on note 52.

While there is no dedicated command for a strobe effect it is implemented by rapidly turning the lights on and off.

The Events Are:

```
        40 Song started
        41 Song finished (going back to menu for any reason)
        42 Song paused
        43 Sone unpaused
        49 Any light event with rainbow mode enabeled
```
