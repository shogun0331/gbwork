
#if !GB_CORE
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Networking;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using System;

namespace GB
{
    public class Setup : EditorWindow
    {

        [MenuItem("GB/Setup")]
        static void init()
        {
            EditorWindow.GetWindow(typeof(Setup));
        }

        const string URL_NewtonsoftJson = "com.unity.nuget.newtonsoft-json";
        const string URL_NaughtyAttributes = "https://github.com/dbrizov/NaughtyAttributes.git#upm";
        const string URL_UIExtensions = "https://github.com/Unity-UI-Extensions/com.unity.uiextensions.git";
        const string URL_CORE = "https://github.com/shogun0331/gbcore.git";

        const string URL_ASSET = "https://github.com/shogun0331/gbassets.git";


        bool isDownload = false;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 20, position.width - 20, position.height - 20));
            EditorGUIUtil.DrawHeaderLabel("GB Setup");

            GB.EditorGUIUtil.Space(5);

            GB.EditorGUIUtil.DrawSectionStyleLabel("Core Download", 18);

            GB.EditorGUIUtil.Start_VerticalBox();

            GB.EditorGUIUtil.DrawSectionStyleLabel("Newtonsoft-Json", 10);
            GB.EditorGUIUtil.DrawSectionStyleLabel("NaughtyAttributes", 10);
            GB.EditorGUIUtil.DrawSectionStyleLabel("QuickEye-Utility", 10);
            GB.EditorGUIUtil.DrawSectionStyleLabel("UIExtensions", 10);

            GB.EditorGUIUtil.DrawSectionStyleLabel("Common", 10);
            GB.EditorGUIUtil.DrawSectionStyleLabel("UI System", 10);
            GB.EditorGUIUtil.DrawSectionStyleLabel("MVP Pattern", 10);
            GB.EditorGUIUtil.DrawSectionStyleLabel("ODatabase", 10);
            GB.EditorGUIUtil.DrawSectionStyleLabel("Assets Downloder", 10);

            GB.EditorGUIUtil.BackgroundColor(Color.green);
            if (GB.EditorGUIUtil.DrawSyleButton("Download"))
            {

                DownloadCore(() => { isDownload = false; });

            }
            GB.EditorGUIUtil.BackgroundColor(Color.white);

            GB.EditorGUIUtil.End_Vertical();



            GUILayout.EndArea();
        }

        void DownloadCore(Action success)
        {

            DownloadURLPackage(URL_NewtonsoftJson, (result1) =>
            {
                if (!result1) success?.Invoke();
                else
                {

                    DownloadURLPackage(URL_NaughtyAttributes, (result2) =>
                    {
                        if (!result2) success?.Invoke();
                        else
                        {

                            DownloadURLPackage(URL_UIExtensions, (result3) =>
                            {
                                if (!result3) success?.Invoke();
                                else
                                {

                                    DownloadURLPackage(URL_CORE, (result4) =>
                                    {
                                        if (!result4) success?.Invoke();
                                        else
                                        {
                                            

                                            DownloadURLPackage(URL_ASSET, (result5) =>
                                            {
                                                  if (!result5) success?.Invoke();
                                                  else
                                                  {
                                                    success?.Invoke();
                                                    if(!DefineSymbolManager.IsSymbolAlreadyDefined("GB_CORE")) DefineSymbolManager.AddDefineSymbol("GB_CORE");
                                                    Debug.Log("Success");
                                                    Close();
                                                  }
                                            });



                                         
                                            
                                        }
                                    });
                                }
                            });
                        }
                    });
                }
            });
        }
        
        private void DownloadURLPackage(string url, Action<bool> result)
        {

            AddRequest request = UnityEditor.PackageManager.Client.Add(url);
            while (!request.IsCompleted)
            {
                // 필요에 따라 진행 상황을 표시하거나 다른 작업을 수행할 수 있습니다.
                // 예: EditorUtility.DisplayProgressBar("패키지 추가 중...", request.Progress * 100, 100);
            }

            if (request.Status == StatusCode.Success)
            {
                Debug.Log("Package Add Success: " + request.Result.packageId);
                AssetDatabase.Refresh();

            }
            else
            {
                Debug.LogError("Package Add Fail!! : " + request.Error.message);
            }

            result?.Invoke(request.Status == StatusCode.Success);
        }

        private void DownloadUnityPackage(string url, string fileName, Action<bool> result)
        {
            string downloadUrl = url;

            using (var www = UnityWebRequest.Get(downloadUrl))
            {
                www.SendWebRequest();

                while (!www.isDone)
                {
                    Debug.Log("Downloading: " + www.downloadProgress * 100 + "%");
                }

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string savePath = Path.Combine(Application.dataPath, fileName + ".unitypackage");
                    File.WriteAllBytes(savePath, www.downloadHandler.data);
                    Debug.Log("Package downloaded to: " + savePath);

                    AssetDatabase.ImportPackage(savePath, true); // 유니티 프로젝트에 임포트

                    // 파일 삭제
                    File.Delete(savePath);
                    AssetDatabase.Refresh(); // 유니티 에디터에 변경 사항 반영

                }
                else
                {
                    Debug.LogError("Download failed: " + www.error);
                }

                result?.Invoke(www.result == UnityWebRequest.Result.Success);

            }
        }
    }

}

#endif