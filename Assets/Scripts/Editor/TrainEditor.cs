using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IntTrain))]
public class TrainEditor : Editor {
    private void OnSceneGUI() {
        var train = (IntTrain)target;

        if (train.stops.Length == 0) return;
        for (int i = 0; i < train.stops.Length - 1; i++) {
            Handles.DrawLine(train.stops[i], train.stops[i + 1]);
        }
    }
}
