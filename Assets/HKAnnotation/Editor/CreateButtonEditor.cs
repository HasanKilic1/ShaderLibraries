using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HKAnnotations
{
    [CustomEditor(typeof(MonoBehaviour), editorForChildClasses:true) , CanEditMultipleObjects]
    public class CreateButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MonoBehaviour monoBehaviour = (MonoBehaviour)target;

            // get method infos
            MethodInfo[] methods = monoBehaviour.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            EditorGUILayout.BeginVertical();

            foreach (MethodInfo methodInfo in methods)
            {
                if (methodInfo.GetCustomAttribute(typeof(ExecuteByButtonClick)) != null)
                {
                    string buttonName = $"Execute {methodInfo.Name}";

                    ExecuteByButtonClick annotation = methodInfo.GetCustomAttribute(typeof(ExecuteByButtonClick)) as ExecuteByButtonClick;
                    if (annotation.Description != null && annotation.Description.Length > 0)
                    {
                        buttonName = annotation.Description;
                    }

                    if (GUILayout.Button(buttonName))
                    {
                        methodInfo.Invoke(monoBehaviour, null);

                    }
                }
            }

            EditorGUILayout.EndVertical();
        }
    }

}

