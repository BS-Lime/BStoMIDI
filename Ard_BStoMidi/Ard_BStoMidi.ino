#include "MIDIUSB.h"
int col = 0; //0 is blue
int prevcol = 0;
char notebuffer[2];
char input[2];
byte note;

void setup() {
	Serial.begin(115200);
	pinMode(LED_BUILTIN, OUTPUT);
}
void loop() {
	if (Serial.available())
	{
		Serial.readBytesUntil('\n', input, 2);
		notebuffer[1] = input[1];
		switch (input[0])
		{
		case 'A':
			notebuffer[0] = '5';
			sendNote();
			break;
		case 'B':
			notebuffer[0] = '6';
			sendNote();
			break;
		case 'C':
			notebuffer[0] = '5';
			sendNote();
			notebuffer[0] = '6';
			sendNote();
			break;
		case 'E':
			notebuffer[0] = '4';
			sendNote();
			break;
		default:
			Serial.println("a");
			digitalWrite(LED_BUILTIN, HIGH);
			delay(10);
			digitalWrite(LED_BUILTIN, LOW);
		}
	}
}

void sendNote()
{
	note = atoi(notebuffer);
	noteOn(note);
	digitalWrite(LED_BUILTIN, HIGH);
	MidiUSB.flush();
	noteOff(note);
	digitalWrite(LED_BUILTIN, LOW);
	MidiUSB.flush();
}

void noteOn(byte pitch) {
	midiEventPacket_t noteOn = { 0x09, 0x90 | 1, pitch, 127 };
	MidiUSB.sendMIDI(noteOn);
}

void noteOff(byte pitch) {
	midiEventPacket_t noteOff = { 0x08, 0x80 | 1, pitch, 127 };
	MidiUSB.sendMIDI(noteOff);
}
