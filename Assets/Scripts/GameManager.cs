using ServerData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public GameObject localPlayerPrefab;
    public GameObject otherPlayerPrefab;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exsits, destroy object!");
            Destroy(this);
        }
    }

    public void SpawnPlayer(PlayerData playerData)
    {
        GameObject player;
        if(playerData.id == Client.instance.myId)
        {
            player = Instantiate(localPlayerPrefab);
        }
        else
        {
            player = Instantiate(otherPlayerPrefab);
        }
        PlayerManager manager = player.GetComponent<PlayerManager>();
        manager.id = playerData.id;
        manager.username = playerData.username;
        players.Add(playerData.id, manager);
    }
}
