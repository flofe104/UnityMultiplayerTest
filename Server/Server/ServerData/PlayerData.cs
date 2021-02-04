using Server;
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

        [ServerOnly]
        private float moveSpeed = 5f / Constants.MS_PER_TICK;

        [ServerOnly]
        private InputData input = new InputData();

        public PlayerData(int id, string username, Vector3 spawnPos)
        {
            this.id = id;
            this.username = username;
            this.position = spawnPos;
        }
        public PlayerData() { }


        public void Update()
        {
            Move(input.inputMovement);
        }

        private void Move(Vector2 inputMovement)
        {
            Vector3 forward = Vector3.Transform(new Vector3(0,0,1), rotation);
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, new Vector3(0, 1, 0)));

            Vector3 moveDir = right * inputMovement.X + forward * inputMovement.Y;
            position += moveDir * moveSpeed;

            ServerSend.PlayerPosition(this);
            ServerSend.PlayerRotation(this);
        }

        public void SetInput(InputData data)
        {
            input = data;
            if (input.inputMovement.X > 0 || input.inputMovement.Y > 0)
            {
                input.inputMovement = Vector2.Normalize(input.inputMovement);
            }
            rotation = data.rotation;
        }
    }
}
