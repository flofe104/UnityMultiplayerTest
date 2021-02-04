using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ServerData
{

    public class InputData
    {
        public Vector2 inputMovement;
        public Quaternion rotation;
        public bool jump;

        public static InputData GetCurrentInputData(Transform transform)
        {
            InputData result = new InputData();
            result.inputMovement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            result.rotation = transform.rotation;
            result.jump = Input.GetButton("Jump");

            return result;
        }

    }

}
