using UnityEngine;
using System.Collections;

public class TimeDilationEffect : MonoBehaviour {

    [SerializeField]
    private float timeMultiplier;
    [SerializeField]
    private float coreOffset;
    [SerializeField]
    private Material timeDilationEffectMaterial;
    [SerializeField]
    private bool effectActivated = false;

    private float startTime;

	void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (effectActivated)
        {
            if (Time.time - startTime <= 1)
            {
                timeDilationEffectMaterial.SetFloat("_DeltaTime", Time.time - startTime);
                timeDilationEffectMaterial.SetFloat("_WaveCoreOffset", coreOffset);
            } else
            {
                effectActivated = false;
            }

            Graphics.Blit(source, destination, timeDilationEffectMaterial);
        } else
        {
            Graphics.Blit(source, destination);
        }
    }

    public void ActivateEffect()
    {
        effectActivated = true;
        startTime = Time.time;
    }
}
