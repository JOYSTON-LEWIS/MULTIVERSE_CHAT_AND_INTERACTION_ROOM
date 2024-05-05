using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 15.2 import
using Photon.Pun;
// 15.5
using TMPro;

namespace Player
{
    // 15.5 IPunObservable added
    public class Player : MonoBehaviour, IPunObservable
    {
        // 13.3 variable for character controller
        CharacterController characterController;
        // 13.3 cam variable will store transform of main function 
        Transform Cam;
        // 14.1 public variable animator
        public Animator animator;
        // 16.1
        bool chatBubbleSync;
        // 15.2 Instantiating Photon View added as component in editor
        // 15.2 within the script to access network functions
        PhotonView view;

        // 15.5
        public TMP_Text PlayerNameDisplay;

        // 16.1
        public GameObject ChatBubble;
        // 16.1
        public TMP_Text messageDisplay;

        // 16.2
        public float timerDuration = 5;
        // 16.2
        public float timerCounter;

        // 13.3 getting input in x axis
        float x;
        // 13.3  getting input in z axis
        float z;
        // 13.3 speed variable for character movement
        public float speed = 3;
        // 13.3 need vector 3 to get new direction
        // 13.3 will give direction of movement
        // 13.3 we will multiply speed with direction
        // 13.3  to move with a particular speed
        // 13.3  in a particular direction
        Vector3 move;

        // Start is called before the first frame update
        void Start()
        {
            // 16.2
            timerCounter = timerDuration;
            // 15.2 if view is not instantiated will give red error in console
            view = GetComponent<PhotonView>();
            // 15.2 If script is running on owner game object only then run this code.
            if (view.IsMine)
            {
                // 15.5 (this code removed in Backup Code with a comment but used in this video)
                PlayerNameDisplay.text = NetworkScript.playerName;

                // 16.1 - this is to hide own player name
                PlayerNameDisplay.gameObject.SetActive(false);  // if you want to enable the player name PlayerNameDisplay.text=NetworkScript.playerName;

                // 13.3 fetch controller using get component
                characterController = GetComponent<CharacterController>();
                // 13.4 locking the cursor to work with the plugin on player instantiation
                Cursor.lockState = CursorLockMode.Locked;
                // 13.3 initializing camera value
                Cam = Camera.main.transform;

                // 16.1 - was not in provided backup code
                // 16.1 removing this code from here adn placing it into
                // 16.1 chat bubble function and converting it into euler function
                // messageDisplay.transform.LookAt(Camera.main.transform);
            }
        }

        // Update is called once per frame
        void Update()
        {

            // 15.2 Logic that does the following:
            // 15.2 If IsMine is false, dont update anything skip the rest
            if (!view.IsMine)
            {
                // 15.5 I want the player to be looking at the camera
                // 15.5 This will ensure the player name is always facing
                // 15.5 the correct way to the camera of the second player
                // 15.5 hence he will be able to read it corrctly at all times
                PlayerNameDisplay.transform.parent.LookAt(Camera.main.transform);
                // 15.2
                return;
            }

            // 16.1 The code below this is already block of code which runs 
            // 16.1 when isMine is true as if its false it will break above itself
            // 16.1 if someone is typing, dont execute rest of the code
            if (NetworkScript.CheckChatBox.isFocused)
                return;

            // 13.3 calling function in update
            Movement();
            // 13.4 calling the function to work on every frame
            CameraSync();
            // 14.1 calling the Animations function at every frame
            Animations();
            // 16.1
            chatBubbleFunction();
        }

