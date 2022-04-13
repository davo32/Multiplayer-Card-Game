using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerIOClient;
using System;

namespace Project.Networking
{
    public class NetworkClient
    {
        public static NetworkClient mInstance;
        public static string gameid = "novatales-qtx63bohukonxmvjetg0q";

        private Client Client;
        public static NetworkClient getInstance
        {
            get 
            {
                if (mInstance == null)
                {
                    mInstance = new NetworkClient();
                }
                return mInstance;
            }
        }

        public void RegisterUser(string username, string password)
        {
            PlayerIO.Authenticate(gameid,"public",new Dictionary<string, string> 
            {
                {"register","true"},
                {"username",username},
                {"password",password}
            },null,delegate(Client client)
            {
                Debug.Log("Registration Successfull");
            },delegate(PlayerIOError error)
            {
                Debug.Log("Registration Failed: " + error.Message);
            });
        }

        public void LoginUser(string username, string password)
        {
            PlayerIO.Authenticate(gameid, "public", new Dictionary<string, string>
            {
                {"username",username },
                {"password",password }
            }, null, delegate (Client _client)
              {
                  Client = _client;
                  JoinLobbyRoom();
                  Debug.Log("Logged in Successfully");
              }, delegate (PlayerIOError error)
              {
                 Debug.Log("Failed to Login" + error.Message);
              });
        }

        public void JoinLobbyRoom()
        {
            Client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost",8184);

            Client.Multiplayer.CreateJoinRoom("$service-room$", "Lobby", true, null, null,
                (Connection connection) =>
                {
                    Debug.Log("Successfully joined room");
                    LevelManager.mInstance.OnAuthenticationComplete();
                }, (PlayerIOError error) =>
                {
                    Debug.Log("Error: " + error.Message);
                });
        }

        public void JoinGameRoom()
        {
            string RoomID = Client.ConnectUserId + "" + DateTime.Now.ToString();

            Client.Multiplayer.ListRooms("GameRoom", null, 5, 0, 
                (RoomInfo[] rooms) =>
                {
                    if (rooms.Length > 0)
                    {
                        Client.Multiplayer.JoinRoom(rooms[0].Id, null,
                            (Connection _connection) =>
                            {
                                Debug.Log("Successfully joined game room");
                                _connection.OnMessage += GameRoomMessageHandler;
                                LevelManager.mInstance.OnJoinGameRoom();
                            });
                    }
                    else 
                    {
                        Client.Multiplayer.CreateJoinRoom(RoomID, "GameRoom", true, null, null, (Connection connection) =>
                        {
                            Debug.Log("Successfully joined game room");
                            connection.OnMessage += GameRoomMessageHandler;
                            LevelManager.mInstance.OnJoinGameRoom();
                        });
                    }
                }, (PlayerIOError error) =>
                {
                    Client.Multiplayer.CreateJoinRoom(RoomID,"GameRoom",true,null,null,(Connection connection)=>
                    {
                        Debug.Log("Successfully joined game room");
                        connection.OnMessage += GameRoomMessageHandler;
                        LevelManager.mInstance.OnJoinGameRoom();
                    });
                });



            
        }

        private void GameRoomMessageHandler(object sender,Message e)
        {
            Debug.Log(e.ToString());
        }
    }
}
