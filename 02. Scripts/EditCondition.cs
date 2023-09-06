using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;



/**
 * @author Chat GPT 4
 * @brief 언리얼 엔진의 EditCondition 메타데이터 지정자처럼, 조건에 따라 프로퍼티를 보여주거나 숨기는 어트리뷰트입니다.
 * @details 해당 코드는 Chat GPT 4가 뽑은 코드를 약간 수정하였습니다.
 * @since 2023-04-17
 */
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class EditCondition : PropertyAttribute
{
	public string EditConditionBooleanProperty { get; private set; }

	public bool bInvertCondition { get; private set; }



	public EditCondition(string ConditionBooleanProperty)
	{
		if (ConditionBooleanProperty.StartsWith("!"))
		{
			EditConditionBooleanProperty = ConditionBooleanProperty.Substring(1);

			bInvertCondition = true;
		}
		else
		{
			EditConditionBooleanProperty = ConditionBooleanProperty;

			bInvertCondition = false;
		}
	}
}



#if UNITY_EDITOR
namespace UnityEditor
{
	[CustomPropertyDrawer(typeof(EditCondition))]
	public class EditConditionPropertDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditCondition EditConditionAttribute = (EditCondition)attribute;

			SerializedProperty ConditionProperty = property.serializedObject.
				FindProperty(EditConditionAttribute.EditConditionBooleanProperty);

			// bool 값과 Invert 조건을 XOR 연산 (이는 (true, false), (false, true)에서 true)
			if (ConditionProperty != null &&
				(ConditionProperty.boolValue ^ EditConditionAttribute.bInvertCondition))
			{
				EditorGUI.PropertyField(position, property, label);
			}
		}



		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			EditCondition EditConditionAttribute = (EditCondition)attribute;

			SerializedProperty ConditionProperty = property.serializedObject.
				FindProperty(EditConditionAttribute.EditConditionBooleanProperty);

			if (ConditionProperty != null &&
				(ConditionProperty.boolValue ^ EditConditionAttribute.bInvertCondition))
			{
				return EditorGUI.GetPropertyHeight(property, label);
			}
			else
			{
				return -EditorGUIUtility.standardVerticalSpacing;
			}
		}
	}
}
#endif