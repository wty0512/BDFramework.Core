﻿using System.Collections;
using System.Collections.Generic;
using BDFramework.Editor.Tools;
using UnityEditor;
using UnityEngine;

public class EditorWindow_Table : EditorWindow
{
  public void OnGUI()
  {
    GUILayout.BeginVertical();
    GUILayout.Label("3.表格打包",EditorGUIHelper.TitleStyle);
    GUILayout.Space(5);
    if (GUILayout.Button("表格导出成Sqlite" ,GUILayout.Width(300),GUILayout.Height(30)))
    {
      
    }
    GUILayout.EndVertical();
  }
}
