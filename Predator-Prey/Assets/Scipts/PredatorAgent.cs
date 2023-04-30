using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class PredatorBehaviour : Agent
{
    public override void Initialize() {
        base.Initialize();
    }
    public override void CollectObservations(VectorSensor sensor) {
        base.CollectObservations(sensor);
    }
    public override void OnActionReceived(float[] vectorAction) {
        base.OnActionReceived(vectorAction);
    }
    public override void Heuristic(float[] actionsOut) {
        base.Heuristic(actionsOut);
    }
    public override void OnEpisodeBegin() {
        base.OnEpisodeBegin();
    }
}
