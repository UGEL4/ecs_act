using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ACT_ActionEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/ACT_ActionEditor")]
    public static void ShowExample()
    {
        ACT_ActionEditor wnd = GetWindow<ACT_ActionEditor>();
        wnd.titleContent = new GUIContent("ACT_ActionEditor");
    }

    int m_ClickCount = 0;
    const string m_ButtonPrefix = "button";

    private ActionEditor info;

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        //VisualElement label = new Label("Hello World! From C#");
        //root.Add(label);

        // Instantiate UXML
        //VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        //root.Add(labelFromUXML);


        ///////////////////////////////////
        Label lb = new Label("These controls were created using C# code.");
        root.Add(lb);
        Button btn = new Button();
        btn.name   = "button2";
        btn.text   = "This is Button2";
        root.Add(btn);
        Toggle tg = new Toggle();
        tg.name   = "toggle2";
        tg.label  = "Number?";
        root.Add(tg);
        var visualTree    = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ACT_ActionEditor.uxml");
        var labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        SetupButtonHandler();


        // 创建工具栏
        CreateToolbar(root);
        // 创建时间轴容器
        var timelineContainer = new VisualElement();
        timelineContainer.style.flexGrow = 1;
        timelineContainer.style.flexDirection = FlexDirection.Column;
        root.Add(timelineContainer);
        // 创建时间标尺
        CreateTimeRuler(timelineContainer);
        
        // 创建轨道区域
        CreateTrackArea(timelineContainer);
        
        // 创建播放控制
        CreatePlaybackControls(root);
        
        // 注册更新回调
        EditorApplication.update += OnEditorUpdate;
    }

        //Functions as the event handlers for your button click and number counts 
    private void SetupButtonHandler()
    {
        var buttons = rootVisualElement.Query<Button>();
        buttons.ForEach(RegisterHandler);
    }

    private void RegisterHandler(Button button)
    {
        button.RegisterCallback<ClickEvent>(PrintClickMessage);
    }

    private void PrintClickMessage(ClickEvent evt)
    {
        ++m_ClickCount;

        //Because of the names we gave the buttons and toggles, we can use the
        //button name to find the toggle name.
        Button button       = evt.currentTarget as Button;
        string buttonNumber = button.name.Substring(m_ButtonPrefix.Length);
        string toggleName   = "toggle" + buttonNumber;
        Toggle toggle       = rootVisualElement.Q<Toggle>(toggleName);

        Debug.Log("Button was clicked!" + (toggle.value ? " Count: " + m_ClickCount : ""));
    }

    private void CreateToolbar(VisualElement parent)
    {
        var toolbar = new Toolbar();
        toolbar.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
        toolbar.style.height = 30;
        
        var addTrackBtn = new Button(() => Debug.Log("Add Track")) { text = "Add Track" };
        var addKeyframeBtn = new Button(() => Debug.Log("Add Keyframe")) { text = "Add Keyframe" };
        
        toolbar.Add(addTrackBtn);
        toolbar.Add(addKeyframeBtn);
        
        parent.Add(toolbar);
    }

    private void CreateTimeRuler(VisualElement parent)
    {
        var rulerContainer = new VisualElement();
        rulerContainer.style.height = 30;
        rulerContainer.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
        rulerContainer.style.borderBottomWidth = 1;
        rulerContainer.style.borderBottomColor = Color.gray;
        
        // 创建时间标尺
        var ruler = new VisualElement();
        ruler.style.flexGrow = 1;
        ruler.RegisterCallback<MouseDownEvent>(OnRulerMouseDown);
        rulerContainer.Add(ruler);
        
        // 创建播放头
        var playhead = new VisualElement();
        playhead.name = "playhead";
        playhead.style.position = Position.Absolute;
        playhead.style.top = 0;
        playhead.style.bottom = 0;
        playhead.style.width = 2;
        playhead.style.backgroundColor = Color.red;
        rulerContainer.Add(playhead);
        
        parent.Add(rulerContainer);
    }

    private void CreateTrackArea(VisualElement parent)
    {
        var trackArea = new ScrollView(ScrollViewMode.Horizontal);
        trackArea.style.flexGrow = 1;
        trackArea.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
        
        // 创建轨道
        for (int i = 0; i < 3; i++)
        {
            var track = new VisualElement();
            track.style.height = 60;
            track.style.borderBottomWidth = 1;
            track.style.borderBottomColor = new Color(0.3f, 0.3f, 0.3f);
            track.style.flexDirection = FlexDirection.Row;
            
            // 轨道标签
            var trackLabel = new Label($"Track {i + 1}");
            trackLabel.style.width = 100;
            trackLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            trackLabel.style.backgroundColor = new Color(0.25f, 0.25f, 0.25f);
            track.Add(trackLabel);
            
            // 轨道内容
            var trackContent = new VisualElement();
            trackContent.style.flexGrow = 1;
            trackContent.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
            trackContent.RegisterCallback<MouseDownEvent>(OnTrackMouseDown);
            
            // 添加示例关键帧
            for (int j = 0; j < 5; j++)
            {
                var keyframe = new VisualElement();
                keyframe.name = "keyframe";
                keyframe.style.position = Position.Absolute;
                keyframe.style.width = 10;
                keyframe.style.height = 10;
                keyframe.style.top = 25;
                keyframe.style.backgroundColor = Color.yellow;
                //keyframe.style.borderRadius = 5;
                keyframe.style.left = 100 + j * 50; // 位置示例
                trackContent.Add(keyframe);
            }
            
            track.Add(trackContent);
            trackArea.Add(track);
        }
        
        parent.Add(trackArea);
    }
    
    private bool isPlaying = false;
    private float duration = 30f;
    private float currentTime = 0f;
    private double lastUpdateTime = 0;
    private void CreatePlaybackControls(VisualElement parent)
    {
        var controls = new VisualElement();
        controls.style.flexDirection = FlexDirection.Row;
        controls.style.height = 40;
        controls.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
        controls.style.justifyContent = Justify.Center;
        controls.style.alignItems = Align.Center;
        
        // 播放/暂停按钮
        var playPauseBtn = new Button(TogglePlayPause);
        playPauseBtn.style.width = 80;
        playPauseBtn.style.height = 30;
        playPauseBtn.text = isPlaying ? "Pause" : "Play";
        controls.Add(playPauseBtn);
        
        // 时间显示
        var timeLabel = new Label("00:00");
        timeLabel.name = "timeLabel";
        timeLabel.style.width = 80;
        timeLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        controls.Add(timeLabel);
        
        // 时间滑块
        var timeSlider = new Slider(0, duration, SliderDirection.Horizontal);
        timeSlider.name = "timeSlider";
        timeSlider.style.flexGrow = 1;
        timeSlider.style.marginLeft = 20;
        timeSlider.style.marginRight = 20;
        timeSlider.RegisterValueChangedCallback(evt => {
            currentTime = evt.newValue;
            UpdatePlayheadPosition();
            UpdateTimeLabel();
        });
        controls.Add(timeSlider);
        
        parent.Add(controls);
    }
    
    private void OnRulerMouseDown(MouseDownEvent evt)
    {
        var ruler = evt.currentTarget as VisualElement;
        var rulerRect = ruler.worldBound;
        currentTime = (evt.localMousePosition.x / rulerRect.width) * duration;
        UpdatePlayheadPosition();
        UpdateTimeSlider();
        UpdateTimeLabel();
    }
    
    private void OnTrackMouseDown(MouseDownEvent evt)
    {
        var trackContent = evt.currentTarget as VisualElement;
        var trackRect = trackContent.worldBound;
        float clickTime = (evt.localMousePosition.x / trackRect.width) * duration;
        
        // 添加关键帧
        var keyframe = new VisualElement();
        keyframe.name = "keyframe";
        keyframe.style.position = Position.Absolute;
        keyframe.style.width = 10;
        keyframe.style.height = 10;
        keyframe.style.top = 25;
        keyframe.style.backgroundColor = Color.yellow;
        //keyframe.style.borderRadius = 5;
        keyframe.style.left = (clickTime / duration) * trackRect.width - 5;
        trackContent.Add(keyframe);
        
        Debug.Log($"Added keyframe at {clickTime:F2} seconds");
    }
    
    private void TogglePlayPause()
    {
        isPlaying = !isPlaying;
        rootVisualElement.Q<Button>().text = isPlaying ? "Pause" : "Play";
        lastUpdateTime = EditorApplication.timeSinceStartup;
    }
    
    private void OnEditorUpdate()
    {
        if (!isPlaying) return;
        
        double currentTimeSinceStartup = EditorApplication.timeSinceStartup;
        double deltaTime = currentTimeSinceStartup - lastUpdateTime;
        lastUpdateTime = currentTimeSinceStartup;
        
        currentTime += (float)deltaTime;
        
        if (currentTime > duration)
        {
            currentTime = 0;
            isPlaying = false;
            rootVisualElement.Q<Button>().text = "Play";
        }
        
        UpdatePlayheadPosition();
        UpdateTimeSlider();
        UpdateTimeLabel();
    }
    
    private void UpdatePlayheadPosition()
    {
        var playhead = rootVisualElement.Q("playhead");
        if (playhead != null)
        {
            var ruler = playhead.parent;
            playhead.style.left = (currentTime / duration) * ruler.worldBound.width;
        }
    }

    private void UpdateTimeSlider()
    {

    }

    private void UpdateTimeLabel()
    {
        
    }
}
