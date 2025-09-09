using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ACTGame;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(SaveActionData))]
public class ActionEditorSaveInspector : Editor
{
    public VisualTreeAsset InspectorUXML;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement inspector = new VisualElement();

        //inspector.Add(new Label("Action Editor"));

        //VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ActionEditorInspector.uxml");
        InspectorUXML.CloneTree(inspector);

        var btn = inspector.Q("Save") as Button;
        if (btn != null)
        {
            btn.RegisterCallback<ClickEvent>(ClickSaveBtn);
        }

        return inspector;
    }

    private void ClickSaveBtn(ClickEvent e)
    {
        SaveActionData action = (SaveActionData)target;
        var data = action.Action;
        if (data.groups.Count > 0)
        {
            ActionDataTrack dataTrack = null;
            List<CancelTagTrack> cancelTagTracks = new();
            CancelDataTrack cancelDataTrack = null;
            MoveInputAcceptanceTrack moveInputAcceptanceTrack = null;
            var group = data.groups[0];
            foreach (var track in group.tracks)
            {
                if (track is ActionDataTrack && dataTrack == null)
                {
                    dataTrack = track as ActionDataTrack;
                }
                else if (track is CancelTagTrack)
                {
                    cancelTagTracks.Add(track as CancelTagTrack);
                }
                else if (track is CancelDataTrack && cancelDataTrack == null)
                {
                    cancelDataTrack = track as CancelDataTrack;
                }
                else if (track is MoveInputAcceptanceTrack && moveInputAcceptanceTrack == null)
                {
                    moveInputAcceptanceTrack = track as MoveInputAcceptanceTrack;
                }
            }

            if (!dataTrack || (dataTrack.clips.Count <= 0))
            {
                Debug.LogWarning("另存失败，没有动作数据轨道");
                return;
            }

            //List<Cancel>
            var actionCfg = ScriptableObject.CreateInstance<ACTGame.Action.ActionConfig>();
            var actionClip = dataTrack.clips[0] as ActionDataClip;
            if (actionClip.TotalFrame <= 0)
            {
                Debug.LogWarning("另存失败，动作数据帧为0");
                return;
            }
            actionCfg.ActionName         = actionClip.ActionName;
            actionCfg.Animation          = actionClip.Animation;
            actionCfg.Priority           = actionClip.Priority;
            actionCfg.AutoNextActionName = actionClip.AutoNextActionName;
            actionCfg.AutoTerminate      = actionClip.AutoTerminate;
            actionCfg.KeepPlayingAnim    = actionClip.KeepPlayingAnim;
            actionCfg.ApplyGravity       = actionClip.ApplyGravity;
            actionCfg.RootMotionTween    = actionClip.RootMotionTween;
            actionCfg.EnterActionEvent   = actionClip.EnterActionEvent;
            actionCfg.FrameCount         = actionClip.TotalFrame;
            
            //CancelTags
            actionCfg.BeCanceledTags = new();
            for (int i = 0; i < cancelTagTracks.Count; i++)
            {
                var track = cancelTagTracks[i];
                for (int j = 0; j < track.clips.Count; j++)
                {
                    var clip               = track.clips[j] as CancelTagClip;
                    int startFrame         = clip.StartFrame;
                    int frameNum           = clip.TotalFrame;
                    ConfigCancelTag cfgTag = new ConfigCancelTag();
                    cfgTag.validRange      = new FrameIndexRange(startFrame, frameNum);
                    cfgTag.cancelTags      = clip.beCancelTag;
                    actionCfg.BeCanceledTags.Add(cfgTag);
                }
            }

            //CancelDatas
            actionCfg.CancelDatas = new();
            if (cancelDataTrack != null)
            {
                var clips = cancelDataTrack.clips;
                for (int i = 0; i < clips.Count; i++)
                {
                    var clip = clips[i] as CancelDataClip;
                    actionCfg.CancelDatas.AddRange(clip.cancelData);
                }
            }

            //moveInputAcceptance
            if (moveInputAcceptanceTrack != null)
            {
                foreach (var clip in moveInputAcceptanceTrack.clips)
                {
                    var tmp = clip as MoveInputAcceptanceClip;
                    actionCfg.MoveInputAcceptances.Add(tmp.MoveInputAcceptance);
                }
            }

            SaveAs(actionCfg);
        }
    }

    void SaveAs(ACTGame.Action.ActionConfig actionConfig)
    {
        if (actionConfig == null) return;

        // 获取默认保存路径
        string defaultPath = "Assets/Resources/Prefabs/ScriptableObjects/ActionConfig/";

        // 打开保存对话框
        string path = EditorUtility.SaveFilePanelInProject(
        "另存为",
        actionConfig.ActionName,
        "asset",
        "选择保存位置",
        defaultPath);

        if (!string.IsNullOrEmpty(path))
        {
            // 保存新资源
            AssetDatabase.CreateAsset(actionConfig, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("另存为成功: " + path);
        }
    }
}
