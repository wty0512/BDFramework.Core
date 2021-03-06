﻿using System.IO;
using System.Reflection;
using LitJson;
using BDFramework.Sql;
using SQLite4Unity3d;
using UnityEditor;
using UnityEngine;

namespace BDFramework.Editor
{
    static public class Excel2SQLiteTools
    {
        public static void GenSQLite(string outPath)
        {
            var tablePath = Path.Combine(Application.dataPath, "Resource/Table");
            var tableDir = Path.GetDirectoryName(tablePath);
            var xlslFiles = Directory.GetFiles(tableDir, "*.xlsx", SearchOption.AllDirectories);
            //
            if (Directory.Exists(outPath) == false)
            {
                Directory.CreateDirectory(outPath);
            }

            var _path = Path.Combine(outPath, "LocalDB");
            //
            sql = new SQLiteService(SqliteLoder.CreateConnetion(_path));
            foreach (var f in xlslFiles)
            {
                var excel = new ExcelUtility(f);
                var json = excel.GetJson();
                Json2Sqlite(f, json);
            }

            sql.Close();
            EditorUtility.ClearProgressBar();
            Debug.Log("导出Sqlite完成!");
            AssetDatabase.Refresh();
        }

        //数据库准备
        static private SQLiteService sql;

        static private void Json2Sqlite(string f, string json)
        {
            //
            var table = Path.GetFileName(f).Replace(Path.GetExtension(f), "");
            var classname = "Game.Data." + table;
//            Debug.Log("class name：" + classname);
            var jsonObj = JsonMapper.ToObject(json);

            var assPath = Path.Combine(Application.dataPath.Replace("Assets", ""),
                "Library/ScriptAssemblies/Assembly-CSharp.dll");
            var ass = Assembly.LoadFile("file:///" + assPath);
            //
            var t = ass.GetType(classname);


            if (t == null)
            {
                Debug.LogError(classname + "类不存在，请检查!");
            }

            //数据库创建表
            //sql.DB.Delete<>()
            sql.Connection.DropTableByType(t);
            sql.Connection.CreateTableByType(t);

            EditorUtility.ClearProgressBar();
            //
            for (int i = 0; i < jsonObj.Count; i++)
            {
                var j = jsonObj[i];
                var jo = JsonMapper.ToObject(t, j.ToJson());
                EditorUtility.DisplayProgressBar("Excel2Sqlite", "正在导出:" + classname + " " + i + "-" + jsonObj.Count,
                    i / jsonObj.Count);
                try
                {
                    sql.Connection.Insert(jo);
                }
                catch
                {
                    Debug.LogError("导出数据有错,跳过! 错误位置:" + classname + ":" + i + "-" + jsonObj.Count);
                }
            }
        }
    }
}