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

    private Weapon leftGun;
    private Weapon rightGun;

    private int currentLeftWeaponIndex = 0;
    private int currentRightWeaponIndex = 0;

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
            InventoryManager.Instance.InstantiateTileAtPosition(tileInfo, 6, SkillType.Weapon);
        } else
        {
            InventoryManager.Instance.InstantiateNewTile(tileInfo, SkillType.Weapon);
        }
    }
	
	void Update () {
        if (GameState.Paused || GameState.InputLocked)
        {
            return;
        }

        // 0 = left, 1 = right, 2 = middle
        if (Input.GetMouseButton(0) && leftGun != null && Player.PlayerEnergy.HasEnoughEnergy(leftGun.GetEnergyRequirement()))
        {
            leftMouseButtonClicked = true;
            Player.PlayerEnergy.energy -= leftGun.Click();
        }

        if (leftMouseButtonClicked && !Input.GetMouseButton(0) && leftGun != null)
        {
            leftMouseButtonClicked = false;
            Player.PlayerEnergy.energy -= leftGun.Release();
        }

        // My original idea of doing the energy management here in a generic way is just not playing nicely at all with the
        // charge gun because of its unusual energy consumption patterns. The interface is basically broken now since I had to hack
        // the energy requirement functions and add the logic into the click handler. Lesson learned: too much abstraction = very
        // inflexible. Better to err on the side of too little abstraction and refactor later once I have a better understanding of
        // my use cases and the variations. Also, TODO: Fix the shit.
        if (Input.GetMouseButton(1) && rightGun != null && Player.PlayerEnergy.HasEnoughEnergy(rightGun.GetEnergyRequirement()))
        {
            rightMouseButtonClicked = true;
            Player.PlayerEnergy.energy -= rightGun.Click();
            
            if (GameState.TutorialMode)
            {
                TutorialEngine.Instance.Trigger(TutorialTrigger.MachineGunFired);
            }
        }

        if (leftMouseButtonClicked && rightMouseButtonClicked && GameState.TutorialMode)
        {
            TutorialEngine.Instance.Trigger(TutorialTrigger.BothGunsFired);
        }

        if (Input.GetButtonDown("ToggleLeftWeapon") || Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            ToggleLeftWeaponForward();
            if (GameState.TutorialMode)
            {
                TutorialEngine.Instance.Trigger(TutorialTrigger.LeftWeaponSwitched);
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            ToggleLeftWeaponBackwards();
        }

        if (Input.GetButtonDown("ToggleRightWeapon"))
        {
            ToggleRightWeapon();
        }

        if (leftMouseButtonClicked && !Input.GetMouseButton(0) && leftGun != null)
        {
            leftMouseButtonClicked = false;
            Player.PlayerEnergy.energy -= leftGun.Release();
        }

        if (rightMouseButtonClicked && !Input.GetMouseButton(1) && rightGun != null)
        {
            rightMouseButtonClicked = false;
            Player.PlayerEnergy.energy -= rightGun.Release();
        }
	}

    private void ToggleWeapon (ref bool mouseButtonClicked, ref int weaponIndex, ref Weapon currentWeapon,
        InventorySlot[] weapons, int dir)
    {
        int weaponsExamined = 0;
        int originalWeaponIndex = weaponIndex;
        DisableWeaponIfExists(currentWeapon);
        do
        {
            if (dir == 1)
            {
                weaponIndex = weaponIndex == weapons.Length - 1 ? 0 : ++weaponIndex;
            } else
            {
                weaponIndex = weaponIndex == 0 ? weapons.Length - 1 : --weaponIndex;
            }
            weaponsExamined++;
        } while (!weapons[weaponIndex].Occupied && weaponsExamined < weapons.Length);

        if (originalWeaponIndex != weaponIndex && currentWeapon != null && mouseButtonClicked)
        {
            Player.PlayerEnergy.energy -= currentWeapon.Release();
            mouseButtonClicked = false;
        }

        currentWeapon = weapons[weaponIndex].GetWeaponIfExists();
        EnableWeaponIfExists(currentWeapon);
        RevealWeapon();
    }

    void ToggleRightWeapon()
    {
        ToggleWeapon(ref rightMouseButtonClicked, ref currentRightWeaponIndex, ref rightGun, rightWeapons, 1);
    }

    void ToggleLeftWeaponForward ()
    {
        ToggleWeapon(ref leftMouseButtonClicked, ref currentLeftWeaponIndex, ref leftGun, leftWeapons, 1);
    }

    void ToggleLeftWeaponBackwards ()
    {
        ToggleWeapon(ref leftMouseButtonClicked, ref currentLeftWeaponIndex, ref leftGun, leftWeapons, -1);
    }

    public void ReconfigureWeapons()
    {
        if (!leftWeapons[currentLeftWeaponIndex].Occupied)
        {
            ToggleLeftWeaponForward();
        } else if (!leftGun)
        {
            leftGun = leftWeapons[currentLeftWeaponIndex].GetWeaponIfExists();
            EnableWeaponIfExists(leftGun);
        }

        if (!rightWeapons[currentRightWeaponIndex].Occupied)
        {
            ToggleRightWeapon();
        }
        else if (!rightGun)
        {
            rightGun = rightWeapons[currentRightWeaponIndex].GetWeaponIfExists();
            EnableWeaponIfExists(rightGun);
        }

        if (!leftGun)
        {
            playerAnimator.SetBool("Armed", false);
        } else
        {
            playerAnimator.SetBool("Armed", true);
            SetUpWeapon(leftGun);
        }
    }

    void EnableWeaponIfExists(Weapon weapon)
    {
        if (weapon != null)
        {
            weapon.gameObject.SetActive(true);
        }
    }

    void DisableWeaponIfExists(Weapon weapon)
    {
        if (weapon != null)
        {
            weapon.gameObject.SetActive(false);
        }
    }

    void SetUpWeapon(Weapon weapon)
    {
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

    void RevealWeapon()
    {
        if (leftGun)
        {
            if (FacingLeft())
            {
                leftGun.SetToLeftSide();
            } else
            {
                leftGun.SetToRightSide();
            }
        }
    }

    bool FacingLeft()
    {
        return VectorUtil.DirectionToMousePointer(transform).x < 0;
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
