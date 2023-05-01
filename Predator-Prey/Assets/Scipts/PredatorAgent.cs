using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PredatorAgent : Agent
{

    public override void OnEpisodeBegin() {
        this.transform.position = new Vector3(-5.5f, 0.5f, 0f);
        this.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
    }

    public override void OnActionReceived(ActionBuffers actions) {
        //TODO: Maximum velocity, etc.
        Rigidbody rb = this.GetComponent<Rigidbody>();

        float speedForward = 5f * Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float rotateY = 2f * Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

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

}
