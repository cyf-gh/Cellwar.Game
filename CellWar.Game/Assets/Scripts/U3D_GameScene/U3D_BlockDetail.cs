﻿using System.Collections;
using System.Collections.Generic;
using CellWar.GameData;
using UnityEngine;
using UnityEngine.UI;

namespace CellWar.View {
    public class U3D_BlockDetail : MonoBehaviour {
        // Start is called before the first frame update
        void Start() {
            gameObject.GetComponent<Text>().text = "Game Start!";
        }

        // Update is called once per frame
        void Update() {
            if (MainGameCurrent.FocusedHexBlock == null || !MainGameCurrent.FocusedHexBlock.IsActive)
            {
                gameObject.GetComponent<Text>().text = "";
                return;
            }

            if (MainGameCurrent.FocusedHexBlock.IsActive)
            {
                gameObject.GetComponent<Text>().text = MainGameCurrent.GetCurrentBlockDetailInfo();
            }
        }
    }
}
