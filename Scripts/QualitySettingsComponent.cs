
using System;
using UdonSharp;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace PeskyBox.EasyQualitySwitch 
{
    public enum QualityLevel
    {
        Mobile = 0b0001,
        Low = 0b0010,
        Medium = 0b0100,
        High = 0b1000
    }
    
    public class QualitySettingsComponent : UdonSharpBehaviour
    {
        [SerializeField] private GameObject[] _objectsToControl;
        [HideInInspector]public int selectedQualityLevelMask;
        
        public void SetQualityLevel(QualityLevel level)
        {
            foreach (var thing in _objectsToControl)
            {
                if (thing == null) continue;
                // Set the active state of the object based on the selected quality level
                thing.SetActive((selectedQualityLevelMask & (int)level) != 0);
            }
        }
        
    }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(QualitySettingsComponent))]
    class QualitSettingsComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var qualitySettingsComponent = (QualitySettingsComponent)target;
            
            // Quality selection
            qualitySettingsComponent.selectedQualityLevelMask = EditorGUILayout.MaskField("Quality Level", qualitySettingsComponent.selectedQualityLevelMask, Enum.GetNames(typeof(QualityLevel)));
            
        }
    }
#endif

}