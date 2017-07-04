using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class main : MonoBehaviour
{

	public int rubishNum = 10;

	// Use this for initialization
	void Start ()
	{
		
	}

	public void CreateRubbishFile ()
	{
		string filePath = Application.dataPath + "/../zysCodePjt";
		createFileToDir (filePath);
	}

	public void InjectionRubbish ()
	{
		string filePath = Application.dataPath + "/source";
		string tarfilePath = Application.dataPath + "/../tarSource";
		ClearPathFile (tarfilePath);
		//拷贝源文件到目标位置 并注入垃圾;
		//只操作根目录下的。子目录下的不管;
		if (Directory.Exists (filePath)) {

			string[] fileArr = Directory.GetFiles (filePath);
			for (int k = 0; k < fileArr.Length; k++) {
				string sourceStr = fileArr [k];
				if (sourceStr.Contains (".meta") || sourceStr.Contains (".DS_Store")) {
					continue;
				}
				int lastIndex = sourceStr.LastIndexOf ("/");
				string fileName = sourceStr.Substring (lastIndex + 1, sourceStr.Length - lastIndex - 1);

				string fileDirStr = sourceStr.Substring (0, lastIndex + 1);
				if (fileName.Substring (0, 1) == ".") {
					continue;
				}

				if (fileName.Contains (".h")) {//只有头文件才进去。然后看有没有cpp 文件;
					string cppName = fileName.Substring (0, fileName.Length - 2);
					string cppNameStr = fileDirStr + cppName + ".cpp";
					if (File.Exists (cppNameStr)) {
						fileName.Contains (".cpp");
						Debug.Log ("insert  " + fileName);	

						string tarStr = fileDirStr.Replace ("source", "../tarSource");
							
						CopyOneFileAndInjection (fileDirStr, tarStr ,cppName);
					}
				}
			
			}
		}
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
			Directory.Delete (filePath);
		}
		if (!Directory.Exists (filePath)) {
			Directory.CreateDirectory (filePath);
		}
	}

	List<string> usedNameArr = new List<string> ();

	void createFileToDir (string filePath)
	{
		usedNameArr.Clear ();
		ClearPathFile (filePath);
	

		FileStream fs_zH = new FileStream (filePath + "/mainZys.h", FileMode.OpenOrCreate, FileAccess.ReadWrite); //可以指定盘符，也可以指定任意文件名，还可以为word等文件
		FileStream fs_zCpp = new FileStream (filePath + "/mainZys.cpp", FileMode.OpenOrCreate, FileAccess.ReadWrite); //可以指定盘符，也可以指定任意文件名，还可以为word等文件
		StreamWriter sw_zH = new StreamWriter (fs_zH); // 创建写入流
		StreamWriter sw_zCpp = new StreamWriter (fs_zCpp); // 创建写入流

		sw_zH.WriteLine ("class mainZys"); // 写入
		sw_zH.WriteLine ("{"); // 写入
		sw_zH.WriteLine ("public:"); // 写入
		sw_zH.WriteLine ("int zysCode;"); // 写入
		sw_zH.WriteLine ("void zysMain();"); // 写入
		sw_zH.WriteLine ("};"); // 写入jieshu

		sw_zCpp.WriteLine ("#include \"mainZys.h\""); // 写入jieshu
		for (int i = 0; i < rubishNum; i++) {
			List<string> valNameArr = new List<string> ();
			List<string> funNameArr = new List<string> ();
			int num = UnityEngine.Random.Range (5, 10);
			string fileName = GetRandomFileName (num);
			while (usedNameArr.IndexOf (fileName) >= 0) {
				fileName = GetRandomFileName (num);
			}
			Debug.Log ("file " + fileName);
			//=========  开始创建一对匹配的文件
			// 创建文件
			FileStream fs_H = new FileStream (filePath + "/" + fileName + ".h", FileMode.OpenOrCreate, FileAccess.ReadWrite); //可以指定盘符，也可以指定任意文件名，还可以为word等文件
			FileStream fs_Cpp = new FileStream (filePath + "/" + fileName + ".cpp", FileMode.OpenOrCreate, FileAccess.ReadWrite); //可以指定盘符，也可以指定任意文件名，还可以为word等文件

			StreamWriter sw_H = new StreamWriter (fs_H); // 创建写入流
			StreamWriter sw_Cpp = new StreamWriter (fs_Cpp); // 创建写入流
			sw_H.WriteLine ("class " + fileName); // 写入
			sw_H.WriteLine ("{"); // 写入
			sw_H.WriteLine ("public:"); // 写入

			//cpp 头写入;
			sw_Cpp.WriteLine ("#include \"" + fileName + ".h\""); // 写入
			sw_zCpp.WriteLine ("#include \"" + fileName + ".h\""); // 写入
			int valNum = UnityEngine.Random.Range (2, 10);
			for (int j = 0; j < valNum; j++) {
				string funName = AddOnePublicProp (sw_H, sw_Cpp, fileName, valNameArr);
				valNameArr.Add (funName);
			}

			int funNum = UnityEngine.Random.Range (2, 10);
			for (int j = 0; j < funNum; j++) {
				string funName = AddOnePublicFun (sw_H, sw_Cpp, fileName, funNameArr, valNameArr);
				funNameArr.Add (funName);
			}
			sw_H.WriteLine ("};"); // 写入jieshu
			sw_H.Close (); //关闭文件
			sw_Cpp.Close (); //关闭文件
		}
		sw_zCpp.WriteLine ("//start main"); // 写入jieshu
		sw_zCpp.WriteLine ("void mainZys::zysMain(){}"); // 写入
		sw_zH.Close (); //关闭文件
		sw_zCpp.Close (); //关闭文件
	}

	string AddOnePublicProp (StreamWriter sw_H, StreamWriter sw_Cpp, string fileName, List<string> valNameArr)
	{
		int valNamenum = UnityEngine.Random.Range (5, 10);
		string valName = GetRandomFileName (valNamenum);
		while (valNameArr.IndexOf (valName) >= 0) {
			valName = GetRandomFileName (valNamenum);
		}
	
		sw_H.WriteLine ("int " + valName + ";"); // 写入
		return valName;
	}

	string AddOnePublicFun (StreamWriter sw_H, StreamWriter sw_Cpp, string fileName, List<string> funNameArr, List<string> valNameArr)
	{
		int funNameNum	= UnityEngine.Random.Range (5, 10);

		string funName = GetRandomFileName (funNameNum);
		while (funNameArr.IndexOf (funName) >= 0||valNameArr.IndexOf (funName) >= 0) {
			funName = GetRandomFileName (funNameNum);
		}
	
		//写几种可能的函数;
		int funType = UnityEngine.Random.Range (0, 5);
		int indexNum = UnityEngine.Random.Range (0, valNameArr.Count); 
		string str = valNameArr [indexNum];
		switch (funType) {
		case 0:
			sw_H.WriteLine ("void " + funName + "(bool state);"); // 写入

			sw_Cpp.WriteLine ("void "+fileName + "::" + funName + "(bool state)"); // 写入
			sw_Cpp.WriteLine ("{"); // 写入
			sw_Cpp.WriteLine ("int i = 0;"); // 写入
			sw_Cpp.WriteLine ("if(state )"); // 写入
			sw_Cpp.WriteLine (" i = " + UnityEngine.Random.Range (0, 100) + ";"); // 写入
			sw_Cpp.WriteLine ("}"); // 写入
			break;
		case 1:
			sw_H.WriteLine ("void " + funName + "(int num);"); // 写入

			sw_Cpp.WriteLine ("void "+fileName + "::" + funName + "(int num)"); // 写入
			sw_Cpp.WriteLine ("{"); // 写入
			sw_Cpp.WriteLine (str+" += num;"); // 写入
			sw_Cpp.WriteLine ("}"); // 写入
			break;
		case 2:
			sw_H.WriteLine ("void " + funName + "(int num);"); // 写入

			sw_Cpp.WriteLine ("void "+fileName + "::" + funName + "(int num)"); // 写入
			sw_Cpp.WriteLine ("{"); // 写入
			sw_Cpp.WriteLine (str+" -= num;"); // 写入
			sw_Cpp.WriteLine ("}"); // 写入
			break;
		case 3:
			sw_H.WriteLine ("void " + funName + "(int num);"); // 写入

			sw_Cpp.WriteLine ("void "+fileName + "::" + funName + "(int num)"); // 写入
			sw_Cpp.WriteLine ("{"); // 写入
			sw_Cpp.WriteLine (str+" = num * "+str+";"); // 写入
			sw_Cpp.WriteLine ("}"); // 写入
			break;
		case 4:
			sw_H.WriteLine ("void " + funName + "(int num);"); // 写入

			sw_Cpp.WriteLine ("void "+fileName + "::" + funName + "(int num)"); // 写入
			sw_Cpp.WriteLine ("{"); // 写入
			sw_Cpp.WriteLine (str+" = (num +"+str+" )/2;"); // 写入
			sw_Cpp.WriteLine ("}"); // 写入
			break;
		}

		return funName;

	}


	string GetRandomFileName (int num)
	{
		System. Random r = new System.Random ();
		string s = string.Empty;
		string str = string.Empty;
		for (int i = 0; i < num; i++) {
			s = ((char)r.Next (97, 123)).ToString ();
			str += s;
		}
		return str;
	}


	//只有 h 文件和cpp 文件需要; 需要成对出现 要匹配;
	//首先找到需要插入的 类名 找class;
	void CopyOneFileAndInjection (string sourceFile, string destinationFile,string cppName)
	{
		//读取头文件结构;
		string destHPath = destinationFile + cppName + ".h";
		FileInfo file = new FileInfo (sourceFile+cppName+".h");
		string contentStr = "";
		if (file.Exists) {
			// true is overwrite
			contentStr = File.ReadAllText (sourceFile+cppName+".h");
		} 
		bool isCanInsert = false;
		int friendClassIndex = contentStr.LastIndexOf ("friend class");
		//找到最后的一个class;
		int lastClassIndex = contentStr.LastIndexOf ("class");
		if(friendClassIndex < 0)
		{
			if (lastClassIndex >= 0) {

				string classPreStr = contentStr.Substring (0, lastClassIndex);
				int lastlineIndex = classPreStr.LastIndexOf ("\n");
				//行注释掉的话;
				if (lastlineIndex >= 0) {
					string classlineStr = contentStr.Substring (lastlineIndex, lastClassIndex - lastlineIndex);
					int zhushiIndex = classlineStr.IndexOf ("//");
					if (zhushiIndex < 0) {
						isCanInsert = true;
					}
				}

				int prelastClassIndex = classPreStr.LastIndexOf ("class");
				int lastKuaiZhushiIndex = classPreStr.LastIndexOf ("/*");
				int preendKuaiZhushiIndex = classPreStr.IndexOf ("*/");
			
				//在注视块之间 的class 不被相应;
				//;
				if ((lastKuaiZhushiIndex > prelastClassIndex || prelastClassIndex == -1)// 上个注视块在class 之后;
				   && (preendKuaiZhushiIndex < lastKuaiZhushiIndex || preendKuaiZhushiIndex == -1)//并且注视没有结束;) {
				)
				{
						isCanInsert = false;
				} else {
						isCanInsert = true;
				}
			}
		}

		if(isCanInsert)
		{
			string classStr = contentStr.Substring (lastClassIndex + 5, contentStr.Length - lastClassIndex - 5);
			classStr = classStr.Trim ();
			int konggeIndex = classStr.IndexOf (" ");
			int maohaoIndex = classStr.IndexOf (":");
			int endIndex = classStr.IndexOf ("\n");
			int zhushiIndex1 = classStr.IndexOf ("/");

			if(konggeIndex <0 )
			{
				konggeIndex = 9999;
			}
			if(maohaoIndex <0 )
			{
				maohaoIndex = 9999;
			}
			if(endIndex <0 )
			{
				endIndex = 9999;
			}
			if(zhushiIndex1 <0 )
			{
				zhushiIndex1 = 9999;
			}
			int realIndex = Math.Min (konggeIndex, maohaoIndex);
			realIndex = Math.Min (realIndex, endIndex);
			realIndex = Math.Min (zhushiIndex1, realIndex);
			string className = classStr.Substring (0, realIndex);
			int valnum = UnityEngine.Random.Range (4, 8);
			List<string> valNameArr = new List<string> ();
			List<string> funNameArr = new List<string> ();
			string propValStr = "\r\n";
			string funValStr = "\r\n";
			Debug.Log ("-===== valnum " + valnum);
			for (int i = 0; i < valnum; i++) {
				string propName = CreateOnePublicProp (valNameArr);
				valNameArr.Add (propName);
				propValStr += "int " + propName + ";\r\n"; // 写入;
				Debug.Log (i + " add prop >> " + propName);
			}

			int funNum = UnityEngine.Random.Range (4, 8);
			Debug.Log ("-===== funNum " + funNum);
			for (int j = 0; j < funNum; j++) {
				string[] funObjArr = CreateOneFunStr (className, funNameArr, valNameArr);
				string funName = funObjArr [2];
				string funHStr = funObjArr [0];
				string funCppStr = funObjArr [1];
				propValStr += funHStr;
				funValStr += funCppStr;
				funNameArr.Add (funName);
				Debug.Log (j + " add fun >> " + funName);
			}


			int lastIndex = classStr.IndexOf ("public:");
			string remiastr = "";
			if (lastIndex >= 0) {
				remiastr = contentStr.Insert (lastClassIndex + lastIndex + 13, "\r\n//zys  code !!!!!!!!!!!\r\n" + propValStr);
			} else {
				int insetIndex = classStr.IndexOf ("{");
				remiastr = contentStr.Insert (lastClassIndex + insetIndex + 7, "\r\n//zys  code !!!!!!!!!!!\r\npublic:\r\n" + propValStr);
			}

			string destCppPath = destinationFile + cppName + ".cpp";
			FileInfo fileCpp = new FileInfo (sourceFile + cppName + ".cpp");
			string contentStrCpp = "";
			if (fileCpp.Exists) {
				// true is overwrite
				//			fileCpp.CopyTo (destCppPath, true);
				contentStrCpp = File.ReadAllText (sourceFile + cppName + ".cpp");
			} 

			//		 contentStrCpp = File.ReadAllText (destCppPath);

			string remiastrCpp = "";

			remiastrCpp = contentStrCpp + "\r\n//zys  code !!!!!!!!!!!\r\n" + funValStr;
			//
			//		File.WriteAllText (destCppPath, remiastrCpp);



			FileStream fs_zH = new FileStream (destHPath, FileMode.OpenOrCreate, FileAccess.ReadWrite); //可以指定盘符，也可以指定任意文件名，还可以为word等文件
			FileStream fs_zCpp = new FileStream (destCppPath, FileMode.OpenOrCreate, FileAccess.ReadWrite); //可以指定盘符，也可以指定任意文件名，还可以为word等文件
			StreamWriter sw_zH = new StreamWriter (fs_zH); // 创建写入流
			StreamWriter sw_zCpp = new StreamWriter (fs_zCpp); // 创建写入流
			sw_zH.Write (remiastr);
			sw_zCpp.Write (remiastrCpp);
			sw_zH.Close ();
			sw_zCpp.Close ();
		} else {
			Debug.LogWarning ("skip file cppName " +cppName);
		}

	}

	string CreateOnePublicProp ( List<string> valNameArr)
	{
		int valNamenum = UnityEngine.Random.Range (5, 10);
		string valName = GetRandomFileName (valNamenum);
		while (valNameArr.IndexOf (valName) >= 0) {
			valName = GetRandomFileName (valNamenum);
		}


		return valName;
	}

	string[] CreateOneFunStr(string className, List<string> funNameArr, List<string> valNameArr)
	{
		int funNameNum	= UnityEngine.Random.Range (5, 10);

		string funName = GetRandomFileName (funNameNum);
		while (funNameArr.IndexOf (funName) >= 0 ||valNameArr.IndexOf (funName) >= 0 ) {
			funName = GetRandomFileName (funNameNum);
		}
		string funStr = "\r\n";
		string funHStr = "\r\n";
		//写几种可能的函数;
		int funType = UnityEngine.Random.Range (0, 5);
		int indexNum = UnityEngine.Random.Range (0, valNameArr.Count); 
		string str = valNameArr [indexNum];
		switch (funType) {
		case 0:
			funHStr += ("void " + funName + "(bool state);\r\n"); // 写入

			funStr += "void "+className + "::" + funName + "(bool state)\r\n"; // 写入
			funStr += "{\r\n"; // 写入
			funStr += ("int i = 0;\r\n"); // 写入
			funStr += ("if(state )\r\n"); // 写入
			funStr += (" i = " + UnityEngine.Random.Range (0, 100) + ";\r\n"); // 写入
			funStr += ("}\r\n"); // 写入
			break;
		case 1:
			funHStr+= ("void " + funName + "(int num);\r\n"); // 写入

			funStr += "void "+(className + "::" + funName + "(int num)\r\n"); // 写入
			funStr += ("{\r\n"); // 写入
			funStr += (str+" += num;\r\n"); // 写入
			funStr += ("}\r\n"); // 写入
			break;
		case 2:
			funHStr+=  ("void " + funName + "(int num);\r\n"); // 写入

			funStr += "void "+(className + "::" + funName + "(int num)\r\n"); // 写入
			funStr += ("{\r\n"); // 写入
			funStr += (str+" -= num;\r\n"); // 写入
			funStr += ("}\r\n"); // 写入
			break;
		case 3:
			funHStr+= ("void " + funName + "(int num);\r\n"); // 写入

			funStr += "void "+(className + "::" + funName + "(int num)\r\n"); // 写入
			funStr += ("{\r\n"); // 写入
			funStr +=(str+" = num * "+str+";\r\n"); // 写入
			funStr +=("}\r\n"); // 写入
			break;
		case 4:
			funHStr+=  ("void " + funName + "(int num);\r\n"); // 写入

			funStr +="void "+(className + "::" + funName + "(int num)\r\n"); // 写入
			funStr += ("{\r\n"); // 写入
			funStr +=(str+" = (num +"+str+" )/2;\r\n"); // 写入
			funStr +=("}\r\n"); // 写入
			break;
		}
		string[] ctStrArr = new string[3];
		ctStrArr [0] = funHStr;
		ctStrArr [1] = funStr;
		ctStrArr [2] = funName;
		return ctStrArr;
	}

}
