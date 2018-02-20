using System;
using Microsoft.Win32.SafeHandles;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using UnityEngine;

//Line 183 - Data from device to unity
//Line 96 - Data from unity to device

public class GameCommPipeServer : MonoBehaviour
{
    GameObject gameController;

    public int tag1Angle { get; private set;}
	public int tag1XPos { get; private set;}
	bool pipingActive = true;
	//static int count = 0;
	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern SafeFileHandle CreateNamedPipe(
		String pipeName,
		uint dwOpenMode,
		uint dwPipeMode,
		uint nMaxInstances,
		uint nOutBufferSize,
		uint nInBufferSize,
		uint nDefaultTimeOut,
		IntPtr lpSecurityAttributes);
	
	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern int ConnectNamedPipe(SafeFileHandle hNamedPipe, IntPtr lpOverlapped);
	
	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern int DisconnectNamedPipe(SafeFileHandle hNamedPipe);
	
	public const uint DUPLEX = (0x00000003);
	public const uint FILE_FLAG_OVERLAPPED = (0x40000000);
	
	public class Client
	{
		public SafeFileHandle handle;
		public FileStream stream;
	}
	
	public const int BUFFER_SIZE = 28;
	public Client clientse =null;
	
	public string pipeName;
	Thread listenThread;
	SafeFileHandle clientHandle;
	public int ClientType;
	
	public GameCommPipeServer(string PName,int Mode)
	{
		pipeName = PName;
		ClientType = Mode;//0 Reading Pipe, 1 Writing Pipe
		
	}
	
	public void Start()
	{
        gameController = GameObject.Find("GameController");
        this.listenThread = new Thread(new ThreadStart(ListenForClients));
		this.listenThread.Start();		
	}
	
	private void ListenForClients()
	{
		Debug.Log("Inside Listening thread");
		while (true)
		{
			
			clientHandle = CreateNamedPipe(this.pipeName,DUPLEX | FILE_FLAG_OVERLAPPED,0,255,BUFFER_SIZE,BUFFER_SIZE,0,IntPtr.Zero);

            //could not create named pipe
            if (clientHandle.IsInvalid)
            {
                Debug.Log("could not create named pipe");
                return;
            }
			
			int success = ConnectNamedPipe(clientHandle, IntPtr.Zero);
			
			//could not connect client
			if (success == 0)
				return;
			
			clientse = new Client();
			clientse.handle = clientHandle;
			clientse.stream = new System.IO.FileStream(clientse.handle, FileAccess.ReadWrite, BUFFER_SIZE, true);
			
			if (ClientType == 0)
			{
                Debug.Log("ClientType == 0");
				Thread readThread = new Thread(new ThreadStart(Read));
				readThread.Start();
			}
			else if(ClientType == 1)	
			{
                Debug.Log("ClientType == 1");
                Thread sendMsgThread = new Thread(new ThreadStart(SendMessageThread));
				sendMsgThread.Start();
			}
            if (ClientType == 2)
            {
                Debug.Log("ClientType == 2");
                Thread readThread = new Thread(new ThreadStart(Read));
                readThread.Start();
            }			
		}
	}
	
	private void SendMessageThread()
	{
		Debug.Log("Inside Sending thread");
		while (pipingActive)
		{
      

		}
        Debug.Log("Outside loop");
		SendMessage ("quit", this.clientse);
	}
	
	
	private void Read()
	{
		//Client client = (Client)clientObj;
		//clientse.stream = new FileStream(clientse.handle, FileAccess.ReadWrite, BUFFER_SIZE, true);
		byte[] buffer = null;
		ASCIIEncoding encoder = new ASCIIEncoding();
		
		Debug.Log("Inside Listening thread");
		
		while (pipingActive)
		{			
			int bytesRead = 0;
			
			try
			{
				buffer = new byte[BUFFER_SIZE];
				bytesRead = clientse.stream.Read(buffer, 0, BUFFER_SIZE);
			}
			catch
			{
				//read error has occurred
				break;
			}
			
			//client has disconnected
			if (bytesRead == 0)
				break;
			
			//fire message received event
			//if (this.MessageReceived != null)
			//    this.MessageReceived(clientse, encoder.GetString(buffer, 0, bytesRead));
			
			int ReadLength = 0;
			for (int i = 0; i < BUFFER_SIZE; i++)
			{
				if (buffer[i].ToString("x2") != "cc")
				{
					ReadLength++;
				}
				else
					break;
			}
			//Debug.Log("ReadLength" + ReadLength);
            if (ReadLength > 0 && ReadLength == BUFFER_SIZE)
            {
                byte[] Rc = new byte[ReadLength];
                Buffer.BlockCopy(buffer, 0, Rc, 0, ReadLength);
                //++count;
                //Debug.Log("C# App: Received " + ReadLength +" Bytes: "+ encoder.GetString(Rc, 0, ReadLength));
                //Debug.Log(System.Text.Encoding.Default.GetString(buffer));
                char[] delimiters = new char[] { '<', ':', '>' };  //Format : <0000:0000:0000:0000:0000> => <tagnum:ang:centerloc_x:centerloc_y:length>. Size : 26 bytes
                String incomingMsg = System.Text.Encoding.Default.GetString(buffer);
                //Debug.Log(incomingMsg);
                String[] splitMsg = incomingMsg.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                //Debug.Log("splitMsg size : " + splitMsg.Length);

                if (splitMsg.Length == 6) 
                {
                    //int msgType = int.Parse(splitMsg[0]);

                    //if (msgType == 0) //Tag co-ords coming in    				
                    //{
                    //Debug.Log("split msgs : " + splitMsg[1] + "," + splitMsg[3] + "," + splitMsg[4]);
                    //tag1Angle = int.Parse(splitMsg[0]);
                    //tag1XPos = int.Parse(splitMsg[1]);
                    int tagid = int.Parse(splitMsg[1]);
                    int angle = int.Parse(splitMsg[2]);
                    int centerX = int.Parse(splitMsg[3]);
                    int centerY = int.Parse(splitMsg[4]);
                    int size = int.Parse(splitMsg[5]);
                    GameControllerScript.setTagInfo(new tagInfo(tagid, angle, centerX, centerY, size));
                    //gameController.SendMessage("getTagInfo", new tagInfo(tagid, angle, centerX, centerY, size));

                    /*if (PythonTest.chiliCodeToPyCodeMapping.ContainsKey(tagid))
                    {
                        Debug.Log(splitMsg[0] + ":" + PythonTest.chiliCodeToPyCodeMapping[tagid]);
                    }*/
                    //}

                    buffer.Initialize();
                }
            }
		}
		
		//clean up resources
		clientse.stream.Close();
		clientse.handle.Close();
		
	}
	public void SendMessage(string message, Client client)
	{
		
		ASCIIEncoding encoder = new ASCIIEncoding();
		byte[] messageBuffer = encoder.GetBytes(message);
        //Debug.Log("Outbound message : " + message);
        //Debug.Log("Outbound msg length : " + messageBuffer.Length);
		
		if (client.stream.CanWrite)
		{
			client.stream.Write(messageBuffer, 0, messageBuffer.Length);
			client.stream.Flush();
		}
		
		
	}
	public void StopServer()
	{
		//clean up resources
		pipingActive = false; //The thread's while loop will now stop
		DisconnectNamedPipe(this.clientHandle);
		this.listenThread.Abort();
	}
}
