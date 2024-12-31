using HKAnnotations;
using System;
using System.Collections;
using UnityEngine;

namespace HKShaderCollection.Showcase.Portal
{
    public class PortalControl : MonoBehaviour
    {
        private MeshRenderer[] portalMeshRenderers;
        private readonly string portalClipShaderReference = "_CircleClip";
        [SerializeField] AnimationCurve portalClipCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] float openDuration = 1f;
        [SerializeField] float closeDuration = 0.5f;

        void Start()
        {
            portalMeshRenderers = GetComponentsInChildren<MeshRenderer>();
        }

        [ExecuteByButtonClick]
        public void OpenPortal()
        {
            StopAllCoroutines();

            foreach (MeshRenderer portal in portalMeshRenderers)
            {
                portal.enabled = true;
            }

            StartCoroutine(PortalValueLerp(0f, 0.5f, openDuration));
        }

        [ExecuteByButtonClick]
        public void ClosePortal()
        {
            StopAllCoroutines();
            StartCoroutine(PortalValueLerp(0.5f, 0f, closeDuration, PortalCloseFinishCallback));
        }

        private void PortalCloseFinishCallback()
        {
            foreach (MeshRenderer portal in portalMeshRenderers)
            {
                portal.enabled = false;
            }
        }

        private IEnumerator PortalValueLerp(float startClipValue, float targetClipValue, float duration, Action finishCallback = null)
        {

            foreach (var meshRenderer in portalMeshRenderers)
            {
                Material material = meshRenderer.material;
                if (material.HasProperty(portalClipShaderReference))
                {
                    material.SetFloat(portalClipShaderReference, startClipValue);
                }
            }

            yield return null;

            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float normalizedValue = portalClipCurve.Evaluate(timer / duration);
                float shaderValue = Mathf.Lerp(startClipValue, targetClipValue, normalizedValue);

                foreach (var meshRenderer in portalMeshRenderers)
                {
                    Material material = meshRenderer.material;
                    if (material.HasProperty(portalClipShaderReference))
                    {
                        material.SetFloat(portalClipShaderReference, shaderValue);
                    }
                }

                yield return null;
            }

            finishCallback?.Invoke();
        }
    }

}
