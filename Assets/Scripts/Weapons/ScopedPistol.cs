using UnityEngine;
using System;
using System.Collections.Generic;

public class ScopedPistol : Weapon {

    private LineRenderer lineRenderer;

    public override float Click ()
    {
        throw new NotImplementedException();
    }

    public override string GetDescription ()
    {
        throw new NotImplementedException();
    }

    public override float GetEnergyRequirement ()
    {
        throw new NotImplementedException();
    }

    public override string GetName ()
    {
        throw new NotImplementedException();
    }

    public override Dictionary<string, object> GetProperties ()
    {
        throw new NotImplementedException();
    }

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }
}
