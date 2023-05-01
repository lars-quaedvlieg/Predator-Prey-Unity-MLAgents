using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PredatorAgent : Agent
{

    Animator _animator;
    string _currentState;
    const string RUN_FORWARD = "RunForward";
    const string RUN_BACKWARD = "RunBack";
    const string WALK_FORWARD = "Walk Forward";
    const string WALK_BACKWARD = "Walk Backward";

    public void Start() {
        _animator = this.gameObject.GetComponent<Animator>();
        ChangeAnimationState(RUN_FORWARD);
    }

    public override void OnEpisodeBegin() {
        this.transform.position = new Vector3(-5.5f, 0.5f, 0f);
        this.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
    }

    public override void OnActionReceived(ActionBuffers actions) {
        //TODO: Maximum velocity, etc.
        Rigidbody rb = this.GetComponent<Rigidbody>();

        float speedForward = 5f * Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float rotateY = 2f * Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        /*
        if (0 < speedForward && speedForward < 0.5f && !IsAnimationPlaying(_animator, RUN_FORWARD)) {
            ChangeAnimationState(RUN_FORWARD);
        } else if (0.5 < speedForward && !IsAnimationPlaying(_animator, RUN_FORWARD)) {
            ChangeAnimationState(RUN_FORWARD);
        } else if (-0.5 < speedForward && speedForward < 0 && !IsAnimationPlaying(_animator, RUN_BACKWARD)) {
            ChangeAnimationState(RUN_BACKWARD);
        } else if (speedForward < -0.5 && !IsAnimationPlaying(_animator, RUN_BACKWARD)) {
            ChangeAnimationState(RUN_BACKWARD);
        }
        */

        this.transform.position += this.transform.forward * speedForward * Time.deltaTime;
        this.transform.Rotate(0f, rotateY, 0f);

    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Vertical");
        continuousActions[1] = Input.GetAxisRaw("Horizontal");
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Prey")) {
            SetReward(1f);
            // Only for the simple case:
            EndEpisode();
        } else if (other.gameObject.CompareTag("Deadzone")) {
            SetReward(-1f);
            // Only for the simple case:
            EndEpisode();
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
