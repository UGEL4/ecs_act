using ACTGame.Action;
using UnityEditor;
using UnityEngine;

// [CustomEditor(typeof(ActionEditor))]
// public class ActionEditorEditorInspector : Editor
// {
//     private int currentFrame = 0;
//     private int maxFrame     = 60;

//     [SerializeField]
//     // private EditActionObject actionInfo;

//     void OnEnable()
//     {
//         // if (actionInfo == null)
//         // {
//         //     actionInfo = new EditActionObject();
//         // }
//     }

//     void OnDisable()
//     {
//         currentFrame = 0;
//         maxFrame     = 0;
//         // actionInfo   = null;
//     }

//     public override void OnInspectorGUI()
//     {
//         // EditorGUILayout.ObjectField("动作文件：", actionInfo, typeof(EditActionObject), false);
//         currentFrame = EditorGUILayout.IntSlider("当前帧：", currentFrame, 0, maxFrame);
//         EditorGUILayout.BeginHorizontal();
//         if (GUILayout.Button("初始化"))
//         {
//             Bake();
//         }
//         if (GUILayout.Button("播放"))
//         {
//             // Bake();
//         }
//         if (GUILayout.Button("停止"))
//         {
//             // Bake();
//         }
//         EditorGUILayout.EndHorizontal();
//     }

//     void Bake()
//     {
//         // if (actionInfo == null)
//         // {
//         //     Debug.LogError("没有选择动作文件");
//         //     return;
//         // }

//         // maxFrame = actionInfo.value.FrameCount;
//     }
// }

public class ActionEditorWindow : EditorWindow
{
    private ActionConfig actionInfo;
    private int currentFrame;
    private int lastFrame;
    private int maxFrame;

    private bool isDirty;
    private bool isPlaying = false;

    private GameObject character;
    private int characterId;
    private Animator animator;
    private AnimationClip animation;
    private AnimationClip lastAnimation;
    private double lastTime = 0;
    private float frameRate = 1 / 60f;
    private int runingFrame = 0;
    // 动画预览状态
    private AnimatorOverrideController overrideController;
    private bool controllerOverridden = false;

    [MenuItem("Tools/Action/ActionEditorWindow")]
    public static void ShowWindow()
    {
        GetWindow<ActionEditorWindow>("EditAction");
    }

    private Editor _embeddedEditor;