        void Movement()
        {
            // 13.3 keytrokes / from joystick are stored in x 
            // 13.3 x stores right left with respect to camera
            x = Input.GetAxis("Horizontal");
            // 13.3 keytrokes / from joystick are stored in z
            // 13.3 z stores forward backward with respect to camera
            z = Input.GetAxis("Vertical");

            // 13.3 x value is amount with which chaarcter should
            // 13.3 move right or left wrt camera
            // 13.3 x value is amount with which chaarcter should
            // 13.3 move forward or backward wrt camera
            // 13.3 move is giving direction of movement
            // 13.3 this multiplied by speed will give us displacement of chaarcter
            // 13.3 in that direction with that speed
            move = Cam.forward * z + Cam.right * x;
            // 13.3 move = direction with speed=speed bboth
            // 13.3 these give velocity now Time.deltatime = distance 
            characterController.Move(move * speed * Time.deltaTime);
            // 13.3 if player loses contact with ground so we need
            // 13.3 (virtual)gravity to act on Virtual gravity
            characterController.Move(Vector3.down * 5 * Time.deltaTime);
        }

        void CameraSync()
        {
            // 13.4 cameras parent position is equals to this parent objects position
            Cam.parent.position = transform.position;
            // 13.4 Now we want players rotation of player only in y axis
            // 13.4 so only for rotation in one axis, we use Quaternion
            // 13.4 first parameter will be angle which will be from camera
            // 13.4 the rotation values in editor are Euler angles
            // 13.4 so reference the y with Euler to access it
            transform.rotation = Quaternion.AngleAxis(Cam.parent.rotation.eulerAngles.y, Vector3.up);
        }

        void Animations()
        {
            // 14.1 if any button is pressed either x or z will not be 0
            if (x != 0 || z != 0)
                // 14.1 
                animator.SetBool("Motion", true);
            else
                // 14.1 
                animator.SetBool("Motion", false);

            // 14.2 Alpha1 is keyboard number key 1
            if (Input.GetKeyDown(KeyCode.Alpha1))
                // 14.2
                animator.SetBool("Hi", true);

            // 14.2 Alpha1 is keyboard number key 2
            if (Input.GetKeyDown(KeyCode.Alpha2))
                // 14.2
                animator.SetBool("Dance", true);
        }

        // 16.1
        void chatBubbleFunction()
        {
            // 16.1
            messageDisplay.transform.localRotation = Quaternion.Euler(0, 180, 0);

            // 16.1 if chat bubble is active in hierarchy , then copy the message
            // 16.1 this is to be done to only work if the chat bubble is active
            // 16.1 to avoid any error chances
            if (ChatBubble.activeInHierarchy)
                messageDisplay.text = NetworkScript.message;

            // 16.2
            if (NetworkScript.IsChatBubble)
            {
                // 16.2 it means times up
                if (timerCounter <= 0)
                {
                    // 16.2 turn off chatbubble
                    NetworkScript.IsChatBubble = false;
                    // 16.2 reset time counter
                    timerCounter = timerDuration;
                }
                // 16.2
                else
                {
                    // 16.2
                    timerCounter = timerCounter - Time.deltaTime;
                }
            }
            // 16.2 syncing chat bubble bool always
            chatBubbleSync = NetworkScript.IsChatBubble;
            // 16.2 locally setting the timer for ourself
            ChatBubble.SetActive(chatBubbleSync);
        }

        // 15.5
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // 15.5 this is executed on owners side meaning for isMine true people
            if (stream.IsWriting)
            {
                // 15.5 sending player name
                stream.SendNext(NetworkScript.playerName);

                // 16.1 order of this is important, if we send player name first
                // 16.1 we need to receive player name first and then send message second
                // 16.1 and receive message second
                stream.SendNext(NetworkScript.message);

                // 16.2
                stream.SendNext(chatBubbleSync);
            }
            // 15.5 this is executed on other users end who have isMine false 
            else if (stream.IsReading)
            {
                // 15.5 as we are receiving string we need to typecast as string
                PlayerNameDisplay.text = (string)stream.ReceiveNext();

                // 16.1
                messageDisplay.text = (string)stream.ReceiveNext();

                // 16.2 setting it false for other players
                ChatBubble.SetActive((bool)stream.ReceiveNext());
            }
        }

        

    }
}
