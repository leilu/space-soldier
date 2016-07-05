using UnityEngine;

public class InventorySlot : MonoBehaviour {
    public bool Occupied {
        get {
            return tile != null;
        }
    }

    public SkillType SkillType;

    private InventoryTile tile;

    public Weapon GetWeaponIfExists()
    {
        return tile ? tile.GetWeapon() : null;
    }

    public void UnsetTile()
    {
        tile = null;
    }

    public void SetTile(InventoryTile tile)
    {
        this.tile = tile;
    }

    public bool CanHostSkillType(SkillType type)
    {
        bool canHost = SkillType == SkillType.All || type == SkillType;
        if (!canHost)
        {
            Debug.Log("can't host, yo");
        }
        return canHost;
    }
}