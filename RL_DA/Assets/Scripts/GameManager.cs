using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using SysRandom = System.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager init;

    private static SysRandom rand = new SysRandom();

    [Header("Time")]
    [SerializeField] private float baseTime = .1f;
    [SerializeField] private float delayTime; // Read-only
    [SerializeField] private bool isPlayerTurn = true;

    [Header("Entities")]
    [SerializeField] private int actorNum = 0;
    [SerializeField] private List<Entity> entities;
    [SerializeField] private List<Actor> actors;

    [Header("Death")]
    [SerializeField] private Sprite deadSprite;

    // Gets
    public bool getIsPlayerTurn { get => isPlayerTurn; }
    public List<Entity> getEntities { get => entities; }
    public List<Actor> getActors { get => actors; }
    public Sprite getDeadSprite { get => deadSprite; }

    static string[] syllables = {
        "blech ","foo ","barf ","rech ","bar ",
        "blech ","quo ","bloto ","oh ","caca ",
        "blorp ","erp ","festr ","rot ","slie ",
        "snorf ","iky ","yuky ","ooze ","ah ",
        "bahl ","zep ","druhl ","flem ","behil ",
        "arek ","mep ","zihr ","grit ","kona ",
        "kini ","ichi ","tims ","ogr ","oo ",
        "ighr ","coph ","swerr ","mihln ","poxi "
    };

    private void Awake()
    {
        if (init == null)
            init = this;
        else
            Destroy(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneState sceneState = SaveManager.init.Save.Scenes.Find(x => x.FloorNumber == SaveManager.init.CurrentFloor);

        if(sceneState is not null)
        {
            LoadState(sceneState.GameState, true);
        }
        else
        {
            entities = new List<Entity>();
            actors = new List<Actor>();
        }
    }

    private void StartTurn()
    {
        if (actors[actorNum].GetComponent<Player>())
        {
            isPlayerTurn = true;
            if (actors[actorNum].GetComponent<Player>().NumSlTurns > 0)
            {
                actors[actorNum].GetComponent<Player>().NumSlTurns -= 1;
                UIManager.init.addMsg("You are asleep!", "#00ff00");
                Action.waitAction();
            }
            if(actors[actorNum].GetComponent<Player>().PoisTurns > 0)
            {
                actors[actorNum].GetComponent<Fighter>().Hp -= 2;
                actors[actorNum].GetComponent<Player>().PoisTurns -= 1;
            }
            if(actors[actorNum].GetComponent<Player>().ConfTurns > 0)
            {
                actors[actorNum].GetComponent<Player>().ConfTurns -= 1;
            }
        }
        else
        {
            if (actors[actorNum].AI != null)
                actors[actorNum].AI.RunAI();
            else
                Action.waitAction();
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

    public void InsertEntity(Entity entity, int index)
    {
        if (!entity.gameObject.activeSelf)
        {
            entity.gameObject.SetActive(true);
        }
        entities.Insert(index, entity);
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

    public void RefreshPlayer()
    {
        actors[0].updateFOV();
    }

    public Actor GetActorAtLocation(Vector3 location)
    {
        foreach(Actor actor in actors)
        {
            if (actor.BlocksMovment && actor.transform.position == location)
                return actor;
        }
        return null;
    }

    public Actor getNPCAtLocation(Vector3 loc)
    {
        foreach(Actor a in actors)
        {
            if (a.BlocksMovment && a.GetComponent<NPC>() && a.transform.position == loc)
                return a;
        }
        return null;
    }

    private float setTime() => baseTime / actors.Count;

    public GameState SaveState()
    {
        foreach(Item item in actors[0].GetInventory.GetItems)
        {
            addEntity(item);
        }

        GameState gameState = new GameState(_entities: entities.ConvertAll(x => x.SaveState()));

        foreach (Item item in actors[0].GetInventory.GetItems)
            removeEntity(item);

        return gameState;
    }

    public void LoadState(GameState state, bool canRemovePlayer)
    {
        isPlayerTurn = false; // Stops the player from moving until everything is loaded
        Reset(canRemovePlayer);
        StartCoroutine(LoadEntityStates(state.Entities, canRemovePlayer));
    }

    public string makeRandName()
    {
        int sylls = Random.Range(2, 5);
        string ti = "";
        for (int j = 0; j < sylls; j++)
        {
            int s = Random.Range(0, syllables.Length);
            ti += syllables[s];
        }
        return ti;
    } 
    
    public string makeRandName(string inputStr)
    {
        var list = new SortedList<int, char>();
        foreach (var c in inputStr)
            list.Add(rand.Next(), c);
        return new string(list.Values.ToArray());
    }

    private IEnumerator LoadEntityStates(List<EntityState> entityStates, bool canPlacePlayer)
    {
        int entityNum = 0;
        while(entityNum < entityStates.Count)
        {
            yield return new WaitForEndOfFrame();
            

            if(entityStates[entityNum].Type == EntityState.EntityType.Actor)
            {
                string entityName = entityStates[entityNum].Name.Contains("Remains of") ? entityStates[entityNum].Name.Substring(entityStates[entityNum].Name.LastIndexOf(' ') + 1) : entityStates[entityNum].Name;

                ActorState actorState = entityStates[entityNum] as ActorState;

                if(entityName == "Player" && !canPlacePlayer)
                {
                    actors[0].transform.position = entityStates[entityNum].Position;
                    RefreshPlayer();
                    entityNum++;
                    continue;
                }

                Actor actor = MapManager.init.createEntity(entityName, actorState.Position).GetComponent<Actor>();

                actor.LoadState(actorState);
            }
            else if(entityStates[entityNum].Type == EntityState.EntityType.Item)
            {
                string entityName = entityStates[entityNum].Name.Contains("(E)") ? entityStates[entityNum].Name.Replace(" (E)", "") : entityStates[entityNum].Name;
                ItemState itemState = entityStates[entityNum] as ItemState;

                if(itemState.Parent == "Player" && !canPlacePlayer)
                {
                    entityNum++;
                    continue;
                }

                Item item = MapManager.init.createEntity(entityName, itemState.Position).GetComponent<Item>();

                item.LoadState(itemState);
            }
            entityNum++;
        }
        isPlayerTurn = true;
    }

    public void Reset(bool canRemovePlayer)
    {
        if (entities.Count > 0)
        {
            foreach (Entity e in entities)
            {
                if(!canRemovePlayer && e.GetComponent<Player>())
                {
                    continue;
                }

                Destroy(e.gameObject);
            }

            if (canRemovePlayer)
            {
                entities.Clear();
                actors.Clear();
            }
            else
            {
                entities.RemoveRange(1, entities.Count - 1);
                actors.RemoveRange(1, actors.Count - 1);
            }

            
        }
    }
}

[System.Serializable]
public class GameState
{
    [SerializeField] private List<EntityState> entities;

    public List<EntityState> Entities { get => entities; set => entities = value; }

    public GameState(List<EntityState> _entities)
    {
        entities = _entities;
    }
}
