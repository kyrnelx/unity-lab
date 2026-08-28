using UnityEditor;
using UnityEngine;

namespace Kyrnel
{
    [CustomEditor(typeof(CharacterData))]
    [CanEditMultipleObjects]
    public class CharacterDataEditor : UnityEditor.Editor
    {
        private SerializedProperty _nameProp;
        private SerializedProperty _healthProp;
        private SerializedProperty _maxHealthProp;
        private SerializedProperty _speedProp;

        private void OnEnable()
        {
            _nameProp = serializedObject.FindProperty("name");
            _healthProp = serializedObject.FindProperty("health");
            _maxHealthProp = serializedObject.FindProperty("maxHealth");
            _speedProp = serializedObject.FindProperty("speed");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.LabelField("基础信息", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_nameProp, new GUIContent("角色名称"));
            EditorGUILayout.PropertyField(_speedProp, new GUIContent("移动速度"));

            GUILayout.Space(10);

            _healthProp.intValue = EditorGUILayout.IntSlider("当前生命值", _healthProp.intValue, 0, _maxHealthProp.intValue);

            float healthPercent =
                _maxHealthProp.intValue > 0 ? (float)_healthProp.intValue / _maxHealthProp.intValue : 0;
            GUI.enabled = false;
            EditorGUILayout.TextField("生命值百分比", (healthPercent * 100f).ToString("F1") + "%");
            GUI.enabled = true;

            if (healthPercent < 0.3f)
            {
                GUIStyle warningStyle = new GUIStyle(EditorStyles.helpBox);
                warningStyle.normal.textColor = Color.red;
                EditorGUILayout.HelpBox("警告：生命值过低！", MessageType.Error);
            }

            GUILayout.Space(10);

            if (GUILayout.Button("重置生命值"))
            {
                _healthProp.intValue = _maxHealthProp.intValue;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}