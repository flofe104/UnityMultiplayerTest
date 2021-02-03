using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ServerData
{
    class PlayerData
    {

        public int id;
        public string username;

        public Vector3 position;
        public Quaternion rotation;

        public PlayerData(int id, string username, Vector3 spawnPos)
        {
            this.id = id;
            this.username = username;
            this.position = spawnPos;
        }
        public PlayerData() { }

    }
}
