using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PredatorAgent : Agent
{

    private Animator _animator;
    private string _currentState;
    private const string RUN_FORWARD = "RunForward";
    private const string RUN_BACKWARD = "RunBack";
    private const string WALK_FORWARD = "Walk Forward";
    private const string WALK_BACKWARD = "Walk Backward";

    private NatureEnvController envController;

    // public bool usePosition = true;

    public void Start() {
        _animator = GetComponent<Animator>();
        ChangeAnimationState(RUN_FORWARD);
        envController = GetComponentInParent<NatureEnvController>();
    }

    public override void OnActionReceived(ActionBuffers actions) {
        float speedForward = envController.predatorMoveSpeed * Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float rotateY = envController.predatorRotateSpeed * Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        transform.position += transform.forward * speedForward * Time.deltaTime;
        transform.Rotate(0f, rotateY, 0f);
    }

    // public override void CollectObservations(VectorSensor sensor) {
    //     if (usePosition) {
    //         sensor.AddObservation(transform.localPosition.x);
    //         sensor.AddObservation(transform.localPosition.z);
    //         sensor.AddObservation(transform.rotation.y);
    //     }
    // }

    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Vertical");
        continuousActions[1] = Input.GetAxisRaw("Horizontal");
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Prey")) {
            Agent preyAgent = other.gameObject.GetComponent<Agent>();
            envController.PredatorPreyCollision(preyAgent, this);
        }
    }

    private void ChangeAnimationState(string newState) {
        if (newState == _currentState) {
            return;
        }

        _animator.Play(newState);
        _currentState = newState;
    }

    private bool IsAnimationPlaying(Animator animator, string stateName) {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) {
            return true;
        }
        return false;
    }

}
