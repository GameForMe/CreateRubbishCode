using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class LuaCtrl : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		
	}

	/// <summary>
	/// Clears the path file.
	/// 清除路径下的文件;
	/// </summary>
	/// <param name="filePath">File path.</param>
	void ClearPathFile (string filePath)
	{
		if (Directory.Exists (filePath)) {

			string[] fileArr = Directory.GetFiles (filePath);
			for (int k = 0; k < fileArr.Length; k++) {
				string str = fileArr [k];
				File.Delete (str);
			}
			Directory.Delete (filePath, true);
		}
		if (!Directory.Exists (filePath)) {
			Directory.CreateDirectory (filePath);
		}
	}

	#region 生成垃圾

	/// <summary>
	/// Creates the rubbish lua file.
	/// 在目录结构生成相应垃圾;
	/// </summary>
	public void CreateRubbish_LuaFile ()
	{
		usedNameArr.Clear ();
		string filePath = Directory.GetCurrentDirectory () + soruceKey;
		string tarfilePath = Directory.GetCurrentDirectory () + unUseKey;
		ClearPathFile (tarfilePath);
		//按照源文件目录结构生成一些垃圾啊;
		if (Directory.Exists (filePath)) {		

			CreateRubInPath (filePath);
			Debug.Log ("create rubish done");
		}	
	}

	/// <summary>
	/// Creates the rub in path.
	/// 在一个目录结构下写入垃圾;
	/// </summary>
	/// <param name="sourcePath">Source path.</param>
	void CreateRubInPath (string sourcePath)
	{
		//替换关键字得到当前的目标路径;
		string tarfilePath = sourcePath.Replace (soruceKey, unUseKey);
		if (!Directory.Exists (tarfilePath)) {	
			Directory.CreateDirectory (tarfilePath);
		}
		//在当前目录写入垃圾;
		int rubNum = UnityEngine.Random.Range (3, 7);
		for(int j = 0;j<rubNum;j++)
		{
			CreateRubFile (tarfilePath);
		}	

		string[] dirArr = Directory.GetDirectories (sourcePath);
		for (int i = 0; i < dirArr.Length; i++) {
			string childPath =	dirArr [i];

			CreateRubInPath (childPath);
		}
	}

	List<string> usedNameArr = new List<string> ();

	void CreateRubFile (string tarfilePath)
	{
		int nameLen = UnityEngine.Random.Range (5, 10);
		string fileName = ToolsSet.GetRandomFileName (nameLen);
		while (usedNameArr.IndexOf (fileName) >= 0) {
			fileName = ToolsSet.GetRandomFileName (nameLen);
		}

		List<string> funNameArr = new List<string> ();
		string content = "";
		int funNum = UnityEngine.Random.Range (5, 10);
		while (funNum > 0) {
			funNum--;
			content += AddOnePublicFun (funNameArr);
		}
		string filePath = tarfilePath + @"\"+preName+ fileName+".lua";
		File.WriteAllText (filePath,content);
	}

	string AddOnePublicFun (List<string> funNameArr)
	{
		string content = "\n";
		int funNameNum	= UnityEngine.Random.Range (5, 10);

		string funName = ToolsSet. GetRandomFileName (funNameNum);
		while (funNameArr.IndexOf (funName) >= 0) {
			funName = ToolsSet. GetRandomFileName (funNameNum);
		}
		funNameArr.Add (funName);
		int typeNum	= UnityEngine.Random.Range (0, 4);
		if (typeNum == 0) {
			content += "function " + funName + "\n " +
				"{\n" +
				"local asd = 1;\n" +
				"return asd;\n" +
				"end\n ";
		} else if(typeNum == 1) {
			content += "function " + funName + "\n " +
				"{\n" +
				"local ggg = 11;\n" +
				"return ggg;\n" +
				"end\n ";
		}else {			
			content += "function " + funName + "\n " +
			"{\n" +
			"local test = 0;\n" +
			"return test;\n" +
			"end\n ";
		}
		return content;
	}
	#endregion


	string preName = @"zys_rub_unuse_";
	string soruceKey = @"\LuaScript";
	string rubKey = @"\LuaScript_Rub";
	string unUseKey = @"\LuaScript_UnUse";

	#region 注入
	public void InjectionRubbish_ToLua ()
	{
		string filePath = Directory.GetCurrentDirectory () + soruceKey;
		string tarfilePath = Directory.GetCurrentDirectory () + rubKey;
		ClearPathFile (tarfilePath);

		//拷贝源文件到目标位置 并注入垃圾;
		if (Directory.Exists (filePath)) {		
			
			CopyOneDir_ToRubDir (filePath);
			Debug.Log ("injectAll done");
		}
	}

	/// <summary>
	/// Copies the one dir to rub dir.
	/// 注入一个目录下的文件 并展开子目录;
	/// </summary>
	/// <param name="sourcePath">Source path.</param>
	/// <param name="tarfilePath">Tarfile path   这个是根目录.</param>
	void CopyOneDir_ToRubDir (string sourcePath)
	{
		//替换关键字得到当前的目标路径;
		string tarfilePath = sourcePath.Replace (soruceKey, rubKey);
		if (!Directory.Exists (tarfilePath)) {	
			Directory.CreateDirectory (tarfilePath);
		}

		//当前目录下的文件;
		string[] fileArr = Directory.GetFiles (sourcePath);
		for (int k = 0; k < fileArr.Length; k++) {
			string sourceStr = fileArr [k];
			if (sourceStr.Contains (".lua")) {
				int lastIndex = sourceStr.LastIndexOf (@"\");
				string fileName = sourceStr.Substring (lastIndex + 1, sourceStr.Length - lastIndex - 1);
			
				Debug.Log ("insert  " + fileName);	


				CopyOneFileAndInjection (sourceStr, tarfilePath + "/" + fileName);

			}
		}

		string[] dirArr = Directory.GetDirectories (sourcePath);
		for (int i = 0; i < dirArr.Length; i++) {
			string childPath =	dirArr [i];
		
			CopyOneDir_ToRubDir (childPath);
		}

	}

	void CopyOneFileAndInjection (string sourceFilePath, string tarPath)
	{
		string contentStr = "";
		contentStr = File.ReadAllText (sourceFilePath);
		contentStr = "--zys  write sone heheda\n" + contentStr;
		contentStr += "\n  --zys end show some  others";
		if (!File.Exists (tarPath)) {
			FileStream st = File.Create (tarPath);
			st.Close ();
		}
		File.WriteAllText (tarPath, contentStr);
	}
	#endregion

}
