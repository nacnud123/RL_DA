using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, Controls.IPlayerActions
{
    private Controls controls;
    [SerializeField] private bool moveKeyHeld;

    private void Awake() => controls = new Controls();

    private void OnEnable()
    {
        controls.Player.SetCallbacks(this);
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.SetCallbacks(null);
        controls.Player.Disable();
    }


    void Controls.IPlayerActions.OnMovement(InputAction.CallbackContext context)
    {
        if (context.started)
            moveKeyHeld = true;
        else if (context.canceled)
            moveKeyHeld = false;
    }

    void Controls.IPlayerActions.OnExit(InputAction.CallbackContext context)
    {
        if (context.performed)
            UIManager.init.toggleMenu();
    }

    public void OnView(InputAction.CallbackContext context)
    {
        if (context.performed)
            if(!UIManager.init.GetIsMenuOpen || UIManager.init.GetIsMsgHistoryOpen)
                UIManager.init.toggleMsgHistory();
    }

    public void OnPickup(InputAction.CallbackContext context)
    {
        if (context.performed)
            Action.pickupAction(GetComponent<Actor>());
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(!UIManager.init.GetIsMenuOpen || UIManager.init.GetIsInvOpen)
            {
                if (GetComponent<Inventory>().GetItems.Count > 0)
                    UIManager.init.toggleInv(GetComponent<Actor>());
                else
                    UIManager.init.addMsg("You have no items", "#808080");
            }
        }
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(!UIManager.init.GetIsMenuOpen || UIManager.init.GetIsDropMenuOpen)
            {
                if (GetComponent<Inventory>().GetItems.Count > 0)
                    UIManager.init.toggleDropMenu(GetComponent<Actor>());
                else
                    UIManager.init.addMsg("You have no items", "#808080");
            }
        }
    }



    private void FixedUpdate()
    {
        if (!UIManager.init.GetIsMenuOpen)
        {
            if (GameManager.init.getIsPlayerTurn && moveKeyHeld && GetComponent<Actor>().IsAlive)
                movePlayer();
        }
    }

    private void movePlayer()
    {
        Vector2 dir = controls.Player.Movement.ReadValue<Vector2>();
        Vector2 roundedDir = new Vector2(Mathf.Round(dir.x), Mathf.Round(dir.y)); // Used to fix some bugs with directional movment
        Vector3 futurePos = transform.position + (Vector3)roundedDir;

        if (isValidPos(futurePos))
            moveKeyHeld = Action.bumpAction(GetComponent<Actor>(), roundedDir);
    }

    private bool isValidPos(Vector3 futurePos)
    {
        Vector3Int gridPos = MapManager.init.getFloorMap.WorldToCell(futurePos);
        if (!MapManager.init.inBounds(gridPos.x, gridPos.y) || MapManager.init.getObstacleMap.HasTile(gridPos) || futurePos == transform.position)
            return false;

        return true;
    }
}
