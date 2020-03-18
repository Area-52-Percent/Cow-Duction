using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class NetworkManagerCowduction : NetworkManager
{
    public Transform spaceshipSpawn;
    public Transform[] farmerSpawns;

    [Scene]
    [Tooltip("Add all sub-scenes to this list")]
    public string[] subScenes;

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        // Load all subscenes on the server
        foreach (string sceneName in subScenes)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        UnloadScenes();
    }

    void UnloadScenes()
    {
        foreach (string sceneName in subScenes)
            if (SceneManager.GetSceneByName(sceneName).IsValid())
                StartCoroutine(UnloadScene(sceneName));
    }

    IEnumerator UnloadScene(string sceneName)
    {
        yield return SceneManager.UnloadSceneAsync(sceneName);
        yield return Resources.UnloadUnusedAssets();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        // add player at correct spawn position
        bool spaceship = conn.connectionId < 1 ? true : false;
        
        Transform start = spaceship ? spaceshipSpawn : farmerSpawns[conn.connectionId - 1];
        GameObject player = Instantiate(spawnPrefabs.Find(prefab => prefab.name == (spaceship ? "Spaceship" : "Farmer")), start.position, start.rotation);

        NetworkServer.AddPlayerForConnection(conn, player);
    }
}
