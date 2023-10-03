using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager init;

    [Header("Time")]
    [SerializeField] private float baseTime = .1f;
    [SerializeField] private float delayTime; // Read-only
    [SerializeField] private bool isPlayerTurn = true;

    [Header("Entities")]
    [SerializeField] private int actorNum = 0;
    [SerializeField] private List<Entity> entities = new List<Entity>();
    [SerializeField] private List<Actor> actors = new List<Actor>();

    [Header("Death")]
    [SerializeField] private Sprite deadSprite;

    // Gets
    public bool getIsPlayerTurn { get => isPlayerTurn; }
    public List<Entity> getEntities { get => entities; }
    public List<Actor> getActors { get => actors; }
    public Sprite getDeadSprite { get => deadSprite; }

    private void Awake()
    {
        if (init == null)
            init = this;
        else
            Destroy(gameObject);
    }

    private void StartTurn()
    {
        if (actors[actorNum].GetComponent<Player>())
            isPlayerTurn = true;
        else
        {
            if (actors[actorNum].GetComponent<HostileEnemy>())
                actors[actorNum].GetComponent<HostileEnemy>().RunAI();
            else
                Action.skipAction();
        }
    }

    public void endTurn()
    {
        if (actors[actorNum].GetComponent<Player>())
            isPlayerTurn = false;

        if (actorNum == actors.Count - 1)
            actorNum = 0;
        else
            actorNum += 1;

        StartCoroutine(turnDelay());
    }

    private IEnumerator turnDelay()
    {
        yield return new WaitForSeconds(delayTime);
        StartTurn();
    }

    public void addEntity(Entity entity)
    {
        if (!entity.gameObject.activeSelf)
        {
            entity.gameObject.SetActive(true);
        }

        entities.Add(entity);
    }

    public void removeEntity(Entity entity)
    {
        entity.gameObject.SetActive(false);
        entities.Remove(entity);
    }

    public void addActor(Actor actor)
    {
        actors.Add(actor);
        delayTime = setTime();
    }

    public void insertActor(Actor actor, int index)
    {
        actors.Insert(index, actor);
        delayTime = setTime();
    }

    public void removeActor(Actor actor)
    {
        actors.Remove(actor);
        delayTime = setTime();
    }

    public Actor GetBlockingActorAtLocation(Vector3 location)
    {
        foreach(Actor actor in actors)
        {
            if (actor.BlocksMovment && actor.transform.position == location)
                return actor;
        }
        return null;
    }

    private float setTime() => baseTime / actors.Count;
}
