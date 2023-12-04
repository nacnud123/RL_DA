using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OdinSerializer;
using UnityEngine.SceneManagement;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager init;

    [SerializeField] private int currentFloor = 0;
    [SerializeField] private string saveFileName = "saveThe.koala";
    [SerializeField] private SaveData save = new SaveData();

    public int CurrentFloor { get => currentFloor; set => currentFloor = value; }
    public SaveData Save { get => save; set => save = value; }


    private void Awake()
    {
        if (SaveManager.init == null)
        {
            SaveManager.init = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool HasSaveAvailable()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        if (!File.Exists(path))
            return false;
        return true;

    }

    public void SaveGame(bool tempSave = true)
    {
        save.SavedFloor = currentFloor;

        bool hasScene = save.Scenes.Find(x => x.FloorNumber == currentFloor) is not null;
        if (hasScene)
            UpdateScene(SaveState());
        else
            AddScene(SaveState());

        if (!tempSave) return;

        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        byte[] saveJson = SerializationUtility.SerializeValue(save, DataFormat.JSON);
        File.WriteAllBytes(path, saveJson);
    }

    public void LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        byte[] saveJson = File.ReadAllBytes(path);
        save = SerializationUtility.DeserializeValue<SaveData>(saveJson, DataFormat.JSON);

        currentFloor = save.SavedFloor;

        if (SceneManager.GetActiveScene().name is not "Dungeon")
            SceneManager.LoadScene("Dungeon");
        else
        {
            LoadScene();
        }
    }

    public void DeleteSave()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        File.Delete(path);
    }

    public void AddScene(SceneState sceneState) => save.Scenes.Add(sceneState);

    public void UpdateScene(SceneState sceneState) => save.Scenes[currentFloor - 1] = sceneState;

    public void LoadScene(bool canRemovePlayer = true)
    {
        SceneState sceneState = save.Scenes.Find(x => x.FloorNumber == currentFloor);
        if (sceneState is not null)
            LoadState(sceneState, canRemovePlayer);
        else
            Debug.LogError("No save data for this floor");
    }

    public SceneState SaveState() => new SceneState(
        currentFloor,
        GameManager.init.SaveState(),
        MapManager.init.SaveState()
        );

    public void LoadState(SceneState sceneState, bool canRemovePlayer)
    {
        MapManager.init.LoadState(sceneState.MapState);
        GameManager.init.LoadState(sceneState.GameState, canRemovePlayer);
    }
}

[System.Serializable]
public class SaveData
{
    [SerializeField] private int savedFloor;
    [SerializeField] private List<SceneState> scenes;

    public int SavedFloor { get => savedFloor; set => savedFloor = value; }
    public List<SceneState> Scenes { get => scenes; set => scenes = value; }

    public SaveData()
    {
        savedFloor = 0;
        scenes = new List<SceneState>();
    }
}

[System.Serializable]
public class SceneState
{
    [SerializeField] private int floorNumber;
    [SerializeField] private GameState gameState;
    [SerializeField] private MapState mapState;

    public int FloorNumber { get => floorNumber; set => floorNumber = value; }
    public GameState GameState { get => gameState; set => gameState = value; }
    public MapState MapState { get => mapState; set => mapState = value; }

    public SceneState(int _floorNumber, GameState _gameState, MapState _mapState)
    {
        floorNumber = _floorNumber;
        gameState = _gameState;
        mapState = _mapState;
    }
}