using System.Collections;
using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FPSNetworkManager : NetworkManager
{
    [SerializeField] private PlayerController gamePlayerPrefab;
    public List<PlayerController> gamePlayers { get; } = new List<PlayerController>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == "Lobby") {
            PlayerController playerInstance = Instantiate(gamePlayerPrefab);
            playerInstance.connectionID = conn.connectionId;
            playerInstance.playerIDNumber = gamePlayers.Count + 1;
            playerInstance.playerSteamID = (ulong) SteamMatchmaking.GetLobbyMemberByIndex((CSteamID) SteamLobby.Instance.currentLobbyID, gamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, playerInstance.gameObject);
        }
    }

    public void StartGame(string sceneName) {
        ServerChangeScene(sceneName);
    }

     void Respawn(NetworkIdentity identity)
    {
        if (identity.netId == 0)
        {
            // If the object has not been spawned, then do a full spawn and update observers
            //Spawn(identity.gameObject, identity.connectionToClient);
        }
        else
        {
            // otherwise just replace his data
           // SendSpawnMessage(identity, identity.connectionToClient);
        }
    }

    IEnumerator RespawnDelay()
    {

        Debug.Log("Respawnin player in 8 seconds.");
        yield return new WaitForSecondsRealtime(8f);
        Debug.Log("Respawning player.");
       // RpcRespawn();
    }
    public void CmdRespawnPlayer()
    {

    }

}
