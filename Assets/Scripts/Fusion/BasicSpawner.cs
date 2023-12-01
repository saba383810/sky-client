using System;
using System.Collections.Generic;
using Cinemachine;
using Common;
using Common.Loading;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using Lobby;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
   
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [NonSerialized] private List<string> users = new();
    [NonSerialized] public NetworkObject LocalPlayerObject;
    [NonSerialized] public NetworkRunner LocalRunner;

    public async void StartGame()
    {
        Debug.Log($"[StartGame]");
        // Create the Fusion runner and let it know that we will be providing user input
        LocalRunner = gameObject.AddComponent<NetworkRunner>();
        LocalRunner.ProvideInput = true;
        PopupManager.ShowPopup(PopupManager.PopupName.LoadingWindow);

        // Start or join (depends on gamemode) a session with a specific name
        await LocalRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "World1",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 50,
        });
        
        LoadingManager.ChangeScene("Room");
    }

    public void GeneratePlayer()
    {
        var playerId = LocalRunner.LocalPlayer.PlayerId;
        DataManager.Instance.PlayerDataManager.PlayerPhotonId = playerId.ToString();
        var playerName = DataManager.Instance.PlayerDataManager.PlayerName;
        LocalPlayerObject = LocalRunner.Spawn(playerPrefab, new Vector3(0, -1, 0), Quaternion.identity);
        LocalPlayerObject.RequestStateAuthority();
        LocalPlayerObject.name = $"Player_{playerId}";
        
        var hairId = DataManager.Instance.PlayerDataManager.HairId;
        var dressId = DataManager.Instance.PlayerDataManager.DressId;
        var accId = DataManager.Instance.PlayerDataManager.AccId;
        var playerController = LocalPlayerObject.GetComponent<PlayerController>();
        playerController.RPC_SetUserData(playerId.ToString(), playerName, hairId, dressId, accId);
       
        var virtualCamera = GameObject.Find("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = LocalPlayerObject.transform;
    }

    private void UpdateUser(string playerId)
    {
        RemoveUser(playerId);
        users.Add(playerId);
    }

    /// <summary>
    /// ユーザリストから対象のPlayerIdのユーザを削除します。
    /// </summary>
    public void RemoveUser(string playerName)
    {
        var user = users.Find(userName => userName == playerName);
        if (user == null) return;

        users.Remove(user);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[Photon][Fusion] Player joined {player.PlayerId}");
        UpdateUser(player.PlayerId.ToString());
       
        if (LocalPlayerObject != null)
        {
            var playerId = DataManager.Instance.PlayerDataManager.PlayerPhotonId;
            var playerName = DataManager.Instance.PlayerDataManager.PlayerName;
            var hairId = DataManager.Instance.PlayerDataManager.HairId;
            var dressId = DataManager.Instance.PlayerDataManager.DressId;
            var accId = DataManager.Instance.PlayerDataManager.AccId;
            var playerController = LocalPlayerObject.GetComponent<PlayerController>();
            playerController.RPC_SetUserData(playerId, playerName,hairId,dressId,accId);
        }
    }

    public async void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[Fusion][OnPlayerLeft]");
        var leftPlayer = GameObject.Find($"Player_{player.PlayerId}");
        if (leftPlayer != null)
        {
            var leftPlayerNetworkObject = leftPlayer.GetComponent<NetworkObject>();
            leftPlayerNetworkObject.RequestStateAuthority();
            runner.Despawn(leftPlayerNetworkObject);
            if (leftPlayerNetworkObject != null)
            {
                Destroy(leftPlayerNetworkObject.gameObject);
            }
        }
        users.Remove(player.PlayerId.ToString());
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        var playerClone = GameObject.Find("Player(Clone)");
        if (playerClone != null)
        {
            Destroy(playerClone);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
       // Debug.Log($"[Fusion][OnInput]");
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.Log($"[Fusion][OnInputMissing]");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"[Fusion][OnShutDown]");
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log($"[Fusion][OnConnectedServer]");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log($"[Fusion][OnDisconnectedFromServer]");
        foreach (var playerId in users)
        {
            var player = GameObject.Find($"Player_{playerId}").GetComponent<NetworkObject>();
            if(player != null) runner.Despawn(player);
        }
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log($"[Fusion][OnConnectRequest]");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log($"[Fusion][OnConnectFailed]");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log($"[Fusion][OnUserSimulationMessage]");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log($"[Fusion][OnSessionListUpdated]");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Debug.Log($"[Fusion][OnCustomAuthenticationResponse]");
    }
     public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
     {
         Debug.Log($"[Fusion][OnHostMigration]");
     }

     public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
     {
         Debug.Log($"[Fusion][OnReliableDataReceived]");
     }

     public void OnSceneLoadDone(NetworkRunner runner)
     {
         Debug.Log($"[Fusion][OnSceneLoadDone]");
     }

     public void OnSceneLoadStart(NetworkRunner runner)
     {
         Debug.Log($"[Fusion][OnSceneLoadStart]");
     }
}
