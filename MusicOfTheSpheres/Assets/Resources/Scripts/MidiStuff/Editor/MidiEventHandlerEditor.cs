using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(MidiEventHandler))]
public class MidiEventHandlerEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        MidiEventHandler t = (MidiEventHandler)target;

        if (t.MIDIFile != null) {
            t.midiPath = AssetDatabase.GetAssetPath(t.MIDIFile);
            string extension = Path.GetExtension(t.midiPath);

            if (Path.GetExtension(extension) != ".mid" && Path.GetExtension(extension) != ".midi") {
                t.midiPath = "";
                t.MIDIFile = null;
                Debug.LogError("Selected file is not of type .mid or .midi");
                return;
            }
        }
    }
}
