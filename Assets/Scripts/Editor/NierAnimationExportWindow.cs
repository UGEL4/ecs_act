using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NierAnimationExportWindow : EditorWindow
{
    public static GameObject targetGo;

    private static AnimationExport baker;

    private static string path = "Asset/";

    private static string animator_name = "Default";

    private static float space = 0f;

    private static int frameRate = 30;

    [SerializeField]//必须要加  
    protected List<UnityEngine.AnimationClip> _animationClips = new List<UnityEngine.AnimationClip>();
    //序列化对象  
    protected SerializedObject _serializedObject;
    //序列化属性  
    protected SerializedProperty _assetLstProperty;

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
        //使用当前类初始化  
        _serializedObject = new SerializedObject(this);
        //获取当前类中可序列话的属性  
        _assetLstProperty = _serializedObject.FindProperty("_animationClips");
    }

    [MenuItem("Art Tools/AnimationExport")]
    public static void ShowWindow()
    {
        var win = EditorWindow.GetWindow(typeof(NierAnimationExportWindow));
        baker = new AnimationExport();
        win.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical("box");
        GUILayout.Space(10f);
        GUILayout.Label("-批量合批动画工具-", fontSytle1);
        GUILayout.Space(10f);

        targetGo = (GameObject)EditorGUILayout.ObjectField("目标动画机(Animator):", targetGo, typeof(GameObject), true);

        animator_name = EditorGUILayout.TextField("输出动作名称(name):", animator_name);

        path = EditorGUILayout.TextField("输出路径(path):", path);

        space = EditorGUILayout.FloatField("动作间隔(space/s):", space);

        frameRate = EditorGUILayout.IntField("帧率(frameRate):", frameRate);

        var animationPath = "Assets/" + animator_name + ".anim";

        if (!string.IsNullOrEmpty(path) &&
            !string.IsNullOrEmpty(System.IO.Path.GetDirectoryName(path)) &&
            System.IO.Directory.Exists(path)
            )
        {
            animationPath = path;
        }

        GUILayout.Label("输出路径(output_path):" + animationPath);

        if (GUILayout.Button("Generate"))
        {
            if (targetGo == null)
            {
                EditorUtility.DisplayDialog("err", "目标动画机 is null！", "OK");
                return;
            }

            if (baker == null)
            {
                baker = new AnimationExport();
            }

            var animator = targetGo.GetComponent<Animator>();

            if (animator == null)
            {
                EditorUtility.DisplayDialog("err", "Animator is null！", "OK");
                return;
            }

            baker.Export(animator, animator_name, space, frameRate, animationPath);
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        GUILayout.Space(10f);
        GUILayout.Label("-批量生成Fbx工具-", fontSytle1);
        GUILayout.Space(10f);

        targetGo = (GameObject)EditorGUILayout.ObjectField("目标物体", targetGo, typeof(GameObject), true);

        if (GUILayout.Button("Initialize"))
        {
            UnityEngine.Object[] _objects = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            foreach (var obj in _objects)
            {
                if (obj.GetType() != typeof(AnimationClip))
                    continue;

                AnimationClip clipGo = (AnimationClip)obj;
                _animationClips.Add(clipGo);
            }
        }

        if (GUILayout.Button("Generate"))
        {
            if (targetGo == null)
            {
                EditorUtility.DisplayDialog("err", "目标物体为空！", "OK");
                return;
            }

            if (baker == null)
            {
                baker = new AnimationExport();
            }

            if (_animationClips.Count <= 0)
            {
                EditorUtility.DisplayDialog("err", "AnimationClips is null！", "OK");
                return;
            }

            baker.Export(targetGo, _animationClips, animator_name);
        }

        //更新  
        _serializedObject.Update();

        //开始检查是否有修改  
        EditorGUI.BeginChangeCheck();

        //显示属性  
        //第二个参数必须为true，否则无法显示子节点即List内容  
        EditorGUILayout.PropertyField(_assetLstProperty, true);

        //结束检查是否有修改  
        if (EditorGUI.EndChangeCheck())
        {//提交修改  
            _serializedObject.ApplyModifiedProperties();
        }

        GUILayout.EndVertical();
    }
}

