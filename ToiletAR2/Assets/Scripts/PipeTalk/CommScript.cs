using UnityEngine;
using System.Collections;
using System;
using Microsoft.Win32.SafeHandles;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
//using System.Diagnostics;

public class CommScript : MonoBehaviour 
{
	public static GameCommPipeServer PipeReadServer;
	public static GameCommPipeServer PipeWriteServer;
    public static GameCommPipeServer PipeReadServer2;	

	
	// Use this for initialization
	void Start () 
	{
        Debug.Log("Comm script start");
		PipeReadServer = new GameCommPipeServer(@"\\.\pipe\myNamedPipe1",0); //Read pipe
		PipeWriteServer = new GameCommPipeServer(@"\\.\pipe\myNamedPipe2",1); //Write pipe
        //PipeReadServer2 = new GameCommPipeServer(@"\\.\pipe\myNamedPipe3",0); //Read pipe
		PipeReadServer.Start();
        PipeWriteServer.Start();
        //PipeReadServer2.Start();
        //call chilitags app here
        //System.Diagnostics.Process.Start(@"..\ToiletAR2CVService\Release\ToiletAR2CVService.exe");
	}
	
	// Update is called once per frame
	void Update () 
	{
		//int ang = PServer1.tag1Angle;
		//int xpos = PServer1.tag1XPos;
		
	}
	
	void OnApplicationQuit()
	{
		PipeReadServer.StopServer();
		PipeWriteServer.StopServer();
        //PipeReadServer2.StopServer();
	}


}
