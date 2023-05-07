using UnityEngine;

namespace Unity.MLAgents.Sensors
{
    /// <summary>
    /// A component for 2D Ray Perception.
    /// </summary>
    [AddComponentMenu("ML Agents/Custom Ray Perception Sensor 2D", (int)MenuGroup.Sensors)]
    public class CustomRayPerceptionSensorComponent2D : CustomRayPerceptionSensorComponentBase
    {
        /// <inheritdoc/>
        public override CustomRayPerceptionCastType GetCastType()
        {
            return CustomRayPerceptionCastType.Cast2D;
        }
    }
}
