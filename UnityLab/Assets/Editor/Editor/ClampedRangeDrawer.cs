using UnityEditor;
using UnityEngine;

namespace Kyrnel
{
    [CustomPropertyDrawer(typeof(ClampedRangeAttribute))]
    public class ClampedRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ClampedRangeAttribute clampedRange = (ClampedRangeAttribute)attribute;
            
            label = EditorGUI.BeginProperty(position, label, property);
            
            if (property.propertyType == SerializedPropertyType.Float)
            {
                property.floatValue = EditorGUI.Slider(position, label, property.floatValue, clampedRange.min, clampedRange.max);
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = EditorGUI.IntSlider(position, label, property.intValue, (int)clampedRange.min, (int)clampedRange.max);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use [ClampedRange] with float or int.");
            }
            
            EditorGUI.EndProperty();
        }
    }
}