﻿using CellWar.Model.Map;
using CellWar.GameData;
using CellWar.Model.Substance;
using UnityEngine;
using CellWar.Controller;

namespace CellWar.View
{
    public class U3D_BlockMouseDetect : MonoBehaviour
    {

        public Block HexBlockModel;

        bool m_IsMouseEnter = false;

        Renderer m_BlockRenderer;

        static Color INIT_COLOR = new Color(188f / 255f, 238f / 255f, 104f / 255f, 1f);
        static Color MOUSE_STRAIN_COLOR = Color.green;
        static Color MOUSE_EMPTY_COLOR = Color.blue;

        /// <summary>
        /// 方块人口对应颜色
        /// </summary>
        Color m_PopulationColor = INIT_COLOR;

        /// <summary>
        /// 目标颜色
        /// </summary>
        Color m_DestColor = INIT_COLOR;
        /// <summary>
        /// 当前颜色和目标差值
        /// </summary>
        const float STEP_COLOR = 2.0f;

        // Start is called before the first frame update
        void Start()
        {
            m_BlockRenderer = GetComponent<MeshRenderer>();
            //ChangeBlockColor(INIT_COLOR);
        }

        public void BlockColorUpdate()
        {
            //更新这块方块颜色和数量
            HexBlockModel.TotalPopulation = HexBlockModel.GetTotalPopulation();
            m_PopulationColor = GetColorAccordingToPopulation(HexBlockModel.TotalPopulation);

            if (!m_IsMouseEnter)
            {
                m_DestColor = m_PopulationColor;
            }
        }

        /// <summary>
        /// 显示颜色变化
        /// </summary>
        private void FixedUpdate()
        {
            ChangeBlockColor(Color.Lerp(GetCurrentColor(), m_DestColor, STEP_COLOR * Time.deltaTime));
        }

        /// <summary>
        /// 返回人口对应颜色,颜色值需要再修订
        /// </summary>
        /// <param name="n">人口</param>
        /// <returns></returns>
        Color GetColorAccordingToPopulation(int n)
        {
            if (n > 1000)
            {
                return new Color(0.5f, 0f, 0f, 0.5f);
            }
            if (n > 500)
            {
                return new Color(0f, 0f, 0f, 0.5f);
            }
            if (n > 100)
            {
                return new Color(1f, 0f, 0f, 0.5f);
            }
            if (n > 50)
            {
                return new Color(1f, 0.2f, 0.2f, 0.5f);
            }
            if (n > 10)
            {
                return new Color(1f, 0.4f, 0.4f, 0.5f);
            }
            
            return INIT_COLOR;
        }
        
        Color GetCurrentColor()
        {
            return m_BlockRenderer.material.color;
        }
        private void ChangeBlockColor(Color color)
        {
            if (m_BlockRenderer)
            {
                m_BlockRenderer.material.color = color;
            }
        }


        /// <summary>
        /// 左键放置细菌 右键取消手上细菌
        /// </summary>
        private void OnMouseOver()
        {
            if (UIManager.Instance.CheckGuiRaycastObjects()) return;

            // 0 1 2 左键 右键 中键
            if (Input.GetMouseButton(0))
            {
                if (MainGameCurrent.HoldingStrain != null)
                {
                    ProcessSelectedStrain();
                }
            }
            else if (Input.GetMouseButton(1))
            {
                if (MainGameCurrent.HoldingStrain != null)
                {
                    MainGameCurrent.HoldingStrain = null;
                    m_DestColor = MOUSE_EMPTY_COLOR;
                }
            }
            /*
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                if (HexBlockModel.IsActive && HexBlockModel.Strains.Count > 0)
                {
                    HexBlockModel.Strains.RemoveAt(HexBlockModel.Strains.Count - 1);
                }
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {

            }*/
        }

        public void ProcessSelectedStrain()
        {
            ChangeBlockColor(Color.yellow);
            // 防止反复增加同一种细菌
            if (!HexBlockModel.Strains.Exists(m => m.Name == MainGameCurrent.HoldingStrain.Name))
            {
                HexBlockModel.Strains.Add((Strain)MainGameCurrent.HoldingStrain.Clone());
            }
            GameObject.Find(MainGameCurrent.HoldingStrain.Name).SetActive(false);
            MainGameCurrent.HoldingStrain = null;

            // 放置一种细菌游戏才会开始
            if (!GameManager.Instance.IsGameStarted && !GameManager.Instance.IsGameCompleted)
            {
                GameManager.Instance.IsGameStarted = true;
            }
        }

        private void OnMouseEnter()
        {
            if (UIManager.Instance.CheckGuiRaycastObjects()) return;
            //Debug.Log("Mouse Enter");
            m_IsMouseEnter = true;
            MainGameCurrent.FocusedHexBlock = HexBlockModel;
            
            if (MainGameCurrent.HoldingStrain != null)
            {
                /// 当手里拿着细菌准备放置时的代码
                m_DestColor = MOUSE_STRAIN_COLOR;
            }
            else
            {
                /// 手里什么都没有拿时鼠标移动到格子上的代码
                m_DestColor = MOUSE_EMPTY_COLOR;
            }
            
        }

        private void OnMouseExit()
        {
            m_IsMouseEnter = false;
            MainGameCurrent.FocusedHexBlock = null;

            m_DestColor = m_PopulationColor;
        }

    }
}