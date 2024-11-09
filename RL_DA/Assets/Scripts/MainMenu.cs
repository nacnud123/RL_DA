using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using UnityEngine.Device;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject changeLog;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject credditsMenu;

    [Header("Video stuff")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject screen;

    private bool isChangeLogOpen = false;
    private bool isOptionsOpen = false;
    private bool isCredditsOpen = false;
    private bool isVideoOpen = false;

    private void Start()
    {
        if (!SaveManager.init.HasSaveAvailable())
        {
            continueButton.interactable = false;
        }
        else
        {
            eventSystem.SetSelectedGameObject(continueButton.gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("Play Video!");
            toggleVideo();
        }
    }

    public void NewGame()
    {
        if (SaveManager.init.HasSaveAvailable())
            SaveManager.init.DeleteSave();

        SaveManager.init.CurrentFloor = 1;
        SceneManager.LoadScene("Dungeon");
    }

    public void ContinueGame()
    {
        SaveManager.init.LoadGame();
    }

    public void QuitGame()
    {
        UnityEngine.Application.Quit();
    }

    public void viewChangeLog()
    {
        isChangeLogOpen = !isChangeLogOpen;
        changeLog.SetActive(isChangeLogOpen);
    }

    public void viewOptions()
    {
        isOptionsOpen = !isOptionsOpen;
        optionsMenu.SetActive(isOptionsOpen);
    }

    public void viewCreddits()
    {
        isCredditsOpen = !isCredditsOpen;
        credditsMenu.SetActive(isCredditsOpen);
    }

    private void toggleVideo()
    {
        isVideoOpen = !isVideoOpen;

        if (isVideoOpen)
        {
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.Stop();
        }

        screen.SetActive(isVideoOpen);
    }
}
