using System;
using System.Reflection;
using UdonSharp;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace PeskyBox.EasyQualitySwitch
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class EasyQualitySwitch : UdonSharpBehaviour
    {
        // [HideInInspector]
        public QualitySettingsComponent[] QualitySettingsComponents;
        // [HideInInspector]
        public Toggle[] MobileQualityButtonsGraphics;
        // [HideInInspector]
        public Toggle[] LowQualityButtonsGraphics;
        // [HideInInspector]
        public Toggle[] MediumQualityButtonGraphics;
        // [HideInInspector]
        public Toggle[] HighQualityButtonGraphics;
        // [HideInInspector]
        public Toggle[] QualitySettingButtons;
        public QualityLevel DefaultQualityLevel;
        
        [HideInInspector]
        public QualityLevel CurrentQualityLevel;
        

        private void Start()
        {
            SetQualityLevel(DefaultQualityLevel);
            
            #if UNITY_ANDROID
                // If quest, set to mobile
                SetQualityLevel(QualityLevel.Low);
            #endif
            
            CurrentQualityLevel = DefaultQualityLevel;
            Debug.Log($"Default quality level set to {DefaultQualityLevel}");
        }

        public void SetQualityLevel(QualityLevel level)
        {
            for (var i = 0; i < QualitySettingsComponents.Length; i++)
            {
                QualitySettingsComponents[i].SetQualityLevel(level);
                CurrentQualityLevel = level;
                Debug.Log($"Quality level set to {level}");
            }
            
            // Set the toggle button graphic states
            foreach (var VARIABLE in MobileQualityButtonsGraphics)
            {
                VARIABLE.SetIsOnWithoutNotify(level == QualityLevel.Mobile);
            }
            foreach (var VARIABLE in LowQualityButtonsGraphics)
            {
                VARIABLE.SetIsOnWithoutNotify(level == QualityLevel.Low);
            }
            foreach (var VARIABLE in MediumQualityButtonGraphics)
            {
                VARIABLE.SetIsOnWithoutNotify(level == QualityLevel.Medium);
            }
            foreach (var VARIABLE in HighQualityButtonGraphics)
            {
                VARIABLE.SetIsOnWithoutNotify(level == QualityLevel.High);
            }
        }
        
        public void SetQualityMobile()
        {
            SetQualityLevel(QualityLevel.Mobile);
            
            Debug.Log("Quality level set to Mobile");
        }
        
        public void SetQualityLevelLow()
        {
            SetQualityLevel(QualityLevel.Low);
        }
        
        public void SetQualityLevelMedium()
        {
            SetQualityLevel(QualityLevel.Medium);
        }
        
        public void SetQualityLevelHigh()
        {
            SetQualityLevel(QualityLevel.High);
        }
    }
    
    #if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(EasyQualitySwitch))]
    class EasyQualitySwitchEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var easyQualitySwitch = (EasyQualitySwitch)target;
            var thisUdon = easyQualitySwitch.GetComponent<UdonBehaviour>();
            
            // Button to get all QualitySettingsComponents in the scene
            if (GUILayout.Button("Setup Everything!"))
            {
                easyQualitySwitch.QualitySettingsComponents = FindObjectsOfType<QualitySettingsComponent>();
                            
                // Get all buttons with the QulitySettingButton script
                var buttons = FindObjectsOfType<QualitySettingsButton>();
                
                // Clear all buttons in list
                easyQualitySwitch.QualitySettingButtons = Array.Empty<Toggle>();
                easyQualitySwitch.MobileQualityButtonsGraphics = Array.Empty<Toggle>();
                easyQualitySwitch.LowQualityButtonsGraphics = Array.Empty<Toggle>();
                easyQualitySwitch.MediumQualityButtonGraphics = Array.Empty<Toggle>();
                easyQualitySwitch.HighQualityButtonGraphics = Array.Empty<Toggle>();
                
                // Clear all listeners from the buttons
                foreach (var qualityButton in buttons)
                {
                    var button = qualityButton.GetComponent<Toggle>();
                    var listeners = button.onValueChanged.GetPersistentEventCount();
                    // Remove all listeners, this reports index out of range errors
                    for (var i = 0; i < listeners; i++)
                        UnityEventTools.RemovePersistentListener(button.onValueChanged, i);
                }
            
                // Set the OnValueChanged callbacks for all buttons
                foreach (var button in buttons)
                {
                    // Get toggle
                    var toggle = button.GetComponent<Toggle>();
                    
                    switch (button.qualityLevelToToggle)
                    {
                        case QualityLevel.Mobile:
                            // Add Button to the list of buttons for its quality level
                            Array.Resize(ref easyQualitySwitch.MobileQualityButtonsGraphics, easyQualitySwitch.MobileQualityButtonsGraphics.Length + 1);
                            easyQualitySwitch.MobileQualityButtonsGraphics[^1] = toggle;
                            setupToggleButton(thisUdon, toggle, "SetQualityMobile");
                            break;
                        case QualityLevel.Low:
                            // Add Button to the list of buttons for its quality level
                            Array.Resize(ref easyQualitySwitch.LowQualityButtonsGraphics, easyQualitySwitch.LowQualityButtonsGraphics.Length + 1);
                            easyQualitySwitch.LowQualityButtonsGraphics[^1] = toggle;
                            setupToggleButton(thisUdon, toggle, "SetQualityLevelLow");
                            break;
                        case QualityLevel.Medium:
                            // Add Button to the list of buttons for its quality level
                            Array.Resize(ref easyQualitySwitch.MediumQualityButtonGraphics, easyQualitySwitch.MediumQualityButtonGraphics.Length + 1);
                            easyQualitySwitch.MediumQualityButtonGraphics[^1] = toggle;
                            setupToggleButton(thisUdon, toggle, "SetQualityLevelMedium");
                            break;
                        case QualityLevel.High:
                            // Add Button to the list of buttons for its quality level
                            Array.Resize(ref easyQualitySwitch.HighQualityButtonGraphics, easyQualitySwitch.HighQualityButtonGraphics.Length + 1);
                            easyQualitySwitch.HighQualityButtonGraphics[^1] = toggle;
                            setupToggleButton(thisUdon, toggle, "SetQualityLevelHigh");
                            break;
                    }
                    
                    // Add the button to the list of buttons
                    Array.Resize(ref easyQualitySwitch.QualitySettingButtons, easyQualitySwitch.QualitySettingButtons.Length + 1);
                    easyQualitySwitch.QualitySettingButtons[^1] = button.GetComponent<Toggle>();
                }
            }
        }
        
        private static void setupToggleButton(UdonBehaviour toCall, Toggle button, string methodName)
        {
            // Get the udonbehaviour of this object
            var toCallUdon = toCall;

            MethodInfo method = toCallUdon.GetType().GetMethod("SendCustomEvent",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var action =
                Delegate.CreateDelegate(typeof(UnityAction<string>), toCall, method) as
                    UnityAction<string>;
            UnityEventTools.AddStringPersistentListener(button.onValueChanged, action,
                methodName);
            // Save
            EditorUtility.SetDirty(button);
        }
    }
    #endif
}