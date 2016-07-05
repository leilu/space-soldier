﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryTile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    private Transform slotTransform;
    private Vector2 pointerOffset;
    private RectTransform tileRectTransform;
    private RectTransform canvasRectTransform;
    private List<RectTransform> slotRects;
    private float rectWidth;
    private float rectHeight;
    private RectTransform mostOverlappedRect;
    private RectTransform rectTransform;
    private Weapon weapon;
    private Tooltip tooltip;
    public SkillType skillType;
    private Vector3[] corners;

    private bool pointerDown = false;
    private bool pointerIn = false;

    public void Init(Transform slotTransform, List<RectTransform> slotRects, Weapon weapon, Tooltip tooltip, SkillType skillType)
    {
        this.slotTransform = slotTransform;
        this.slotRects = slotRects;
        this.weapon = weapon;
        this.tooltip = tooltip;
        this.skillType = skillType;
        tileRectTransform = transform as RectTransform;
        canvasRectTransform = GetComponentInParent<Canvas>().transform as RectTransform;
        rectTransform = GetComponent<RectTransform>();

        corners = new Vector3[4];
        tileRectTransform.GetWorldCorners(corners);
        // RectTransform doesn't give a way to get width/height in world units, so must hack it.
        rectWidth = corners[2].x - corners[0].x;
        rectHeight = corners[2].y - corners[0].y;
    }

	public void OnPointerDown (PointerEventData data)
    {
        pointerDown = true;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(tileRectTransform, data.position, data.pressEventCamera, out pointerOffset);
        tooltip.Hide();
    }

    public void OnDrag (PointerEventData data)
    {
        Vector2 localPointerPosition;
        // Canvas rectangle != Screen rectangle.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, data.position, data.pressEventCamera, out localPointerPosition);

        tileRectTransform.localPosition = ClampToWindow(localPointerPosition - pointerOffset);
        HandleTileOverlap();
    }

    void HandleTileOverlap()
    {
        float maxArea = 0;
        mostOverlappedRect = null;
        for (int i = 0; i < slotRects.Count; i++)
        {
            RectTransform rect = slotRects[i];
            // Could probably optimize this by caching the previous rect but... fuck it. Performs fine already.
            rect.GetComponent<Image>().color = Color.grey;
            if (BoxesCollide(rect, tileRectTransform))
            {
                float collisionArea = CollisionArea(rect, tileRectTransform);
                if (collisionArea > maxArea)
                {
                    maxArea = collisionArea;
                    mostOverlappedRect = rect;
                }
            }
        }

        if (mostOverlappedRect)
        {
            mostOverlappedRect.GetComponent<Image>().color = Color.green;
        }
    }

    public void OnPointerUp (PointerEventData data)
    {
        if (mostOverlappedRect)
        {
            InventorySlot currSlot = slotTransform.GetComponent<InventorySlot>();
            InventorySlot newSlot = mostOverlappedRect.transform.GetComponent<InventorySlot>();

            if (!newSlot.Occupied && newSlot.CanHostSkillType(skillType))
            {
                MoveTile(currSlot, newSlot);

                Player.PlayerWeaponControl.ReconfigureWeapons();

                TriggerTutorialEventsIfNecessary();

                slotTransform = mostOverlappedRect.transform;
            } else
            {
                mostOverlappedRect.GetComponent<Image>().color = Color.grey;
            }
        }

        transform.position = slotTransform.position;
        pointerDown = false;

        if (CursorIsOnTile())
        {
            tooltip.Render(rectTransform, weapon);
        }
    }

    bool CursorIsOnTile()
    {
        return Input.mousePosition.x >= corners[0].x && Input.mousePosition.x <= corners[2].x
            && Input.mousePosition.y >= corners[0].y && Input.mousePosition.y <= corners[2].y;
    }

    void MoveTile(InventorySlot oldSlot, InventorySlot newSlot)
    {
        oldSlot.UnsetTile();
        newSlot.SetTile(this);
    }

    void TriggerTutorialEventsIfNecessary()
    {
        if (GameState.TutorialMode)
        {
            if (weapon.name == "Pistol"
                && Player.PlayerWeaponControl.HasGun(PlayerWeaponControl.WeaponSide.Left, "Pistol"))
            {
                TutorialEngine.Instance.Trigger(TutorialTrigger.EquipLaserPistol);
            }

            if ((weapon.name == "Pistol" || weapon.name == "MachineGun")
                && Player.PlayerWeaponControl.HasGun(PlayerWeaponControl.WeaponSide.Left, "Pistol")
                && Player.PlayerWeaponControl.HasGun(PlayerWeaponControl.WeaponSide.Left, "MachineGun"))
            {
                // This will also fire if the player just moves around the left items, but it doesn't matter.
                TutorialEngine.Instance.Trigger(TutorialTrigger.SecondLeftWeaponEquipped);
            }
        }
    }

    Vector2 ClampToWindow (Vector2 pos)
    {
        Vector3[] canvasCorners = new Vector3[4];
        canvasRectTransform.GetLocalCorners(canvasCorners); // 0: lower left, 1: upper left, 2: upper right, 3: lower right

        float halfTileLength = InventoryManager.TileSideLength / 2;

        float clampedX = Mathf.Clamp(pos.x, canvasCorners[0].x + halfTileLength, canvasCorners[2].x - halfTileLength);
        float clampedY = Mathf.Clamp(pos.y, canvasCorners[0].y + halfTileLength, canvasCorners[2].y - halfTileLength);

        return new Vector2(clampedX, clampedY);
    }

    private float CollisionArea(RectTransform box1, RectTransform box2)
    {
        // Can check centers instead of edges because these are all squares of the same size.
        float xMax = box1.position.x > box2.position.x ? box2.position.x + rectWidth / 2 : box1.position.x + rectWidth / 2;
        float xMin = box1.position.x > box2.position.x ? box1.position.x - rectWidth / 2 : box2.position.x - rectWidth / 2;
        float yMax = box1.position.y > box2.position.y ? box2.position.y + rectHeight / 2 : box1.position.y + rectHeight / 2;
        float yMin = box1.position.y > box2.position.y ? box1.position.y - rectHeight / 2 : box2.position.y - rectHeight / 2;

        return (xMax - xMin) * (yMax - yMin);
    }

    private bool BoxesCollide(RectTransform box1, RectTransform box2)
    {
        return Mathf.Abs(box1.position.x - box2.position.x) < (rectWidth + rectWidth) / 2 &&
            Mathf.Abs(box1.position.y - box2.position.y) < (rectHeight + rectHeight) / 2;
    }

    public Weapon GetWeapon()
    {
        return weapon;
    }

    public void SetWeapon(Weapon weapon)
    {
        this.weapon = weapon;
    }

    public void OnPointerEnter (PointerEventData data)
    {
        pointerIn = true;
        if (pointerDown)
        {
            return;
        }

        tooltip.Render(rectTransform, weapon);
    }

    public void OnPointerExit (PointerEventData data)
    {
        pointerIn = false;
        tooltip.Hide();
    }
}
