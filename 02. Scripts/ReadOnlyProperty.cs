using UnityEngine;
using System;



/**
 * @author 이성수
 * @brief 에디터에서 사용 가능한 커스텀 어트리뷰트입니다.
 * @details 사용법은 변수 앞에 [ReadOnlyProperty]입니다.
 * @details 기본적으로 에디터에서 편집 불가능한 채 보여주기만 하는 속성을 정의하고자 할 때 사용합니다.
 * @details 참고: 이전 프로젝트에서 사용하던 것을 가져와 사용
 * @since 2022-11-08
 */
[AttributeUsage(AttributeTargets.Field)]
public class ReadOnlyProperty : PropertyAttribute
{
	/// <summary>
	/// 필드를 읽기 전용으로 만들어 에디터에서 편집할 수 없도록 만듭니다.
	/// </summary>
	public ReadOnlyProperty()
	{

	}
}



#if UNITY_EDITOR
namespace UnityEditor
{
	[CustomPropertyDrawer(typeof(ReadOnlyProperty))]
	public class ReadOnlyDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GUI.enabled = false;
			EditorGUI.PropertyField(position, property, label, true);
			GUI.enabled = true;
		}
	}
}
#endif