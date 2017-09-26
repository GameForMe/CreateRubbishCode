using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class JpgCtrl : MonoBehaviour {

	List<byte[]> temArr = new List<byte[]>();
	// Use this for initialization
	void Start () {
		string filePath = Directory.GetCurrentDirectory () + @"\ImgTem";
		if (Directory.Exists (filePath)) {
			string[] fileArr = Directory.GetFiles (filePath);
			for (int k = 0; k < fileArr.Length; k++) {
				string str = fileArr [k];
				using (FileStream fsSource = File.OpenRead ( str)) {
					// Read the source file into a byte array.
					byte[] bytes = new byte[fsSource.Length];
					int numBytesToRead = (int)fsSource.Length;
					int numBytesRead = 0;
					while (numBytesToRead > 0)
					{
						// Read may return anything from 0 to numBytesToRead.
						int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);

						// Break when the end of the file is reached.
						if (n == 0)
							break;

						numBytesRead += n;
						numBytesToRead -= n;
					}
					numBytesToRead = bytes.Length;


					temArr.Add (bytes);
				}
			}
		}

		if(temArr.Count <=0 )
		{
			Debug.LogError ("没有图片模板 " + filePath);
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
	public void CreateRubbish_PngFile ()
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

		List<byte> byteSource = new List<byte>();  

		int funNum = UnityEngine.Random.Range (3, 7);
		while (funNum > 0) {
			funNum--;
			byte[]  bytes = AddOnePublicFun ();
			byteSource.AddRange(bytes);  
		}
		string filePath = tarfilePath + @"\"+preName+ fileName+".png";
		byte[] byteArray = byteSource.ToArray();
		using (FileStream fsNew = new FileStream(filePath,
			FileMode.Create, FileAccess.Write))
		{
			fsNew.Write(byteArray, 0, byteArray.Length);
			fsNew.Close ();
		}
//		File.WriteAllText (filePath,content);
	}

	byte[] AddOnePublicFun ()
	{
		int num	= UnityEngine.Random.Range (0, temArr.Count);
		while(num >= temArr.Count)
		{
			num	= UnityEngine.Random.Range (0, temArr.Count);
		}
 
 
		return temArr[num];
	}
	#endregion
	string preName = @"zys_Img_unuse_";
	string soruceKey = @"\Img";
	string rubKey = @"\Img_Rub";
	string unUseKey = @"\Img_UnUse";
	#region 注入
	public void InjectionRubbish_ToPng ()
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
			if (sourceStr.Contains (".png") || sourceStr.Contains (".jpg")) {
				int lastIndex = sourceStr.LastIndexOf (@"\");
				string fileName = sourceStr.Substring (lastIndex + 1, sourceStr.Length - lastIndex - 1);

				Debug.Log ("insert  " + fileName);	


				CopyOneFileAndInjection (sourceStr, tarfilePath + @"\" + fileName);

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
		using (FileStream fsSource = File.OpenRead (sourceFilePath)) {
			// Read the source file into a byte array.
			byte[] bytes = new byte[fsSource.Length];
			int numBytesToRead = (int)fsSource.Length;
			int numBytesRead = 0;
			while (numBytesToRead > 0)
			{
				// Read may return anything from 0 to numBytesToRead.
				int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);

				// Break when the end of the file is reached.
				if (n == 0)
					break;

				numBytesRead += n;
				numBytesToRead -= n;
			}
			numBytesToRead = bytes.Length;


			string contentStr = "";
			int typeNum	= UnityEngine.Random.Range (0, 4);
			if (typeNum == 0) {
				contentStr = contentStr + "e0ee";
			} else if (typeNum == 1) {
				contentStr = contentStr + "ede2";
			} else {			
				contentStr = contentStr + "7cfe";
			}
			byte[] array   = System.Text.Encoding.Default.GetBytes ( contentStr );
			List<byte> byteSource = new List<byte>();  
			byteSource.AddRange(bytes);  
			byteSource.AddRange(array);  
		byte[] data = byteSource.ToArray();  
//			Array.Copy (array, bytes, array.Length);

			// Write the byte array to the other FileStream.
			using (FileStream fsNew = new FileStream(tarPath,
				FileMode.Create, FileAccess.Write))
			{
								fsNew.Write(data, 0, data.Length);
				fsNew.Close ();
			}
			fsSource.Close ();



//
//			if (!File.Exists (tarPath)) {
//				FileStream st1 = File.Create (tarPath);
//				st1.Close ();
//			}
//			File.WriteAllText (tarPath, contentStr);
		}
	}
	#endregion
}
