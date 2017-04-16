using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityStandardAssets.Utility
{
#if UNITY_EDITOR

    [ExecuteInEditMode]
#endif
    public class PlatformSpecificContent : MonoBehaviour
    {
        private enum BuildTargetGroup
        {
            Standalone,
            Mobile
        }

        [SerializeField] private BuildTargetGroup m_BuildTargetGroup;
        [SerializeField] private GameObject[] m_Content = new GameObject[0];
        [SerializeField] private MonoBehaviour[] m_MonoBehaviours = new MonoBehaviour[0];
        [SerializeField] private bool m_ChildrenOfThisObject;

#if !UNITY_EDITOR
	void OnEnable()
	{
		CheckEnableContent();
	}
#endif

#if UNITY_EDITOR

        private void OnEnable()
        {
            EditorUserBuildSettings.activeBuildTargetChanged += this.Update;
            EditorApplication.update += this.Update;
        }


        private void OnDisable()
        {
            EditorUserBuildSettings.activeBuildTargetChanged -= this.Update;
            EditorApplication.update -= this.Update;
        }


        private void Update()
        {
            CheckEnableContent();
        }
#endif


        private void CheckEnableContent()
        {
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_TIZEN || UNITY_STV )
		if (m_BuildTargetGroup == BuildTargetGroup.Mobile)
		{
			EnableContent(true);
		} else {
			EnableContent(false);
		}
#endif

#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_TIZEN || UNITY_STV )
            if (this.m_BuildTargetGroup == BuildTargetGroup.Mobile)
            {
                EnableContent(false);
            }
            else
            {
                EnableContent(true);
            }
#endif
        }


        private void EnableContent(bool enabled)
        {
            if (this.m_Content.Length > 0)
            {
                foreach (var g in this.m_Content)
                {
                    if (g != null)
                    {
                        g.SetActive(enabled);
                    }
                }
            }
            if (this.m_ChildrenOfThisObject)
            {
                foreach (Transform t in this.transform)
                {
                    t.gameObject.SetActive(enabled);
                }
            }
            if (this.m_MonoBehaviours.Length > 0)
            {
                foreach (var monoBehaviour in this.m_MonoBehaviours)
                {
                    monoBehaviour.enabled = enabled;
                }
            }
        }
    }
}
