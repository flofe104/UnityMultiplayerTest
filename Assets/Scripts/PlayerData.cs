using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerData
{
    public class PlayerData
    {

        public int id;
        public string username;

        public Vector3 position;
        public Quaternion rotation;

        public PlayerData(int id, string username, Vector3 spawnPos)
        {
            this.id = id;
            this.username = username;
            position = spawnPos;
        }

        public PlayerData() { }

    }
}