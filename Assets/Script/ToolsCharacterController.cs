using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Timeline;

public class ToolsCharacterController : MonoBehaviour
{
    PlayerMovement character;
    Rigidbody2D rgbd2d;
    ToolbarController toolbarController;
    Animator animator;
    [SerializeField] float offsetDistance = 1f;
    [SerializeField] MarkerManager markerManager;
    [SerializeField] TileMapReadController tileMapReadController;
    [SerializeField] float maxDistance = 1.5f;
    [SerializeField] ToolAction onTilePickUp;

    Vector3Int selectedTilePosition;
    bool selectable;



    private void Awake()
    {
        character = GetComponent<PlayerMovement>(); // Thêm dòng này để khởi tạo character
        rgbd2d = GetComponent<Rigidbody2D>(); // Thêm dòng này để khởi tạo rgbd2d
        toolbarController = GetComponent<ToolbarController>();
        animator = GetComponent<Animator>();
    }


    private void Update()
    {
        SelectTile();
        CanSelectCheck();
        Marker();
        if (Input.GetMouseButtonDown(0))
        {
            UseToolWorld();
            UseToolGrid();
        }
    }

    private void SelectTile()
    {
        selectedTilePosition = tileMapReadController.GetGridPosition(Input.mousePosition, true);
    }

    void CanSelectCheck()
    {
        Vector2 characterPosition = transform.position;
        Vector2 cameraPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        selectable = Vector2.Distance(characterPosition, cameraPosition) < maxDistance;
        markerManager.Show(selectable);
    }

    private void Marker()
    {
        markerManager.markedCellPosition = selectedTilePosition;
    }


    private bool UseToolWorld()
    {
        Vector2 position = rgbd2d.position + character.lastMovement * offsetDistance;
        
        Item item = toolbarController.GetItem;
        if (item == null) { return false; }
        if (item.onAction == null) { return false; }

        animator.SetTrigger("act");
        bool complete = item.onAction.OnApply(position);

        if (complete == true)
        {
            if (item.onItemUsed != null)
            {
                item.onItemUsed.OnItemUsed(item, GameManager.instance.inventoryContainer);
            }
        }

        return complete;
    }

    private void UseToolGrid()
    {
        if (selectable == true)
        {
            Item item = toolbarController.GetItem;
            if (item == null)
            {
                PickUpTile();
                return;
            }
            if(item.onTileMapAction == null)
            {
                return;
            }

            animator.SetTrigger("act");
            bool complete = item.onTileMapAction.OnApplyToTileMap(selectedTilePosition, tileMapReadController,item);
        
            if (complete == true)
            {
                if (item.onItemUsed != null)
                {
                    item.onItemUsed.OnItemUsed(item, GameManager.instance.inventoryContainer);
                }
            }
        }
    }

    private void PickUpTile()
    {
        if (onTilePickUp == null) { return; }
        onTilePickUp.OnApplyToTileMap(selectedTilePosition, tileMapReadController, null);
    }
}
