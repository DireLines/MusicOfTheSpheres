using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;

public class MidiEventHandler : MonoBehaviour {

    [SerializeField]
    private MidiParser MIDIParser;

    public ColumnManager columnManager;

    [HideInInspector]
    public string midiPath;

    public UnityEngine.Object MIDIFile;

    //list of MIDI events in the song
    private List<MidiEvent> events;

    //MIDI state information
    private bool eventHappenedThisUpdate = false;
    private List<MidiEvent> currentEvents; //midi events that happened this frame
    private List<int> currentNotes; //notes that are currently playing
    private int currentEventIndex = 0;
    private float currentEventTime = 0f;

    public float MIDITimeRate = 1f;//rate at which time passes for MIDI events (may be affected by items)
    public float ElapsedMIDITime { get; private set; } = 0f;

    [HideInInspector]
    public int seed;


    //list mapping condition functions to functions to execute.
    //other objects will add their MIDI-dependent events to this list.
    //every frame that new MIDI events happen, 
    //every condition function in the list will be evaluated
    //and the associated action will be taken if true.
    private List<Tuple<Func<bool>, Action>> gameEvents;

    private void Awake() {
        byte[] midihash = SHA256.Create().ComputeHash(File.ReadAllBytes(midiPath));
        seed = BitConverter.ToInt32(midihash, 0);
        UnityEngine.Random.InitState(seed);
        events = MIDIParser.CreateMidiEventList(midiPath);
        gameEvents = new List<Tuple<Func<bool>, Action>>();
        currentEvents = new List<MidiEvent>();
        currentNotes = new List<int>();
        currentEventTime = events[currentEventIndex].time;
    }

    private void Start() {
        //example:
        //gameEvents.Add(new Tuple<Func<bool>, Action>(AnyNotes, Whatever));
    }

    private void Update() {
        //increment time
        ElapsedMIDITime += Time.deltaTime * MIDITimeRate;
        eventHappenedThisUpdate = false;
        currentEvents.Clear();

        //check for all new midi events
        while (ElapsedMIDITime >= currentEventTime && currentEventIndex < events.Count - 1) {
            //update currentlyPlaying and other MIDI state information
            eventHappenedThisUpdate = true;
            MidiEvent currentEvent = events[currentEventIndex];
            //print(currentEvent);
            currentEvents.Add(currentEvent);
            if (currentEvent.type.Contains("on")) {
                currentNotes.Add(currentEvent.note);
                if (currentEvent.type.Contains("first")) {
                    columnManager.CreateColumn(currentEvent.note);
                }
                columnManager.PowerOn(currentEvent.note);
            } else {
                currentNotes.Remove(currentEvent.note);
                columnManager.PowerOff(currentEvent.note);
                if (currentEvent.type.Contains("last")) {
                    columnManager.DestroyColumn(currentEvent.note);
                }
            }

            //move to next midi event
            currentEventIndex++;
            currentEventTime = events[currentEventIndex].time;
        }

        //do appropriate in-game actions
        //for each condition in list, execute action if condition is true
        if (eventHappenedThisUpdate) {
            foreach (Tuple<Func<bool>, Action> gameEvent in gameEvents) {
                if (gameEvent.Item1()) {
                    gameEvent.Item2();
                }
            }
        }
    }

    public void AdjustTimeRate(float factor) {
        MIDITimeRate *= factor;
    }

    //method to add a conditional event to the list of events
    //normally called from Start in other object scripts
    public void AddGameEvent(Tuple<Func<bool>, Action> newEvent) {
        gameEvents.Add(newEvent);
    }

    //getters for MIDI state vars, useful for condition functions in other classes
    public List<MidiEvent> GetCurrentEvents() {
        return currentEvents;
    }
    public List<int> GetCurrentNotes() {
        return currentNotes;
    }
    public List<int> GetCurrentNotesModOctave() {
        List<int> result = new List<int>();
        foreach (int note in currentNotes) {
            if (!result.Contains(note))
                result.Add(note % 12);
        }
        return result;
    }
    public int GetNumNotes() {
        return currentNotes.Count;
    }

    //test functions for conditional action thing
    public bool AnyNotes() {
        return currentNotes.Count > 0;
    }
    public bool PlayingNotes(List<int> notes) {
        foreach (int note in notes) {
            if (!currentNotes.Contains(note)) {
                return false;
            }
        }
        return true;
    }
    public List<MidiEvent> EventsForNote(int note) {
        return MIDIParser.getEventsForNote(events, note);
    }
}
