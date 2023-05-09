using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.MLAgents.Sensors
{
    /// <summary>
    /// A base class to support sensor components for raycast-based sensors.
    /// </summary>
    public abstract class CustomRayPerceptionSensorComponentBase : SensorComponent
    {
        [SerializeField, FormerlySerializedAs("sensorName")]
        string m_SensorName = "CustomRayPerceptionSensor";

        /// <summary>
        /// The name of the Sensor that this component wraps.
        /// Note that changing this at runtime does not affect how the Agent sorts the sensors.
        /// </summary>
        public string SensorName
        {
            get { return m_SensorName; }
            set { m_SensorName = value; }
        }

        [SerializeField, FormerlySerializedAs("detectableTags")]
        [Tooltip("List of tags in the scene to compare against.")]
        List<string> m_DetectableTags;

        /// <summary>
        /// List of tags in the scene to compare against.
        /// Note that this should not be changed at runtime.
        /// </summary>
        public List<string> DetectableTags
        {
            get { return m_DetectableTags; }
            set { m_DetectableTags = value; }
        }

        [SerializeField, FormerlySerializedAs("raysPerDirection")]
        [Range(0, 50)]
        [Tooltip("Number of rays to the left and right of center.")]
        int m_RaysPerDirection = 3;

        /// <summary>
        /// Number of rays to the left and right of center.
        /// Note that this should not be changed at runtime.
        /// </summary>
        public int RaysPerDirection
        {
            get { return m_RaysPerDirection; }
            // Note: can't change at runtime
            set { m_RaysPerDirection = value; }
        }

        [SerializeField, FormerlySerializedAs("depthRaysPerDirection")]
        [Range(0, 50)]
        [Tooltip("Number of rays to the left and right of center with depth information.")]
        int m_DepthRaysPerDirection = 3;

        /// <summary>
        /// Number of depth rays to the left and right of center.
        /// Note that this should not be changed at runtime.
        /// </summary>
        public int DepthRaysPerDirection
        {
            get { return m_DepthRaysPerDirection; }
            // Note: can't change at runtime
            set { m_DepthRaysPerDirection = value; }
        }

        [SerializeField, FormerlySerializedAs("maxRayDegrees")]
        [Range(0, 180)]
        [Tooltip("Cone size for rays. Using 90 degrees will cast rays to the left and right. " +
            "Greater than 90 degrees will go backwards.")]
        float m_MaxRayDegrees = 70;

        /// <summary>
        /// Cone size for rays. Using 90 degrees will cast rays to the left and right.
        /// Greater than 90 degrees will go backwards.
        /// </summary>
        public float MaxRayDegrees
        {
            get => m_MaxRayDegrees;
            set { m_MaxRayDegrees = value; UpdateSensor(); }
        }

        [SerializeField, FormerlySerializedAs("sphereCastRadius")]
        [Range(0f, 10f)]
        [Tooltip("Radius of sphere to cast. Set to zero for raycasts.")]
        float m_SphereCastRadius = 0.5f;

        /// <summary>
        /// Radius of sphere to cast. Set to zero for raycasts.
        /// </summary>
        public float SphereCastRadius
        {
            get => m_SphereCastRadius;
            set { m_SphereCastRadius = value; UpdateSensor(); }
        }

        [SerializeField, FormerlySerializedAs("rayLength")]
        [Range(1, 1000)]
        [Tooltip("Length of the rays to cast.")]
        float m_RayLength = 20f;

        /// <summary>
        /// Length of the rays to cast.
        /// </summary>
        public float RayLength
        {
            get => m_RayLength;
            set { m_RayLength = value; UpdateSensor(); }
        }

        // The value of the default layers.
        const int k_PhysicsDefaultLayers = -5;
        [SerializeField, FormerlySerializedAs("rayLayerMask")]
        [Tooltip("Controls which layers the rays can hit.")]
        LayerMask m_RayLayerMask = k_PhysicsDefaultLayers;

        /// <summary>
        /// Controls which layers the rays can hit.
        /// </summary>
        public LayerMask RayLayerMask
        {
            get => m_RayLayerMask;
            set { m_RayLayerMask = value; UpdateSensor(); }
        }

        [SerializeField, FormerlySerializedAs("observationStacks")]
        [Range(1, 50)]
        [Tooltip("Number of raycast results that will be stacked before being fed to the neural network.")]
        int m_ObservationStacks = 1;

        /// <summary>
        /// Whether to stack previous observations. Using 1 means no previous observations.
        /// Note that changing this after the sensor is created has no effect.
        /// </summary>
        public int ObservationStacks
        {
            get { return m_ObservationStacks; }
            set { m_ObservationStacks = value; }
        }

        /// <summary>
        /// Color to code a ray that hits another object.
        /// </summary>
        //[HideInInspector]
        [SerializeField]
        [Header("Debug Gizmos", order = 999)]
        internal Color rayHitColor = Color.red;

        /// <summary>
        /// Color to code a ray that avoid or misses all other objects.
        /// </summary>
        //[HideInInspector]
        [SerializeField]
        internal Color rayMissColor = Color.white;

        /// <summary>
        /// Color to code a ray that has depth information.
        /// </summary>
        //[HideInInspector]
        [SerializeField]
        internal Color depthRayColor = Color.blue;

        [NonSerialized]
        CustomRayPerceptionSensor m_RaySensor;

        /// <summary>
        /// Get the CustomRayPerceptionSensor that was created.
        /// </summary>
        public CustomRayPerceptionSensor RaySensor
        {
            get => m_RaySensor;
        }

        /// <summary>
        /// Returns the <see cref="CustomRayPerceptionCastType"/> for the associated raycast sensor.
        /// </summary>
        /// <returns></returns>
        public abstract CustomRayPerceptionCastType GetCastType();

        /// <summary>
        /// Returns the amount that the ray start is offset up or down by.
        /// </summary>
        /// <returns></returns>
        public virtual float GetStartVerticalOffset()
        {
            return 0f;
        }

        /// <summary>
        /// Returns the amount that the ray end is offset up or down by.
        /// </summary>
        /// <returns></returns>
        public virtual float GetEndVerticalOffset()
        {
            return 0f;
        }

        /// <summary>
        /// Returns an initialized raycast sensor.
        /// </summary>
        /// <returns></returns>
        public override ISensor[] CreateSensors()
        {
            var CustomRayPerceptionInput = GetCustomRayPerceptionInput();

            m_RaySensor = new CustomRayPerceptionSensor(m_SensorName, CustomRayPerceptionInput);

            if (ObservationStacks != 1)
            {
                var stackingSensor = new StackingSensor(m_RaySensor, ObservationStacks);
                return new ISensor[] { stackingSensor };
            }

            return new ISensor[] { m_RaySensor };
        }

        /// <summary>
        /// Returns the specific ray angles given the number of rays per direction and the
        /// cone size for the rays.
        /// </summary>
        /// <param name="raysPerDirection">Number of rays to the left and right of center.</param>
        /// <param name="maxRayDegrees">
        /// Cone size for rays. Using 90 degrees will cast rays to the left and right.
        /// Greater than 90 degrees will go backwards.
        /// </param>
        /// <returns></returns>
        internal static float[] GetRayAngles(int raysPerDirection, float maxRayDegrees)
        {
            // Example:
            // { 90, 90 - delta, 90 + delta, 90 - 2*delta, 90 + 2*delta }
            var anglesOut = new float[2 * raysPerDirection + 1];
            var delta = maxRayDegrees / raysPerDirection;
            anglesOut[0] = 90f;
            for (var i = 0; i < raysPerDirection; i++)
            {
                anglesOut[2 * i + 1] = 90 - (i + 1) * delta;
                anglesOut[2 * i + 2] = 90 + (i + 1) * delta;
            }
            return anglesOut;
        }

        /// <summary>
        /// Returns a bool array for which rays should have depth recorded.
        /// Follows the same order as the GetRayAngles function so if that's
        /// changed this will need to be updated accordingly
        /// </summary>
        /// <param name="raysPerDirection">Number of rays to the left and right of center.</param>
        /// <param name="depthRaysPerDirection">Number of depth rays to the left and right of center.</param>
        /// <returns></returns>
        internal static bool[] GetDepthRays(int raysPerDirection, int depthRaysPerDirection)
        {
            // Since rayAngles is as follows:
            // { 90, 90 - delta, 90 + delta, 90 - 2*delta, 90 + 2*delta }
            // The first depthRaysPerDirectiofloatn + 1 elements will be True and rest false
            var depthRays = new bool[2 * raysPerDirection + 1];
            depthRays[0] = true;
            for (var i = 0; i < depthRaysPerDirection; i++)
            {
                depthRays[2 * i + 1] = true;
                depthRays[2 * i + 2] = true;
            }
            // pretty sure default for bool arrays is false but do this as a sanity check anyway
            for (var i = depthRaysPerDirection; i < raysPerDirection; i++)
            {
                depthRays[2 * i + 1] = false;
                depthRays[2 * i + 2] = false;
            }
            return depthRays;
        }

        /// <summary>
        /// Get the CustomRayPerceptionInput that is used by the <see cref="CustomRayPerceptionSensor"/>.
        /// </summary>
        /// <returns></returns>
        public CustomRayPerceptionInput GetCustomRayPerceptionInput()
        {
            var rayAngles = GetRayAngles(RaysPerDirection, MaxRayDegrees);
            var depthRays = GetDepthRays(RaysPerDirection, DepthRaysPerDirection);

            var CustomRayPerceptionInput = new CustomRayPerceptionInput();
            CustomRayPerceptionInput.RayLength = RayLength;
            CustomRayPerceptionInput.DetectableTags = DetectableTags;
            CustomRayPerceptionInput.Angles = rayAngles;
            CustomRayPerceptionInput.DepthRays = depthRays;
            CustomRayPerceptionInput.StartOffset = GetStartVerticalOffset();
            CustomRayPerceptionInput.EndOffset = GetEndVerticalOffset();
            CustomRayPerceptionInput.CastRadius = SphereCastRadius;
            CustomRayPerceptionInput.Transform = transform;
            CustomRayPerceptionInput.CastType = GetCastType();
            CustomRayPerceptionInput.LayerMask = RayLayerMask;

            return CustomRayPerceptionInput;
        }

        internal void UpdateSensor()
        {
            if (m_RaySensor != null)
            {
                var rayInput = GetCustomRayPerceptionInput();
                m_RaySensor.SetCustomRayPerceptionInput(rayInput);
            }
        }

        internal int SensorObservationAge()
        {
            if (m_RaySensor != null)
            {
                return Time.frameCount - m_RaySensor.DebugLastFrameCount;
            }

            return 0;
        }

        void OnDrawGizmosSelected()
        {
            if (m_RaySensor?.CustomRayPerceptionOutput?.RayOutputs != null)
            {
                // If we have cached debug info from the sensor, draw that.
                // Draw "old" observations in a lighter color.
                // Since the agent may not step every frame, this helps de-emphasize "stale" hit information.
                var alpha = Mathf.Pow(.5f, SensorObservationAge());

                foreach (var rayInfo in m_RaySensor.CustomRayPerceptionOutput.RayOutputs)
                {
                    DrawRaycastGizmos(rayInfo, alpha);
                }
            }
            else
            {
                var rayInput = GetCustomRayPerceptionInput();
                // We don't actually need the tags here, since they don't affect the display of the rays.
                // Additionally, the user might be in the middle of typing the tag name when this is called,
                // and there's no way to turn off the "Tag ... is not defined" error logs.
                // So just don't use any tags here.
                rayInput.DetectableTags = null;
                for (var rayIndex = 0; rayIndex < rayInput.Angles.Count; rayIndex++)
                {
                    var rayOutput = CustomRayPerceptionSensor.PerceiveSingleRay(rayInput, rayIndex);
                    DrawRaycastGizmos(rayOutput);
                }
            }
        }

        /// <summary>
        /// Draw the debug information from the sensor (if available).
        /// </summary>
        void DrawRaycastGizmos(CustomRayPerceptionOutput.RayOutput rayOutput, float alpha = 1.0f)
        {
            var startPositionWorld = rayOutput.StartPositionWorld;
            var endPositionWorld = rayOutput.EndPositionWorld;
            var rayDirection = endPositionWorld - startPositionWorld;
            // take abs value of hit fraction since no depth info = -1
            rayDirection *= Math.Abs(rayOutput.HitFraction);

            // hit fraction ^2 will shift "far" hits closer to the hit color
            var lerpT = rayOutput.HitFraction * rayOutput.HitFraction;
            var color = Color.Lerp(rayHitColor, rayMissColor, lerpT);
            color = rayOutput.HitFraction < 0 ? color : Color.Lerp(color, depthRayColor, 0.5f);
            color.a *= alpha;
            Gizmos.color = color;
            Gizmos.DrawRay(startPositionWorld, rayDirection);

            // Draw the hit point as a sphere. If using rays to cast (0 radius), use a small sphere.
            if (rayOutput.HasHit)
            {
                var hitRadius = Mathf.Max(rayOutput.ScaledCastRadius, .05f);
                Gizmos.DrawWireSphere(startPositionWorld + rayDirection, hitRadius);
            }
        }
    }
}
