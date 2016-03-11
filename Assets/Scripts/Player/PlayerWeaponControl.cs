﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerWeaponControl : MonoBehaviour {

    [SerializeField]
    private float firingDelay;
    [SerializeField]
    private float bulletSpeed;
    [SerializeField]
    private Slider energySlider;
    [SerializeField]
    private InventorySlot[] leftWeapons;
    [SerializeField]
    private InventorySlot[] rightWeapons;
    [SerializeField]
    private Weapon defaultStartingWeapon;
    [SerializeField]
    private Animator playerAnimator;
    [SerializeField]
    private Transform weaponTransform;

    private Weapon leftGun;
    private Weapon rightGun;

    private int currentLeftWeaponIndex = 0;
    private int currentRightWeaponIndex = 0;

    private int numLeftGuns = 0;

    private bool leftMouseButtonClicked = false;
    private bool rightMouseButtonClicked = false;

	void Awake () {
        StartCoroutine("AddDefaultStartingWeaponToInventory");
	}

    IEnumerator AddDefaultStartingWeaponToInventory ()
    {
        // Must wait for end of frame so that UI scaling can take place. This ensures that the tile is initialized properly.
        yield return new WaitForEndOfFrame();

        InventoryManager.InventoryTileInfo tileInfo = new InventoryManager.InventoryTileInfo(null, defaultStartingWeapon);

        if (GameState.TutorialMode)
        {
            InventoryManager.Instance.InstantiateTileAtPosition(tileInfo, 6);
        } else
        {
            InventoryManager.Instance.InstantiateNewTile(tileInfo);
        }
    }
	
	void Update () {
        if (GameState.Paused)
        {
            return;
        }

        // 0 = left, 1 = right, 2 = middle
        if (Input.GetMouseButton(0) && leftGun != null && Player.PlayerEnergy.HasEnoughEnergy(leftGun.GetEnergyRequirement()))
        {
            leftMouseButtonClicked = true;
            Player.PlayerEnergy.energy -= leftGun.Click(weaponTransform);
        }

        if (leftMouseButtonClicked && !Input.GetMouseButton(0) && leftGun != null)
        {
            leftMouseButtonClicked = false;
            Player.PlayerEnergy.energy -= leftGun.Release(weaponTransform);
        }

        // My original idea of doing the energy management here in a generic way is just not playing nicely at all with the
        // charge gun because of its unusual energy consumption patterns. The interface is basically broken now since I had to hack
        // the energy requirement functions and add the logic into the click handler. Lesson learned: too much abstraction = very
        // inflexible. Better to err on the side of too little abstraction and refactor later once I have a better understanding of
        // my use cases and the variations. Also, TODO: Fix the shit.
        if (Input.GetMouseButton(1) && rightGun != null && Player.PlayerEnergy.HasEnoughEnergy(rightGun.GetEnergyRequirement()))
        {
            rightMouseButtonClicked = true;
            Player.PlayerEnergy.energy -= rightGun.Click(weaponTransform);
            
            if (GameState.TutorialMode)
            {
                TutorialEngine.Instance.Trigger(TutorialTrigger.MachineGunFired);
            }
        }

        if (leftMouseButtonClicked && rightMouseButtonClicked && GameState.TutorialMode)
        {
            TutorialEngine.Instance.Trigger(TutorialTrigger.BothGunsFired);
        }

        if (Input.GetButtonDown("ToggleLeftWeapon"))
        {
            ToggleLeftWeapon();
            if (GameState.TutorialMode)
            {
                TutorialEngine.Instance.Trigger(TutorialTrigger.LeftWeaponSwitched);
            }
        }

        if (Input.GetButtonDown("ToggleRightWeapon"))
        {
            ToggleRightWeapon();
        }

        if (rightMouseButtonClicked && !Input.GetMouseButton(1) && rightGun != null)
        {
            rightMouseButtonClicked = false;
            Player.PlayerEnergy.energy -= rightGun.Release(weaponTransform);
        }
	}

    private void ToggleWeapon (ref bool mouseButtonClicked, ref int weaponIndex, ref Weapon currentWeapon, InventorySlot[] weapons)
    {
        int weaponsExamined = 0;
        int originalWeaponIndex = weaponIndex;
        HideWeapon();
        do
        {
            weaponIndex = weaponIndex == weapons.Length - 1 ? 0 : ++weaponIndex;
            weaponsExamined++;
        } while (!weapons[weaponIndex].Occupied && weaponsExamined < weapons.Length);

        if (originalWeaponIndex != weaponIndex && currentWeapon != null && mouseButtonClicked)
        {
            Player.PlayerEnergy.energy -= currentWeapon.Release(weaponTransform);
            mouseButtonClicked = false;
        }

        currentWeapon = weapons[weaponIndex].GetWeaponIfExists();
        RevealWeapon();
    }

    void ToggleRightWeapon()
    {
        ToggleWeapon(ref rightMouseButtonClicked, ref currentRightWeaponIndex, ref rightGun, rightWeapons);
    }

    void ToggleLeftWeapon ()
    {
        ToggleWeapon(ref leftMouseButtonClicked, ref currentLeftWeaponIndex, ref leftGun, leftWeapons);
    }

    public void ReconfigureWeapons()
    {
        HideWeapon();

        if (!leftWeapons[currentLeftWeaponIndex].Occupied)
        {
            ToggleLeftWeapon();
        } else if (!leftGun)
        {
            leftGun = leftWeapons[currentLeftWeaponIndex].GetWeaponIfExists();
        }

        if (!rightWeapons[currentRightWeaponIndex].Occupied)
        {
            ToggleRightWeapon();
        }
        else if (!rightGun)
        {
            rightGun = rightWeapons[currentRightWeaponIndex].GetWeaponIfExists();
        }

        if (!leftGun)
        {
            playerAnimator.SetBool("Armed", false);
        } else
        {
            playerAnimator.SetBool("Armed", true);
            setUpWeapon(leftGun);
        }
    }

    void setUpWeapon(Weapon weapon)
    {
        weaponTransform = weapon.transform;
        leftGun.GetComponent<SpriteRenderer>().enabled = true;
        Vector2 offset = VectorUtil.DirectionToMousePointer(transform);
        float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        RotateWeapon(angle);

        if (offset.x > 0)
        {
            leftGun.SetToRightSide();
        } if (offset.x < 0)
        {
            leftGun.SetToLeftSide();
        }
    }

    public bool HasGun(WeaponSide side, string weaponName)
    {
        InventorySlot[] weapons = side == WeaponSide.Left ? leftWeapons : rightWeapons;

        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].Occupied && weapons[i].GetWeaponIfExists().name == weaponName)
            {
                return true;
            }
        }

        return false;
    }

    public void RotateWeapon(float angle)
    {
        if (leftGun)
        {
            leftGun.transform.rotation = Quaternion.Euler(0, leftGun.transform.eulerAngles.y, leftGun.FacingLeft ? -angle - 180 : angle);
        }
    }

    void HideWeapon()
    {
        if (leftGun)
        {
            leftGun.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    void RevealWeapon()
    {
        if (leftGun)
        {
            leftGun.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    public void ShowWeaponOnRight()
    {
        if (leftGun && leftGun.FacingLeft)
        {
            leftGun.SetToRightSide();
        }
    }

    public void ShowWeaponOnLeft ()
    {
        if (leftGun && !leftGun.FacingLeft)
        {
            leftGun.SetToLeftSide();
        }
    }

    public enum WeaponSide { Left, Right };
}
