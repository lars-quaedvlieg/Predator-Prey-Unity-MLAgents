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

        float forceMagnitude = 0.4f * actions.ContinuousActions[0];
        float rotateY = 2f * actions.ContinuousActions[1];

        rb.AddForce(transform.forward * forceMagnitude, ForceMode.Impulse);
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