public class AnimationExport
{
    private float index;

    /// <summary>
    /// name为动作名称
    /// space为每个动作之间的间隔,秒为单位
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="name"></param>
    /// <param name="space"></param>
    public void Export(Animator animator, string name, float space, int frameRate, string path)
    {
        var animationPath = "Assets/" + name + ".anim";

        if (AssetDatabase.IsValidFolder(path))
        {
            animationPath = path;
        }

        var output_clip = new AnimationClip()
        {
            name = name,
            legacy = false,
            wrapMode = WrapMode.Once,
            frameRate = frameRate
        };

        index = 0;

        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        AnimationClip[] ori_clips = ac.animationClips;
        if (null == ori_clips || ori_clips.Length <= 0) return;
        AnimationClip current_clip;
        for (int i = 0, length = ori_clips.Length; i < length; i++)
        {
            current_clip = ac.animationClips[i];
            if (current_clip != null)
            {
                float keys_lenght = AnimCopy(current_clip, ref output_clip, index);
                index += keys_lenght + space;
            }
        }

        AssetDatabase.CreateAsset(output_clip, animationPath); //AssetDatabase中的路径都是相对Asset的  如果指定路径已存在asset则会被删除，然后创建新的asset

        AssetDatabase.SaveAssets();//保存修改

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// name为动作名称
    /// space为每个动作之间的间隔,秒为单位
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="name"></param>
    /// <param name="space"></param>
    public void Export(GameObject obj, List<AnimationClip> clips, string name)
    {
        AnimationClip current_clip;
        for (int i = 0, length = clips.Count; i < length; i++)
        {
            current_clip = clips[i];
            if (current_clip != null)
            {
                current_clip.legacy = true;

                var _animation = obj.GetComponent<Animation>();
                if (_animation != null)
                {
                    GameObject.DestroyImmediate(_animation);
                }

                _animation = obj.AddComponent<Animation>();

                var _name = current_clip.name;
                var animationPath = "Assets/" + _name + ".fbx";
                _animation.AddClip(current_clip, _name);
                _animation.clip = current_clip;
                UnityEditor.Formats.Fbx.Exporter.ModelExporter.ExportObject(animationPath, _animation);
            }
        }


    }

    float AnimCopy(AnimationClip srcClip, ref AnimationClip outputClip, float start_index)
    {
        //AnimationClipSettings setting = AnimationUtility.GetAnimationClipSettings(srcClip);//获取AnimationClipSettings

        //AnimationUtility.SetAnimationClipSettings(newClip, setting);//设置新clip的AnimationClipSettings

        //newClip.frameRate = srcClip.frameRate;//设置新clip的帧率

        float keys_count_max = 0;

        EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(srcClip);//获取clip的curveBinds

        keys_count_max = srcClip.length;

        for (int i = 0; i < curveBindings.Length; i++)
        {
            AnimationCurve ori_curve = AnimationUtility.GetEditorCurve(srcClip, curveBindings[i]);

            AnimationCurve output_curve = AnimationUtility.GetEditorCurve(outputClip, curveBindings[i]);

            AnimationCurve new_curve = new AnimationCurve();

            if (output_curve != null && output_curve.length > 0)
            {
                for (int j = 0; j < output_curve.keys.Length; j++)
                {
                    new_curve.AddKey(output_curve.keys[j].time, output_curve.keys[j].value);
                }
            }

            if (ori_curve != null && ori_curve.length > 0)
            {
                for (int j = 0; j < ori_curve.keys.Length; j++)
                {
                    new_curve.AddKey(start_index + ori_curve.keys[j].time, ori_curve.keys[j].value);
                }
            }

            AnimationUtility.SetEditorCurve(outputClip, curveBindings[i], new_curve);
        }

        return keys_count_max;
    }

}