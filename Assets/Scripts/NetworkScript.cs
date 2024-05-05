using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 14.4 importing Photon Library
using Photon.Pun;
// Importing TextMeshPro
using TMPro;
using System;

// 14.4 we have this behaviour which is also called as
// 14.4 MonoBehaviourPunCallback function
public class NetworkScript : MonoBehaviourPunCallbacks
{
    // 14.4 Variable for Create Code
    public TMP_InputField CreateCodeInput;
    // 14.4 Variable for Join Code
    public TMP_InputField JoinCodeInput;
    // 15.5 For Player Name
    public TMP_InputField PlayerNameInput;
    // 16.1 For Chat Bubble
    public TMP_InputField ChatInput;
    // 14.4 Variables and gameobject references  for implementing loading screen
    public TMP_Text JoinCodeDisplayText;
    // 14.4 Variables and gameobject references  for implementing loading screen
    public GameObject MainMenuCanvas;
    // 14.4 Variables and gameobject references  for implementing loading screen
    public GameObject GameCanvas;
    // 14.4 Variables and gameobject references  for implementing loading screen
    public GameObject LoadingScreen;
    // 14.4 Variables and gameobject references  for implementing loading screen
    public GameObject Screen0;
    // 14.4 Variable for storing the join code string
    string joinCode;
    // 14.5 Variable to reference Player Prefab
    public GameObject PlayerPrefab;
    // 14.5 Player Position storing variable
    public Transform StartPosition;
    // 16.1 Variable added to sync with player script
    public static TMP_InputField CheckChatBox;
    // 15.5 we make variable static if we want to access them across the scripts
    public static string playerName = " player ";
    // 16.1
    public static string message = "";
    // 16.1 
    public static bool IsChatBubble = false;
    // 16.1
    public static bool IsInRoom = true;

    // Start is called before the first frame update
    // 14.4 On Start we want to disable other screens and show loading
    // 14.4 screen which si implemented here
    void Start()
    {
        // 16.1 assigning the value to the variable in start
        CheckChatBox = ChatInput;
        // 14.4 Variables and gameobject references  for implementing loading screen
        MainMenuCanvas.SetActive(true);
        // 16.1 To initially hide the chat input
        ChatInput.gameObject.SetActive(false);
        // 14.4 Variables and gameobject references  for implementing loading screen
        GameCanvas.SetActive(false);
        // 14.4 Variables and gameobject references  for implementing loading screen
        LoadingScreen.SetActive(true);
        // 14.4 Variables and gameobject references  for implementing loading screen
        Screen0.SetActive(false);
        // 14.4 This will run the code to connect to server
        PhotonNetwork.ConnectUsingSettings();
    }

    // 14.4 Once we connect to server, we connect to lobby
    // 14.4 We can either create the room or join the room
    // 14.4 so lets create functions for the same
    public override void OnConnectedToMaster()
    {
        // 14.4 so lets create functions for the same
        PhotonNetwork.JoinLobby();
    }

    // 14.4 just like start and update for mono behaviour
    // 14.4 we have this function
    public override void OnJoinedLobby()
    {
        // 14.4 Enable Screen0 and disable loading screen0
        // 14.4 Variables and gameobject references  for implementing loading screen
        LoadingScreen.SetActive(false);
        // 14.4 Variables and gameobject references  for implementing loading screen
        Screen0.SetActive(true);
    }

    // 14.4 Create Room Function
    public void CreateRoom()
    {

        // 14.4 Creating Room takes time so loading screen again and screen0 off
        // 14.4 Variables and gameobject references  for implementing loading screen
        LoadingScreen.SetActive(true);
        // 14.4 Variables and gameobject references  for implementing loading screen
        Screen0.SetActive(false);
        // 14.4 Assign value of Create Code with inputted code
        joinCode = CreateCodeInput.text;
        // 14.4 creating a room with the code
        PhotonNetwork.CreateRoom(joinCode);
    }

