# BStoMidi

This thing allows you to control lighting control software through Midi

It is heavily based on [LightToSerialRelight](https://github.com/MyLegIsPotato/LightToSerialRelight) by [MyLegIsPotato](https://github.com/MyLegIsPotato)

## Getting Started

### Mod Install
Just download the .dll file and drop it into your BeatSaber folder/Plugins.

The Default Baud in the arduino is set to 115200.

You will also need other mods, that is [BeatSaberMarkupLanguage](https://github.com/monkeymanboy/BeatSaberMarkupLanguage) and [BeatSaberUtils](https://github.com/Kylemc1413/Beat-Saber-Utils).

### Getting the Arduino ready

You'll need an Arduino Leonardo or anything compareable that supports USB HID emulation for the MIDI Device.

Flash the arduino software onto the Arduino.

### The Light software

Everything done by the left side will get send on note 50 to 59, the right side on 60 to 69 (Currently set to notes or the back lasers left and right or the last cut block depending on your setting in the ingame menu. Nothing else.) and other Events comes in on 40 to currently 49.

You'll get a Note On immediatly followed by a Note Off for each of those.

The Lightstates are as followed:

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

I.e. if the left laser turns on you'll get a Note-On flollowed by an immediate Note-Off on note 52.

Strobe is handled by the game with rapid turning on and off.

The Events are

```
        40 Song started
        41 Song finished
        42 Song paused
        43 Sone unpaused
        49 Any light event with rainbow mode enaboled
```
