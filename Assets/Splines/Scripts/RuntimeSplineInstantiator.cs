using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace HK.SplineUtils
{
    public enum RuntimeSpawnMode
    {
        Immediate,
        ByTime
    }

    [RequireComponent(typeof(SplineContainer))]
    [RequireComponent(typeof(SplineInstantiate))]
    public class RuntimeSplineInstantiator : MonoBehaviour
    {
        private SplineInstantiate splineInstantiate;
        private SplineContainer splineContainer;
        private Spline spline;
        [SerializeField] RuntimeSpawnMode spawnMode;
        [SerializeField] GameObject spawnObject;
        [SerializeField] int instanceCount;
        [SerializeField] float duration = 3f;

        private void Awake()
        {
            splineInstantiate = GetComponent<SplineInstantiate>();
            splineContainer = GetComponent<SplineContainer>();
        }

        void Start()
        {
            splineInstantiate.Container = splineContainer;
            spline = splineContainer.Splines[0];
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnOnSpline(spline, out Coroutine spawnRoutine, DebugSomething);
            }
        }

        private void DebugSomething()
        {
            Debug.Log("Spline operation has finished");
        }

        private void SpawnOnSpline(Spline spline , out Coroutine spawnRoutine , Action spawnFinishCallback = null)
        {
            spawnRoutine = null;

            if (spawnMode == RuntimeSpawnMode.Immediate)
            {
                SpawnImmediately(spline, spawnFinishCallback);                
            }
            else
            {
                spawnRoutine = StartCoroutine(SpawnByTimeRoutine(spline , spawnFinishCallback));
            }
        }

        private void SpawnImmediately(Spline spline , Action spawnFinishCallback)
        {
            float spacing = 1f / (instanceCount - 1);

            for (int i = 0; i < instanceCount; i++)
            {
                float normalizedProgress = i * spacing;

                // Get the position, tangent, and up vector at this normalized position
                SpawnProgressRelatively(spline, normalizedProgress);
            }

            spawnFinishCallback?.Invoke();
        }

        private IEnumerator SpawnByTimeRoutine(Spline spline , Action spawnFinishCallback)
        {
            float timeStep = duration / instanceCount;
            float progress = 0f;

            for (int i = 0; i < instanceCount; i++)
            {
                float normalizedProgress = progress / duration;                
                SpawnProgressRelatively(spline , normalizedProgress);

                yield return new WaitForSeconds(timeStep);
                progress += timeStep;
            }

            if(spawnFinishCallback != null)
            {
                spawnFinishCallback();
            }

        }

        private void SpawnProgressRelatively(Spline spline, float normalizedProgress)
        {
            spline.Evaluate(normalizedProgress, out float3 splinePosition, out float3 tangent, out float3 upVector);

            // Convert float3 to Vector3 (since Unity uses Vector3)
            Vector3 spawnPos = new Vector3(splinePosition.x, splinePosition.y, splinePosition.z);
            Vector3 tangentDir = new Vector3(tangent.x, tangent.y, tangent.z);
            Vector3 upVec = new Vector3(upVector.x, upVector.y, upVector.z);

            // Create a rotation using the tangent and up vector
            Quaternion rotation = Quaternion.LookRotation(tangentDir, upVec);

            // Instantiate the object at the calculated position and rotation
            Instantiate(spawnObject, spawnPos, rotation);
        }
    }
}

