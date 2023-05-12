using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class NatureEnvController : MonoBehaviour
{

    [System.Serializable]
    public class AgentInfo {
        public Agent agent;
        [HideInInspector]
        public Vector3 startingPos;
        [HideInInspector]
        public Quaternion startingRot;
        [HideInInspector]
        public Rigidbody rb;
    }

    private SimpleMultiAgentGroup predatorGroup;
    private SimpleMultiAgentGroup preyGroup;

    // Some environment settings
    public float predatorMoveSpeed = 5f;
    public float predatorRotateSpeed = 2f;
    public float preyMoveSpeed = 4f;
    public float preyRotateSpeed = 1.5f;

    public bool placeRandomly = false;
    public float rnd_x_width = 2.5f;
    public float rnd_z_width = 2.5f;
    public float rotMin = 0f;
    public float rotMax = 360f;

    // Temporary container for killed preys
    private List<Agent> removedAgents = new List<Agent>();

    public List<AgentInfo> agentsList = new List<AgentInfo>();
    private int numInitPrey = 0;
    private int numInitPredator = 0;

    private int numDeadPrey = 0;
    private int numDeadPredator = 0;

    public int maxEnvironmentSteps = 25000;
    private int resetTimer = 0;

    void Start()
    {

        // Create team managers
        predatorGroup = new SimpleMultiAgentGroup();
        preyGroup = new SimpleMultiAgentGroup();

        // Add players to their teams based on their tag
        foreach (var item in agentsList) {
            item.startingPos = item.agent.transform.position;
            item.startingRot = item.agent.transform.rotation;
            item.rb = item.agent.GetComponent<Rigidbody>();
            if (item.agent.CompareTag("Predator")) {
                predatorGroup.RegisterAgent(item.agent);
                numInitPredator += 1;
            } else if (item.agent.CompareTag("Prey")) {
                preyGroup.RegisterAgent(item.agent);
                numInitPrey += 1;
            }
        }

        // Initialize the scene
        ResetScene();

    }

    private float debug = 0;

    void FixedUpdate()
    {   
        preyGroup.AddGroupReward(1f / (float)maxEnvironmentSteps);
        debug += 1f / (float)maxEnvironmentSteps;
        predatorGroup.AddGroupReward(-1 / (float)maxEnvironmentSteps);
        
        resetTimer += 1;
        if (resetTimer >= maxEnvironmentSteps && maxEnvironmentSteps > 0) {
            predatorGroup.GroupEpisodeInterrupted();
            preyGroup.GroupEpisodeInterrupted();
            ResetScene();
        }
    }

    public void PredatorPreyCollision(Collider prey) {
        // Note: For now, the agent cannot die if it collides with the wall (I don't see an issue with this)

        Agent preyAgent = prey.gameObject.GetComponent<Agent>();

        preyGroup.AddGroupReward(-1f / numInitPrey);
        debug += -1f / numInitPrey;
        predatorGroup.AddGroupReward(1f / numInitPrey);

        KillAgent(preyAgent);

        if (numDeadPrey == numInitPrey) {
            preyGroup.EndGroupEpisode();
            predatorGroup.EndGroupEpisode();
            ResetScene();
        }

    }

    public void KillAgent(Agent agent) {
        removedAgents.Add(agent);

        /* This might not even be necessary
        if (agent.CompareTag("Predator")) {
            predatorGroup.UnregisterAgent(agent);
        } else if (agent.CompareTag("Prey")) {
            preyGroup.UnregisterAgent(agent);
        }
        */

        //agent.gameObject.SetActive(false);
        numDeadPrey += 1;
    }

    private void ResetScene() {
        Debug.Log("EnvController: " + debug);
        debug = 0;
        resetTimer = 0;

        //Reset agents position and rotation
        foreach (var item in agentsList) {
            var newStartPos = item.startingPos;
            var newStartRot = item.startingRot;
            if (placeRandomly) {
                // Random position
                var rndX = Random.Range(-rnd_x_width / 2, rnd_x_width / 2);
                var rndZ = Random.Range(-rnd_z_width / 2, rnd_z_width / 2);
                newStartPos += new Vector3(rndX, 0f, rndZ);

                // Random rotation
                var rndRot = Random.Range(rotMin, rotMax);
                newStartRot = Quaternion.Euler(0, rndRot, 0);
            }

            item.agent.transform.SetPositionAndRotation(newStartPos, newStartRot);
        }

        // Add killed preys back to the group
        foreach (var item in removedAgents) {
            item.gameObject.SetActive(true);
            /*
            if (item.CompareTag("Predator")) {
                predatorGroup.RegisterAgent(item);
            } else if (item.CompareTag("Prey")) {
                preyGroup.RegisterAgent(item);
            }
            */
        }

        removedAgents = new List<Agent>();
        numDeadPredator = 0;
        numDeadPrey = 0;
    }
}
