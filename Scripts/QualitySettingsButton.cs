using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDKBase;
using qualityLevels = PeskyBox.EasyQualitySwitch.QualityLevel;

namespace PeskyBox.EasyQualitySwitch 
{
    public class QualitySettingsButton : MonoBehaviour, IEditorOnly
    {
        public qualityLevels qualityLevelToToggle;
    }
}
