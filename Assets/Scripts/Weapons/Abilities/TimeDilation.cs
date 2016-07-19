using UnityEngine;
using System.Collections.Generic;

public class TimeDilation : Weapon {
    [SerializeField]
    private float energyRequirement;
    [SerializeField]
    private float duration;
    [SerializeField]
    private float timeDilationScale;

    private TimeDilationEffect effect;

    void Awake ()
    {
        effect = Camera.main.GetComponent<TimeDilationEffect>();
    }

    void OnLevelWasLoaded()
    {
        effect = Camera.main.GetComponent<TimeDilationEffect>();
    }

    public override float Click ()
    {
        if (CanFire())
        {
            nextFiringTime = Time.time + FiringDelay;
            effect.ActivateEffect();
            GameState.TimeDilationScale = timeDilationScale;

            for (int i = 0; i < GameState.Enemies.Count; i++)
            {
                GameState.Enemies[i].SlowDown();
            }

            CancelInvoke("ResetTimeScale");
            Invoke("ResetTimeScale", duration);
            return energyRequirement;
        }

        return 0;
    }

    void ResetTimeScale()
    {
        GameState.TimeDilationScale = 1;

        for (int i = 0; i < GameState.Enemies.Count; i++)
        {
            GameState.Enemies[i].RestoreSpeed();
        }
    }

    public override string GetDescription ()
    {
        return "Slow down time";
    }

    public override float GetEnergyRequirement ()
    {
        return energyRequirement;
    }

    public override string GetName ()
    {
        return "Time Dilation";
    }

    public override Dictionary<string, object> GetProperties ()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(WeaponProperties.EnergyCost, GetEnergyRequirement());
        dict.Add(WeaponProperties.Duration, duration);
        return dict;
    }
}
