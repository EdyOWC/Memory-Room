#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using TMPro;
using RTLTMPro; // runtime component lives in the package

// NOTE: Avoid "RTLTMPro.Editor" namespace to prevent name collisions
namespace RTLTMProFixes
{
    [CustomEditor(typeof(RTLTextMeshPro), true)]
    [CanEditMultipleObjects]
    public class RTLTextMeshProInspector : UnityEditor.Editor
    {
        SerializedProperty textProp;
        SerializedProperty fontAssetProp;
        SerializedProperty fontSizeProp;
        SerializedProperty colorProp;

        void OnEnable()
        {
            // standard TMP backing fields
            textProp = serializedObject.FindProperty("m_text");
            fontAssetProp = serializedObject.FindProperty("m_fontAsset");
            fontSizeProp = serializedObject.FindProperty("m_fontSize");
            colorProp = serializedObject.FindProperty("m_fontColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("RTL TextMeshPro", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(textProp, new GUIContent("Text (RTL)"));
            EditorGUILayout.PropertyField(fontAssetProp, new GUIContent("Font Asset"));
            EditorGUILayout.PropertyField(fontSizeProp, new GUIContent("Font Size"));
            EditorGUILayout.PropertyField(colorProp, new GUIContent("Font Color"));

            EditorGUILayout.Space();

            var rtl = (RTLTextMeshPro)target;

            // alignment + basic RTL options
            rtl.alignment = (TextAlignmentOptions)EditorGUILayout.EnumPopup("Alignment", rtl.alignment);
            rtl.isRightToLeftText = EditorGUILayout.Toggle("Right To Left", rtl.isRightToLeftText);
            rtl.richText = EditorGUILayout.Toggle("Rich Text", rtl.richText);
            rtl.enableWordWrapping = EditorGUILayout.Toggle("Word Wrapping", rtl.enableWordWrapping);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
