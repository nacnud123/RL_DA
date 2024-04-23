using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Actor))]
public class Player : MonoBehaviour, Controls.IPlayerActions
{
    private Controls controls;
    [SerializeField] private bool moveKeyDown;
    [SerializeField] private bool targetMode;
    [SerializeField] private bool isSingleTarget;
    [SerializeField] private GameObject targetObj;
    [SerializeField] private int numSleepTurns = 0;
    [SerializeField] private int poisionTurns = 0;
    [SerializeField] private int confusionTurns = 0;
    [SerializeField] private bool healthRegen = false;

    public int NumSlTurns { get => numSleepTurns; set => numSleepTurns = value; }
    public int PoisTurns { get => poisionTurns; set => poisionTurns = value; }
    public int ConfTurns { get => confusionTurns; set => confusionTurns = value; }
    public bool HealthRegen { get => healthRegen; set => healthRegen = value; }

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
        if (context.started && GetComponent<Actor>().IsAlive)
        {
            if (targetMode && !moveKeyDown)
            {
                moveKeyDown = true;
                Move();
            }
            else if (!targetMode)
            {
                moveKeyDown = true;
            }
        }
        else if (context.canceled)
            moveKeyDown = false;


    }

    void Controls.IPlayerActions.OnExit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (targetMode)
            {
                ToggleTargetMode();
            }
            else if(!UIManager.init.GetIsEscMenuOpen && !UIManager.init.GetIsMenuOpen)
            {
                UIManager.init.toggleEscMenu();
            }
            else if (UIManager.init.GetIsMenuOpen)
            {
                UIManager.init.toggleMenu();
            }
        }

    }

    public void OnView(InputAction.CallbackContext context)
    {
        if (context.performed)
            if (!UIManager.init.GetIsMenuOpen || UIManager.init.GetIsMsgHistoryOpen)
                UIManager.init.toggleMsgHistory();
    }

    public void OnPickup(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CanAct())
            {
                Action.pickupAction(GetComponent<Actor>());
            }
        }

    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CanAct() || UIManager.init.GetIsInvOpen)
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
            if (CanAct() || UIManager.init.GetIsDropMenuOpen)
            {
                if (GetComponent<Inventory>().GetItems.Count > 0)
                    UIManager.init.toggleDropMenu(GetComponent<Actor>());
                else
                    UIManager.init.addMsg("You have no items", "#808080");
            }
        }
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (targetMode)
            {
                if (isSingleTarget)
                {
                    Actor target = SingleTargetChecks(targetObj.transform.position);

                    if (target != null)
                        Action.CastAction(GetComponent<Actor>(), target, GetComponent<Inventory>().SelectedConsumable);
                }
                else
                {
                    List<Actor> targets = AreaTargetChecks(targetObj.transform.position);

                    if (targets != null)
                        Action.CastAction(GetComponent<Actor>(), targets, GetComponent<Inventory>().SelectedConsumable);
                }
            }
            else if (CanAct())
            {
                Action.TakeStairsAction(GetComponent<Actor>());
            }
        }
    }

    public void OnInfo(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CanAct() || UIManager.init.GetIsCharMenuOpen)
            {
                UIManager.init.toggleCharInfoMenu(GetComponent<Actor>());
            }
        }
    }

    public void OnTalk(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CanAct())
            {
                Action.TalkAction(GetComponent<Actor>());
            }
        }
    }

    public void OnId(InputAction.CallbackContext context)
    {
    }

    public void ToggleTargetMode(bool isArea = false, int radius = 1)
    {
        targetMode = !targetMode;

        if (targetMode)
        {
            if (targetObj.transform.position != transform.position)
                targetObj.transform.position = transform.position;

            if (isArea)
            {
                isSingleTarget = false;
                targetObj.transform.GetChild(0).localScale = Vector3.one * (radius + 1);
                targetObj.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                isSingleTarget = true;
            }

            targetObj.SetActive(true);
        }
        else
        {
            if (targetObj.transform.GetChild(0).gameObject.activeSelf)
                targetObj.transform.GetChild(0).gameObject.SetActive(false);
            targetObj.SetActive(false);
            GetComponent<Inventory>().SelectedConsumable = null;
        }
    }


    private void FixedUpdate()
    {
        if (!UIManager.init.GetIsMenuOpen && !targetMode)
        {
            if (GameManager.init.getIsPlayerTurn && moveKeyDown && GetComponent<Actor>().IsAlive)
                Move();
        }
    }

    private void Move()
    {
        Vector2 dir = controls.Player.Movement.ReadValue<Vector2>();
        Vector2 roundedDir = new Vector2(Mathf.Round(dir.x), Mathf.Round(dir.y)); // Used to fix some bugs with directional movment
        Vector3 futurePos;

        if (targetMode)
        {
            futurePos = targetObj.transform.position + (Vector3)roundedDir;
        }
        else
            futurePos = transform.position + (Vector3)roundedDir;


        if (targetMode)
        {
            Vector3Int targetGridPos = MapManager.init.getFloorMap.WorldToCell(futurePos);

            if (MapManager.init.isValidPos(futurePos) && GetComponent<Actor>().getFOV.Contains(targetGridPos))
                targetObj.transform.position = futurePos;
        }
        else
        {
            if(confusionTurns > 0)
            {
                moveKeyDown = Action.bumpAction(GetComponent<Actor>(), -roundedDir);
                confusionTurns -= 1;
            }
            else
            {
                moveKeyDown = Action.bumpAction(GetComponent<Actor>(), roundedDir);
            }
            
        }


    }

    public bool CanAct()
    {
        if (targetMode || UIManager.init.GetIsMenuOpen || !GetComponent<Actor>().IsAlive)
            return false;
        else
            return true;
    }

    private Actor SingleTargetChecks(Vector3 targetPos)
    {
        Actor target = GameManager.init.GetActorAtLocation(targetPos);

        if(target == null)
        {
            UIManager.init.addMsg("You must select an enemy to target.", "#ffffff"); ;
            return null;
        }

        if(target == GetComponent<Actor>())
        {
            UIManager.init.addMsg("You can't target yourself.", "#ffffff");
            return null;
        }

        return target;
    }

    private List<Actor> AreaTargetChecks(Vector3 targetPos)
    {
        int radius = (int)targetObj.transform.GetChild(0).localScale.x - 1;

        Bounds targetBounds = new Bounds(targetPos, Vector3.one * radius * 2);
        List<Actor> targets = new List<Actor>();

        foreach(Actor target in GameManager.init.getActors)
        {
            if (targetBounds.Contains(target.transform.position))
                targets.Add(target);
        }

        if(targets.Count == 0)
        {
            UIManager.init.addMsg("There are no targets in the radius.", "#ffffff");
            return null;
        }

        return targets;
    }

   

}
