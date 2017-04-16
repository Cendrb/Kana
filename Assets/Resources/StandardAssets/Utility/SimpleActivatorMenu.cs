using System;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
    public class SimpleActivatorMenu : MonoBehaviour
    {
        // An incredibly simple menu which, when given references
        // to gameobjects in the scene
        public GUIText camSwitchButton;
        public GameObject[] objects;


        private int m_CurrentActiveObject;


        private void OnEnable()
        {
            // active object starts from first in array
            this.m_CurrentActiveObject = 0;
            this.camSwitchButton.text = this.objects[this.m_CurrentActiveObject].name;
        }


        public void NextCamera()
        {
            int nextactiveobject = this.m_CurrentActiveObject + 1 >= this.objects.Length ? 0 : this.m_CurrentActiveObject + 1;

            for (int i = 0; i < this.objects.Length; i++)
            {
                this.objects[i].SetActive(i == nextactiveobject);
            }

            this.m_CurrentActiveObject = nextactiveobject;
            this.camSwitchButton.text = this.objects[this.m_CurrentActiveObject].name;
        }
    }
}