    void OnGUI()
    {
        if (isDirty)
        {
            EditorGUILayout.HelpBox("有未保存的修改", MessageType.Warning);
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("加载"))
        {
            Load();
        }
        if (GUILayout.Button("播放"))
        {
            PlayAction(true);
        }
        if (GUILayout.Button("停止"))
        {
            PlayAction(false);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        character = (GameObject)EditorGUILayout.ObjectField("预览角色", character, typeof(GameObject), true);
        animation = (AnimationClip)EditorGUILayout.ObjectField("动画片段", animation, typeof(AnimationClip), true);
        if (animation)
        {
            int totalFrame = (int)(animation.length * animation.frameRate);
            EditorGUILayout.IntField("动画总帧数：", totalFrame );
            EditorGUILayout.FloatField("动画帧率：", animation.frameRate);
            EditorGUILayout.IntField("动作帧率：", 60);
        }
        EditorGUILayout.EndVertical();

        if (character)
        {
            if (characterId != character.GetInstanceID())
            {
                RestoreOriginalAnimation();
                animator = character.GetComponentInChildren<Animator>(true);
            }
            else
            {
                if (animator == null)
                {
                    animator = character.GetComponentInChildren<Animator>(true);
                }
            }
        }

        if (animation && animation != lastAnimation)
        {
            RestoreOriginalAnimation();
            lastAnimation = animation;
        }

        if (actionInfo != null)
        {
            if (maxFrame != actionInfo.FrameCount - 1)
            {
                maxFrame = actionInfo.FrameCount - 1;
                if (currentFrame > maxFrame)
                {
                    currentFrame = maxFrame;
                }
            }
            EditorGUILayout.BeginHorizontal();
            currentFrame = EditorGUILayout.IntSlider("当前帧：", currentFrame, 0, maxFrame);
            EditorGUILayout.EndHorizontal();
            // 创建或更新内嵌 Editor
            if (_embeddedEditor == null || _embeddedEditor.target != actionInfo)
            {
                DestroyImmediate(_embeddedEditor);
                _embeddedEditor = Editor.CreateEditor(actionInfo);
            }

            // 检测修改
            EditorGUI.BeginChangeCheck();
            if (actionInfo && animation && actionInfo.Animation != animation.name)
            {
                actionInfo.Animation = animation.name;
            }
            // 绘制内嵌 Editor
            _embeddedEditor.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                isDirty = true;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("保存"))
            {
                SaveChange();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    void Load()
    {
        if (actionInfo != null)
        {
            if (!EditorUtility.DisplayDialog("提示", "是否选择新的动作文件？未保存的修改将丢失", "确定", "取消"))
            {
                return;
            }
        }
        string resources = "/Resources/";
        string path      = EditorUtility.OpenFilePanel("选择动作文件", Application.dataPath + resources + "Prefabs/ScriptableObjects/ActionConfig", "asset");
        if (string.IsNullOrEmpty(path))
        {
            EditorUtility.DisplayDialog("提示", "没有选择动作文件", "确定");
            return;
        }

        string relativePath = "Assets" + path.Substring(Application.dataPath.Length);
        actionInfo          = AssetDatabase.LoadAssetAtPath<ActionConfig>(relativePath);
        if (actionInfo != null)
        {
            currentFrame = 0;
            maxFrame     = actionInfo.FrameCount;
        }
    }

    void SaveChange()
    {
        if (actionInfo != null && isDirty)
        {
            // 保存修改
            EditorUtility.SetDirty(actionInfo);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            isDirty = false;
            Debug.Log("保存成功: " + actionInfo.name);
        }
    }

    void SaveAs()
    {
        if (actionInfo == null) return;

        // 获取默认保存路径
        string defaultPath = "Assets/Resources/ActionData/";
        string defaultName = actionInfo.name + "_Copy";

        // 打开保存对话框
        string path = EditorUtility.SaveFilePanelInProject(
        "另存为",
        defaultName,
        "asset",
        "选择保存位置",
        defaultPath);

        if (!string.IsNullOrEmpty(path))
        {
            // 创建副本
            ActionConfig newAsset = Instantiate(actionInfo);

            // 保存新资源
            AssetDatabase.CreateAsset(newAsset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 加载新资源
            actionInfo = AssetDatabase.LoadAssetAtPath<ActionConfig>(path);

            // 重新创建嵌入编辑器
            if (_embeddedEditor != null) DestroyImmediate(_embeddedEditor);
            _embeddedEditor = Editor.CreateEditor(actionInfo);

            isDirty = false;
            Debug.Log("另存为成功: " + path);
        }
    }

    void OnEnable()
    {
        lastTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += OnUpdate;

        AnimationMode.StartAnimationMode();
    }
    void OnDisable()
    {
        RestoreOriginalAnimation();
        AnimationMode.StopAnimationMode();
        EditorApplication.update -= OnUpdate;
        // 窗口关闭时检查未保存的修改
        if (isDirty && actionInfo != null)
        {
            if (EditorUtility.DisplayDialog(
                "未保存的修改",
                $"是否保存对 {actionInfo.name} 的修改？",
                "保存",
                "不保存"))
            {
                SaveChanges();
            }
        }

        // 清理资源
        if (_embeddedEditor != null)
        {
            DestroyImmediate(_embeddedEditor);
            _embeddedEditor = null;
        }
    }

    void OnUpdate()
    {
        if (isPlaying)
        {
            double now = EditorApplication.timeSinceStartup;
            double delta = now - lastTime;
            //Debug.Log($"Tick lastTime: {now} {lastTime} {delta} {frameRate}");
            while (delta >= frameRate)
            {
                delta -= frameRate;
                runingFrame += 1;
                currentFrame = runingFrame;
                Tick();

                //Debug.Log($"Tick: {now} {delta} {frameRate} {runingFrame}");
                lastTime = now;
            }
        }
        else
        {
            if (lastFrame != currentFrame)
                ApplyAnimationFrame(currentFrame);
        }
    }

    void Tick()
    {
        if (runingFrame >= maxFrame)
        {
            isPlaying  = false;
            runingFrame = 0;
        }
        
        if (character == null || animator == null)
        {
            return;
        }

        ApplyAnimationFrame(runingFrame);

        Repaint();
    }

    void PlayAction(bool play)
    {
        if (character == null || animation == null || animator == null || actionInfo == null)
        {
            if (character == null)
            {
                EditorUtility.DisplayDialog("提示", "没有选择预览角色", "确定");
            }
            if (animation == null)
            {
                EditorUtility.DisplayDialog("提示", "没有选择动画片段", "确定");
            }
            if (animator == null)
            {
                EditorUtility.DisplayDialog("提示", "没有animator组件", "确定");
            }
            if (actionInfo == null)
            {
                EditorUtility.DisplayDialog("提示", "没有动作数据", "确定");
            }
            return;
        }
        if (play)
        {
            isPlaying = true;
            lastTime  = EditorApplication.timeSinceStartup;
            currentFrame = 0;
            runingFrame  = 0;
        }
        else
        {
            isPlaying = false;
        }

        Repaint();
    }

    void ApplyAnimationFrame(int frame)
    {
        lastFrame = frame;
        // 计算当前帧对应的时间
        float frameTime = frame * 1f / maxFrame;
        
        // 确保时间在动画长度范围内
        float time = Mathf.Clamp(frameTime, 0, animation.length);

        if (!controllerOverridden)
            SetupOverrideController();

        // 使用AnimationMode采样动画
        AnimationMode.BeginSampling();
        AnimationMode.SampleAnimationClip(character, animation, time);
        AnimationMode.EndSampling();

        // 强制场景重绘
        SceneView.RepaintAll();
    }

     void SetupOverrideController()
    {
        // 如果还没有创建覆盖控制器
        if (overrideController == null)
        {
            // 保存原始控制器
            RuntimeAnimatorController originalController = animator.runtimeAnimatorController;
            
            // 创建覆盖控制器
            overrideController = new AnimatorOverrideController(originalController);
            animator.runtimeAnimatorController = overrideController;
        }
        
        // 覆盖默认动画
        overrideController["DefaultAnimation"] = animation;

        controllerOverridden = true;
    }

    void RestoreOriginalAnimation()
    {
        if (animator != null && overrideController != null)
        {
            // 恢复原始控制器
            animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            overrideController                 = null;
            controllerOverridden               = false;
        }
    }

    void OnCharacterChange()
    {
        RestoreOriginalAnimation();
    }

    void OnAnimationClipChange()
    {
        RestoreOriginalAnimation();
    }

}
