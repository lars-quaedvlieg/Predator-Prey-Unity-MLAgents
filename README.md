# [Visual Intelligence (CS-503)](https://vilab.epfl.ch/) @ EPFL. Hunting for Insights: Investigating Predator-Prey Dynamics through Simulated Vision and Reinforcement Learning

Course project for CS-503 at EPFL. This research project investigates how different vision fields affect predator-prey interactions. By simulating 
simplified environments and training agents with reinforcement learning and self-play, we identify trends that emerge in the strategies and effectiveness
of trained predator and prey agents which use varying vision fields. 

[Project Display Image](https://imgtr.ee/i/S2FLI)

**Author:** 
- [Lars C.P.M. Quaedvlieg](https://lars-quaedvlieg.github.io/)
- [Arvind S. Menon](https://arvind6599.github.io/)
- [Somesh Mehra](https://vilab.epfl.ch/#prospective)

## Important Links

- [Report Website](https://arvind6599.github.io/PredatorPreyWebsite/) 
- [Recorded Video](https://www.youtube.com/watch?v=K0tJrpMla-o)
- [Presentation](https://docs.google.com/presentation/d/1PcIU6uHoWMkugl5NohnZsoXTvl1WS9NC8TRfWVWeUQM/)
- [Unity](https://unity.com/download)
- [Unity ML-Agents](https://github.com/Unity-Technologies/ml-agents)

## Project introduction

The natural world is full of fascinating and complex interactions between predators and prey, with each constantly adapting and evolving to survive. 
As researchers seek to better understand these dynamics, visual intelligence has emerged as a critical field of study, allowing us to gain 
new insights into how animals perceive and react to their environments. In this research project, we will investigate the prey-predator setting by 
training agents in a simplified environment with obstacles, using different vision fields to simulate different types of prey and predators observed 
in the real world. Our goal is to examine emerging behaviours that can shed light on the strategies used by animals to survive 
an attack or hunt a prey, as well as evaluate how different types of vision can help or hinder predator and prey agents. Furthermore, we will examine 
the psychology of chasing and how prey agents might learn to use occlusions in their environment to their advantage, avoiding the predator's line of 
sight and increasing their chances of survival.

## Getting Started

1) install Unity (our project uses version 2021.3.24f1)
2) Set up Python (preferably version 3.7)
3) Clone the project onto your local machine and open the project.

### Requirements

Locate the repository, create a Python environment, and run:
  ```sh
  pip install -r requirements.yaml
  ```

This will install all necessary packages to train your own agent on the environments using Unity's ML-Agents package.

### Usage

Please follow any Unity tutorial to find out [how to open a project](https://support.unity.com/hc/en-us/articles/4402520287124-How-do-I-add-a-project-saved-on-my-computer-into-the-Unity-Hub-),
and how to work with the Unity editor.

## Structure of the repository

Since the project is mostly developed in Unity, we insert the following screenshot of the project structure:

[Project Structure](https://imgtr.ee/i/SwQXB)

In order to train an agent, you need to [build](https://docs.unity3d.com/Manual/BuildSettings.html) a scene and then train the agents in the scene with a set
configuration. The configurations for training with self-play can be found in the "Predator-Prey/config" subfolder. For more information about the specific
configurations, you can read about it [here](https://unity-technologies.github.io/ml-agents/Training-Configuration-File/). Afterwards, you can use Python to train the 
agents in the following way:

1) Navigate to the "Predator-Prey" folder in your command line.
2) Activate your Python environment
3) run ```sh mlagents-learn CONFIG_PATH --env=BUILT_SCENE_PATH --run-id=ANY_RUN_IDENTIFIER --no-graphics``` (you can add "--resume" if you wish to continue training from a previous checkpoint.
4) Enjoy :)

In order to track your training progress, you can run tensorboard in the same directory in the following way:

1) Navigate to the "Predator-Prey" folder in your command line.
2) Activate your Python environment
3) run ```sh tensorboard --logdir results``` and open the webpage
4) Enjoy even more :)

For inference, you can use the "inference" scene in Unity, which will write logs to the "Predator-Prey/inference_logs" folder. This folder also contains a file called
"experiments.ipynb", which can be used to generate quantitative results from the generated logs.
