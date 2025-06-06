using System;

public static class InputCommandExtenstion
{
    public static bool ActionOccur(this InputToCommandComponent self, ActionCommand cmd, long curFrame)
    {
        long lastFrame = curFrame - Math.Min(curFrame, cmd.validInFrameCount);
        //double lastStamp = mCurrTimeStamp - Math.Max(actionCmd.validInSecond, Time.deltaTime);
        for (int i = 0; i < cmd.keySequences.Length; i++)
        {
            bool found = false;
            for (int j = 0; j < self.commands.Count; j++)
            {
                if (self.commands[j].frame >= lastFrame && self.commands[j].key == cmd.keySequences[i])
                {
                    found     = true;
                    lastFrame = self.commands[j].frame;
                    break;
                }
            }
            if (found) continue;
            return false;
        }
        return true;
    }
}