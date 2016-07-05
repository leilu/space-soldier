using UnityEngine;

public class SniperScope : MonoBehaviour {

    [SerializeField]
    private Material scopeEffectMaterial;
    [SerializeField]
    private bool scopeActivated;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (scopeActivated)
        {
            Graphics.Blit(source, destination, scopeEffectMaterial);
        } else
        {
            Graphics.Blit(source, destination);
        }
    }

    public void Activate()
    {
        scopeActivated = true;
    }

    public void Deactivate()
    {
        scopeActivated = false;
    }

    public bool IsActive()
    {
        return scopeActivated;
    }
}
