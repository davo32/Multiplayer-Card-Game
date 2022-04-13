using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Networking;
namespace Project.Scenes
{
    public class MainMenuScene : MonoBehaviour
    {
        public void JoinGameRoom()
        {
            NetworkClient.mInstance.JoinGameRoom();
        }
    }
}