    // 14.4 Join Room Function
    public void JoinRoom()
    {
        // 14.4 Joining Room takes time so loading screen again and screen0 off
        // 14.4 Variables and gameobject references  for implementing loading screen
        LoadingScreen.SetActive(true);
        // 14.4 Variables and gameobject references  for implementing loading screen
        Screen0.SetActive(false);
        // 14.4 Assign value of Join Code with inputted code
        joinCode = JoinCodeInput.text;

        // 14.4 joining a room with the code
        PhotonNetwork.JoinRoom(joinCode);
    }

    // 14.4 just like start and update for mono behaviour
    // 14.4 we have this function
    public override void OnJoinedRoom()
    {
        // 16.1
        IsInRoom = true;
        // 14.4 Variables and gameobject references  for implementing loading screen
        LoadingScreen.SetActive(false);
        // 14.4 Variables and gameobject references  for implementing loading screen
        MainMenuCanvas.SetActive(false);
        // 14.4 Variables and gameobject references  for implementing loading screen
        GameCanvas.SetActive(true);
        // 14.4 Variables and gameobject references  for implementing loading screen
        JoinCodeDisplayText.text = "Join Code: " + joinCode;

        // 15.5
        playerName = PlayerNameInput.text;

        // 14.5 create a folder named resources with R capital
        // 14.5 To avoid typo mistakes, take Prefab variable which contains the name
        PhotonNetwork.Instantiate(PlayerPrefab.name, StartPosition.position, StartPosition.rotation);
    }

    // 16.1 We want to capture message once user types message and hits enter
    // 16.1 for this there is callback function in Message Box we have on UI
    public void getMessage()
    {
        // 16.1
        message = ChatInput.text;
        // 16.1
        IsChatBubble = true;
        // 16.1 toclear the chat input once enter is hit
        ChatInput.text = "";
        // 16.1 to remove the chat bubble once message is done and sent
        // 16.1 same will be implemented to open the chat bubble
        ChatInput.gameObject.SetActive(false);
    }

    // 16.3
    public void LeaveRoom()
    {
        // 16.3
        PhotonNetwork.LeaveRoom();
        // 16.3 turn off game canvas while leaving room
        GameCanvas.SetActive(false);
        // 16.3 turn on Main Screen
        MainMenuCanvas.SetActive(true);
        // 16.3 turn on Loading Screen
        LoadingScreen.SetActive(true);
    }

    // 16.3
    public override void OnLeftRoom()
    {
        // 16.3
        base.OnLeftRoom();
        // 16.3 once we confirm that we left
        IsInRoom = false;
        // 16.3
        LoadingScreen.SetActive(false);
        // 16.3 back to screen0
        Screen0.SetActive(true);
        // BugFix to get the mouse back once we exit the room
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        // 16.1 if keyboard key is Q do the following 
        // 16.1 && IsInRoom added midway of video to run 
        // 16.1 this only when player is in room and not 
        // 16.1 run when  creating or joining lobby
        if (Input.GetKeyDown(KeyCode.Q) && IsInRoom)
        {
            // 16.1
            ChatInput.gameObject.SetActive(true);
            // 16.1 to focus cursor on chat bubble as we dont have our mouse ingame
            ChatInput.Select();
            // 16.1 and it will be activated
            ChatInput.ActivateInputField();
        }

        // 16.3 check if we are chatting or not
        // 16.3 if not then only leave the room
        // 16.3 should work
        // 16.3 Keyboard key E pressed
        // 16.3 Chat is not active
        // 16.3 We are in a room
        // 16.3 Only then run this logic
        // Original Code
        // has a bug it runs even though not in room
        // To fix it using PhotonNetwork property of checking is in Room
        // if (Input.GetKeyDown(KeyCode.E) && !ChatInput.isFocused && IsInRoom)
        // {
             // 16.3
             // LeaveRoom();
        // }

        if (Input.GetKeyDown(KeyCode.E) && !ChatInput.isFocused && PhotonNetwork.CurrentRoom != null)
        {
            // 16.3
            LeaveRoom();
        }
    }
}
