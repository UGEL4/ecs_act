using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class NierTools
{
    [MenuItem("尼尔工具箱/重新命名文件（十六进制转十进制）")]
    public static void RenameFile()
    {
        string _path = EditorUtility.SaveFolderPanel("选择文件夹", "", "");
        string _newPath = _path + "_new";
        if (!Directory.Exists(_newPath))
        {
            Directory.CreateDirectory(_newPath);
        }

        string[] _files = System.IO.Directory.GetFiles(_path);
        foreach (var _filePath in _files)
        {
            var _extension = Path.GetExtension(_filePath);
            var _fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_filePath);
            var _newFileName = _fileNameWithoutExtension;
            string[] _tmp = _fileNameWithoutExtension.Split('_');
            if (_tmp.Length >= 2)
            {
                //进制转换
                int _id = Convert.ToInt32(_tmp[1], 16);
                //保留五位数，列如：00001
                var _format = string.Format("{0:d5}", _id);
                _tmp[1] = _format;
                _newFileName = _tmp[0];
                for (int i = 1; i < _tmp.Length; i++)
                {
                    _newFileName += "_" + _tmp[i];
                }
                var _srcfileName = _newPath + "/" + _newFileName + _extension;
                File.Copy(_filePath, _srcfileName);
            }
            else
            {
                var _srcfileName = _newPath + "/" + _newFileName + _extension;
                File.Copy(_filePath, _srcfileName);
            }
        }

        EditorUtility.DisplayDialog("", "处理完成！", "OK");
    }

    [MenuItem("尼尔工具箱/解析mot文件生成数据")]
    public static void ReadMotFile()
    {
        string _path = EditorUtility.SaveFolderPanel("选择文件夹", "", "");
        var _directoryName = Path.GetFileName(_path);
        int _firstFrame = 1;
        int _lastFrame = _firstFrame - 1;
        List<string> _list = new List<string>();
        string[] _files = System.IO.Directory.GetFiles(_path);
        foreach (var _filePath in _files)
        {
            var _extension = Path.GetExtension(_filePath);
            var _fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_filePath);
            /*
             mot文件头结构
             struct {
                    char      id[4]; // "mot\0"
                    uint32    hash;
                    uint16    flag;
                    int16     frameCount;
                    uint32    recordOffset;
                    uint32    recordNumber;
                    uint32    unknown; // usually 0 or 0x003c0000, maybe two uint16
                    string    animName; // found at most 12 bytes with terminating 0
                } 
             */
            if (_extension.Equals(".mot"))
            {
                var _fByte = File.ReadAllBytes(_filePath);
                //读取动画片段总帧数
                var _frameCount = BitConverter.ToInt16(_fByte, 10);
                _lastFrame += _frameCount;
                _list.Add($"{_firstFrame}|{_lastFrame}|{_fileNameWithoutExtension}");
                _firstFrame += _frameCount;
            }
        }

        File.WriteAllLines(_path + $"/{_directoryName}.txt", _list);
        EditorUtility.DisplayDialog("", "解析成功！", "OK");
    }


    /// <summary>
    /// Clip数据
    /// </summary>
    public class FbxClipData
    {
        public string mName;
        public int mFirstFrame;
        public int mLastFrame;
    }


    [MenuItem("尼尔工具箱/自动拆分FBX动画片段")]
    public static void SplitFbxClip()
    {
        if (EditorUtility.DisplayDialog("自动拆分FBX动画片段", "确认是否已在Project窗口下选中了要处理的FBX", "确定", "取消"))
        {
            UnityEngine.Object[] _objects = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            int _count = 0;
            int _objectsLength = _objects.Length;
            foreach (var obj in _objects)
            {
                _count++;
                string _assetPath = AssetDatabase.GetAssetPath(obj);
                if (Path.GetExtension(_assetPath)?.ToLower() != ".fbx")
                {
                    continue;
                }

                EditorUtility.DisplayProgressBar("正在处理,请稍后", _assetPath, (float)(_count) / _objectsLength);
                List<FbxClipData> _fbxClipDataList = new List<FbxClipData>();

                string _extension = Path.ChangeExtension(_assetPath, "txt");
                if (!File.Exists(_extension))
                {
                    Debug.LogError(_extension + " 数据文件不存在！");
                    continue;
                }

                if (string.IsNullOrEmpty(_extension))
                {
                    continue;
                }

                StreamReader _reader = new StreamReader(_extension);
                string _line;
                while ((_line = _reader.ReadLine()) != null)
                {
                    string[] _info = _line.Split('|');
                    FbxClipData _fbxClipData = new FbxClipData
                    {
                        mFirstFrame = int.Parse(_info[0]),
                        mLastFrame = int.Parse(_info[1]),
                        mName = _info[2]
                    };
                    _fbxClipDataList.Add(_fbxClipData);
                }
                _reader.Close();

                ModelImporter _modelImporter = AssetImporter.GetAtPath(_assetPath) as ModelImporter;
                ArrayList _clipsList = new ArrayList();
                for (int i = 0; i < _fbxClipDataList.Count; i++)
                {
                    ModelImporterClipAnimation _clipAnimation = new ModelImporterClipAnimation
                    {
                        name = _fbxClipDataList[i].mName,
                        firstFrame = _fbxClipDataList[i].mFirstFrame,
                        lastFrame = _fbxClipDataList[i].mLastFrame
                    };
                    _clipsList.Add(_clipAnimation);
                }

                if (_modelImporter != null)
                {
                    _modelImporter.clipAnimations =
                        (ModelImporterClipAnimation[])_clipsList.ToArray(typeof(ModelImporterClipAnimation));
                    _modelImporter.SaveAndReimport();
                }
            }

            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("提示", "处理完成", "OK");
        }
    }

    [MenuItem("Tools/导出AnimationClip")]
    public static void AnimationClipsCopy()
    {
        UnityEngine.Object[] go = Selection.objects;

        string Path = AssetDatabase.GetAssetPath(go[0]);

        string parentPath = getParentPathForAsset(Path);

        for (int i = 0; i < go.Length; i++)
        {
            string fbxPath = AssetDatabase.GetAssetPath(go[i]);

            AnimCopy(fbxPath, parentPath, string.Format("{0}Clone", go[i].name));
        }
    }

    /// <summary>
    /// 返回传入目录的父目录(相对于asset)
    /// </summary>
    /// <param name="assetPath"></param>
    /// <returns></returns>
    public static string getParentPathForAsset(string assetPath)
    {
        string[] pathName = assetPath.Split('/');
        string parentPath = "";

        if (pathName.Length < 2 || pathName[pathName.Length - 1] == "")
        {
            Debug.Log(assetPath + @"没有父目录！");
            return parentPath;
        }

        for (int i = 0; i < pathName.Length - 1; i++)
        {

            if (i != pathName.Length - 2) parentPath += pathName[i] + @"/";
            else
                parentPath += pathName[i];
        }

        return parentPath;
    }
    /*static void AnimCopy(string fbxPath, string parentPath, string name)
    {
        UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
 
        string animationPath = "";
 
        AnimationClipSettings setting;
 
        AnimationClip srcClip;//源AnimationClip
 
        AnimationClip newClip;//新AnimationClip
 
        foreach (UnityEngine.Object o in objs)
        {
            if (o.GetType() == typeof(AnimationClip))
            {
                srcClip = o as AnimationClip;
 
                newClip = new AnimationClip();
 
                newClip.name = srcClip.name;//设置新clip的名字
 
                if (!Directory.Exists(parentPath + @"/copy/"))
 
                    Directory.CreateDirectory(parentPath + @"/copy/");
 
                animationPath = parentPath + @"/copy/" + newClip.name + ".anim";
 
                setting = AnimationUtility.GetAnimationClipSettings(srcClip);//获取AnimationClipSettings
 
                AnimationUtility.SetAnimationClipSettings(newClip, setting);//设置新clip的AnimationClipSettings
 
                newClip.frameRate = srcClip.frameRate;//设置新clip的帧率
 
                EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(srcClip);//获取clip的curveBinds
 
                for (int i = 0; i < curveBindings.Length; i++)
                {
                    AnimationUtility.SetEditorCurve(newClip, curveBindings[i], AnimationUtility.GetEditorCurve(srcClip, curveBindings[i]));//设置新clip的curve
                }
 
                AssetDatabase.CreateAsset(newClip, animationPath); //AssetDatabase中的路径都是相对Asset的  如果指定路径已存在asset则会被删除，然后创建新的asset
 
                AssetDatabase.SaveAssets();//保存修改
 
                AssetDatabase.Refresh();
            }
        }
    }*/


    static void AnimCopy(string fbxPath, string parentPath, string name)
    {
        // 1. 预创建目标目录
        string copyPath = Path.Combine(parentPath, "copy");
        if (!Directory.Exists(copyPath))
        {
            Directory.CreateDirectory(copyPath);
        }

        // 2. 批量加载所有资源（使用更高效的加载方式）
        AnimationClip[] animationClips = AssetDatabase.LoadAllAssetsAtPath(fbxPath)
            .Where(o => o is AnimationClip)
            .Cast<AnimationClip>()
            .ToArray();

        // 3. 使用进度条显示处理状态
        int total = animationClips.Length;
        int processed = 0;


        // 4. 优化处理流程
        EditorApplication.update += ProcessAnimations;

        void ProcessAnimations()
        {
            if (processed >= total)
            {
                // 处理完成
                EditorApplication.update -= ProcessAnimations;
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
                return;
            }

            // 批量处理多个片段（每帧处理5个）
            int batchSize = Mathf.Min(5, total - processed);
            for (int i = 0; i < batchSize; i++)
            {
                AnimationClip srcClip = animationClips[processed];

                // 5. 获取动画片段
                AnimationClip newClip = new AnimationClip
                {
                    name = srcClip.name,
                    frameRate = srcClip.frameRate
                };

                // 6. 复制设置
                AnimationClipSettings setting = AnimationUtility.GetAnimationClipSettings(srcClip);
                AnimationUtility.SetAnimationClipSettings(newClip, setting);

                // 7. 复制曲线
                EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(srcClip);
                foreach (var binding in bindings)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(srcClip, binding);
                    AnimationUtility.SetEditorCurve(newClip, binding, curve);
                }
                // . 一次性设置所有曲线（比循环设置快3-5倍）
                //AnimationUtility.SetEditorCurves(newClip, bindings, curves);



                // 8. 确保文件名唯一
                string animationPath = Path.Combine(copyPath, $"{newClip.name}.anim");
                animationPath = AssetDatabase.GenerateUniqueAssetPath(animationPath);
                // 9. 创建资源
                AssetDatabase.CreateAsset(newClip, animationPath);

                processed++;
            }

            //9. 更新进度（减少GUI开销）
            if (processed % 10 == 0 || processed == total)
            {
                EditorUtility.DisplayProgressBar("Copying Animations",
                    $"Processed {processed}/{total} animations",
                    (float)processed / total);
            }

            // 10. 每处理20个保存一次（减少IO操作）
            if (processed % 20 == 0)
            {
                AssetDatabase.SaveAssets();
            }
        }
    }


    public class NierAnimationClipExportWindow : EditorWindow
    {
        private static string path = "Asset/";
        private static string clipName = string.Empty;

        private static float frameRate = 30f;

        GUIStyle fontSytle1;

        private void OnEnable()
        {
            fontSytle1 = new GUIStyle();
            fontSytle1.fontSize = 15;
            fontSytle1.normal.textColor = Color.yellow;
            fontSytle1.fontStyle = FontStyle.Bold;
            fontSytle1.alignment = TextAnchor.MiddleCenter;
            fontSytle1.wordWrap = true;
            Debug.Log("初始化 ");
        }

        [MenuItem("Nier Tools/导出单个动画片段")]
        public static void ShowWindow()
        {
            var win = EditorWindow.GetWindow(typeof(NierAnimationClipExportWindow));
            win.Show();
        }

        void OnGUI()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Space(10f);
            GUILayout.Label("-导出单个动画工具-", fontSytle1);
            GUILayout.Space(10f);

            clipName = EditorGUILayout.TextField("片段名:", clipName);
            GUILayout.Space(10f);
            frameRate = EditorGUILayout.IntField("帧率:", (int)frameRate);

            //frameRate = EditorGUILayout.IntField("帧率(frameRate):", frameRate);

            UnityEngine.Object[] go = Selection.objects;

            if (GUILayout.Button("Export"))
            {
                if (go.Length == 0)
                {
                    EditorUtility.DisplayDialog("err", "没有在project窗口选中对应的fbx！", "OK");
                    return;
                }

                if (clipName == string.Empty)
                {
                    EditorUtility.DisplayDialog("err", "片段名为空！", "OK");
                    return;
                }

                path = EditorUtility.SaveFilePanel("Save File", Application.dataPath + "/Resources", $"{clipName}.anim", "anim");

                string fbxPath = AssetDatabase.GetAssetPath(go[0]);
                SingleAnimCopy(fbxPath, path, clipName);

            }
            GUILayout.EndVertical();
        }


        static void SingleAnimCopy(string fbxPath, string animationPath, string name)
        {
            // 2. 批量加载所有资源（使用更高效的加载方式）
            AnimationClip[] animationClips = AssetDatabase.LoadAllAssetsAtPath(fbxPath)
                .Where(o => o is AnimationClip)
                .Cast<AnimationClip>()
                .ToArray();

            int total = animationClips.Length;
            for (int i = 0; i < total; i++)
            {
                if (name == animationClips[i].name)
                {
                    AnimationClip srcClip = animationClips[i];
                    AnimationClip newClip = new AnimationClip
                    {
                        name = srcClip.name,
                        //frameRate = srcClip.frameRate
                        frameRate = frameRate
                    };

                    // 复制设置
                    AnimationClipSettings setting = AnimationUtility.GetAnimationClipSettings(srcClip);
                    AnimationUtility.SetAnimationClipSettings(newClip, setting);

                    // 复制曲线
                    EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(srcClip);
                    foreach (var binding in bindings)
                    {
                        AnimationCurve curve = AnimationUtility.GetEditorCurve(srcClip, binding);
                        AnimationUtility.SetEditorCurve(newClip, binding, curve);
                    }

                    // 确保文件名唯一
                    if (animationPath.StartsWith(Application.dataPath))
                    {
                        animationPath = "Assets" + animationPath.Substring(Application.dataPath.Length);
                    }
                    animationPath = AssetDatabase.GenerateUniqueAssetPath(animationPath);
                    // 创建资源
                    AssetDatabase.CreateAsset(newClip, animationPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    break;
                }
            }
        }
    
    }
}


