using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidiTest : MonoBehaviour {
    public MidiParser MP;

    // Use this for initialization
    void Start() {
        List<MidiEvent> myEvents = MP.CreateMidiEventList("Assets/Items/MIDI/bwv790.mid");
        print("myEvents.Count " + myEvents.Count);
        //foreach (MidiEvent theEvent in myEvents) {
        //    print(theEvent);
        //}
    }
}
