

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Resources;
using System.IO;

public static class SoundHelper
{
    [DllImport("winmm.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
     static extern int PlaySound(string name, int hmod, int flags);
    [DllImport("winmm.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
     static extern int PlaySound(byte[] name, int hmod, int flags);

     const int SND_SYNC = 0x0;
     const int SND_ASYNC = 0x1;
     const int SND_MEMORY = 0x4;
     const int SND_ALIAS = 0x10000;
     const int SND_NODEFAULT = 0x2;
     const int SND_FILENAME = 0x20000;
     const int SND_RESOURCE = 0x40004;

    public static void PlayWaveFile(string fileWaveFullPath)
    {
        try
        {
            PlaySound(fileWaveFullPath, 0, SND_FILENAME);
        }
        catch
        {
        }
    }

    public static void PlayWaveResource(string WaveResourceName)
    {

        string strNameSpace = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.ToString();
        if (strNameSpace.EndsWith("Test"))
            strNameSpace = strNameSpace.Substring(0, strNameSpace.Length - 4);
        Stream resourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(strNameSpace + ".Resources." + WaveResourceName);
        if (resourceStream == null)
            return;
        byte[] wavData = null;
        wavData = new byte[Convert.ToInt32(resourceStream.Length) + 1];
        resourceStream.Read(wavData, 0, Convert.ToInt32(resourceStream.Length));
        resourceStream.Close();
        PlaySound(wavData, 0, SND_ASYNC | SND_MEMORY);
    }

    public static void PlaySound (Sounds sound)
    {
        string resourceName = sound.ToString() + ".wav";
        PlayWaveResource(resourceName);

    }

    public enum Sounds { MajorError,MinorError,WorkflowCompleted, WorkflowInitiated, WorkflowProgress}


}

