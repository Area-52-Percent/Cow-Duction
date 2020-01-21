using UnityEngine;
using Mirror;

public class NetworkManagerCowduction : NetworkManager
{
    public Transform spaceshipSpawn;
    public Transform[] farmerSpawns;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        // add player at correct spawn position
        bool spaceship = conn.connectionId < 1 ? true : false;
        
        Transform start = spaceship ? spaceshipSpawn : farmerSpawns[conn.connectionId - 1];
        GameObject player = Instantiate(spawnPrefabs.Find(prefab => prefab.name == (spaceship ? "Spaceship" : "Farmer")), start.position, start.rotation);

        NetworkServer.AddPlayerForConnection(conn, player);
    }
}
