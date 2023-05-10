using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.MLAgents.Sensors
{
    /// <summary>
    /// A component for 3D Ray Perception with limited depth.
    /// </summary>
    [AddComponentMenu("ML Agents/Custom Ray Perception Sensor 3D", (int)MenuGroup.Sensors)]
    public class CustomRayPerceptionSensorComponent3D : CustomRayPerceptionSensorComponentBase
    {
        [SerializeField, FormerlySerializedAs("startVerticalOffset")]
        [Range(-10f, 10f)]
        [Tooltip("Ray start is offset up or down by this amount.")]
        float m_StartVerticalOffset;

        /// <summary>
        /// Ray start is offset up or down by this amount.
        /// </summary>
        public float StartVerticalOffset
        {
            get => m_StartVerticalOffset;
            set { m_StartVerticalOffset = value; UpdateSensor(); }
        }

        [SerializeField, FormerlySerializedAs("endVerticalOffset")]
        [Range(-10f, 10f)]
        [Tooltip("Ray end is offset up or down by this amount.")]
        float m_EndVerticalOffset;

        /// <summary>
        /// Ray end is offset up or down by this amount.
        /// </summary>
        public float EndVerticalOffset
        {
            get => m_EndVerticalOffset;
            set { m_EndVerticalOffset = value; UpdateSensor(); }
        }

        /// <inheritdoc/>
        public override CustomRayPerceptionCastType GetCastType()
        {
            return CustomRayPerceptionCastType.Cast3D;
        }

        /// <inheritdoc/>
        public override float GetStartVerticalOffset()
        {
            return StartVerticalOffset;
        }

        /// <inheritdoc/>
        public override float GetEndVerticalOffset()
        {
            return EndVerticalOffset;
        }
    }
}
