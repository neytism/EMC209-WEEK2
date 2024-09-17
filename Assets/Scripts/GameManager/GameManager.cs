using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using GNW.InputData;
using GNW.UIManager;

namespace GNW.GameManager
{
    public class GameManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        private NetworkRunner _runner;

        [SerializeField] private NetworkPrefabRef _playerPrefab;
        private Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();

        public Dictionary<PlayerRef, NetworkObject> SpawnedPlayers => _spawnedPlayers;


        private bool _isJumpButtonPressed;
        private bool _isShootButtonPressed;

        private void Update()
        {
            _isJumpButtonPressed = Input.GetKey(KeyCode.Space);
            _isShootButtonPressed = Input.GetMouseButton(0);
            //_isMouseButton0Pressed = _isMouseButton0Pressed | Input.GetMouseButton(0);
        }

        #region NetworkRunner Callbacks

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                Vector3 customLocation = new Vector3(runner.SessionInfo.PlayerCount, 0, 0);
                NetworkObject playerNetworkObject = runner.Spawn(_playerPrefab, customLocation, quaternion.identity);
                playerNetworkObject.AssignInputAuthority(player);
                
                _spawnedPlayers.Add(player, playerNetworkObject);
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (_spawnedPlayers.TryGetValue(player, out NetworkObject playerNetworkObject))
            {
                runner.Despawn(playerNetworkObject);
                _spawnedPlayers.Remove(player);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var data = new NetworkInputData();

            if (Input.GetKey(KeyCode.W)) data.Direction += Vector3.forward;
            if (Input.GetKey(KeyCode.A)) data.Direction += Vector3.left;
            if (Input.GetKey(KeyCode.S)) data.Direction += Vector3.back;
            if (Input.GetKey(KeyCode.D)) data.Direction += Vector3.right;

            data.buttons.Set(NetworkInputData.JUMPBUTTON, _isJumpButtonPressed);
            data.buttons.Set(NetworkInputData.SHOOTBUTTON, _isShootButtonPressed);
            
            input.Set(data);
        }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {}
        public void OnConnectedToServer(NetworkRunner runner) {}
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) {}
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {}
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {}
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {}
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) {}
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) {}
        public void OnSceneLoadDone(NetworkRunner runner) {}
        public void OnSceneLoadStart(NetworkRunner runner) {}

        #endregion

        async void StartGame(GameMode mode)
        {
            // lets fusion know that we will be sending input
            _runner = this.gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true;

            //create the scene info to send to fusion
            var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
            var sceneInfo = new NetworkSceneInfo();
            if (scene.IsValid)
            {
                sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
            }

            await _runner.StartGame(
                new StartGameArgs()
                {
                    GameMode = mode,
                    SessionName = "TestRoom",
                    Scene = scene,
                    SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
                }
            );
            
        }

        private void OnEnable()
        {
            UIManager.UIManager.OnStartGameButtonEvent += StartGame;
        }

        private void OnDisable()
        {
            UIManager.UIManager.OnStartGameButtonEvent -= StartGame;
        }


        // private void OnGUI()
        // {
        //     if (_runner == null)
        //     {
        //         if (GUI.Button(new Rect(0,0,200,40), "Host"))
        //         {
        //             StartGame(GameMode.Host);
        //         }
        //         
        //         if (GUI.Button(new Rect(0,45,200,40), "Client"))
        //         {
        //             StartGame(GameMode.Client);
        //         }
        //     }
        // }
    }

}

