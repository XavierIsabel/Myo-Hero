using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class BioPointReader : MonoBehaviour
{
    public string IP = "127.0.0.1";
    private int port = 12346;
    private int sendPort = 12347;
    public string readVal = "";
    public string timestamp = "";
    public float velocity;
    public float[] prob_vector = new float[5];

    // read Thread
    Thread readThread;
    // udpclient object
    UdpClient client;
    UdpClient server;
    IPEndPoint serverTarget;
    
    private static BioPointReader playerInstance;

    [Obsolete]
    void Awake() 
    {
        DontDestroyOnLoad (this);
         
        if (playerInstance == null) {
            playerInstance = this;
        } else {
            DestroyObject(gameObject);
        }
        StartReadingData(); //Somethign weird happening
    }

    public void StartReadingData()
    {
        // create thread for reading UDP messages
        readThread = new Thread(new ThreadStart(ReceiveData))
        {
            IsBackground = true
        };
        readThread.Start();
        server = new UdpClient(sendPort);
        serverTarget = new IPEndPoint(IPAddress.Parse(IP), sendPort);
    }

    // Unity Application Quit Function
    void OnApplicationQuit()
    {
        StopThread();
    }

    // Stop reading UDP messages
    public void StopThread()
    {
        if (readThread.IsAlive)
        {
            readThread.Abort();
        }
        server.Close();
        client.Close();
    }

    // receive thread function
    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (true)
        {
            try
            {
                // receive bytes
                IPEndPoint anyIP = new(IPAddress.Any, 0);
                byte[] buff = client.Receive(ref anyIP);

                // encode UTF8-coded bytes to text format
                string text = Encoding.UTF8.GetString(buff);
                string[] splitText = text.Split();

                string prediction = "";
                float max_probs = 0f;
                for (int i =0; i < 4; i++){
                    if (max_probs < float.Parse(splitText[i])){
                        prediction = i.ToString();
                        max_probs = float.Parse(splitText[i]);
                    }
                    prob_vector[i] = float.Parse(splitText[i]);
                }
                //Debug.Log("highest prob: C" + prediction);
                
                readVal = prediction;

                    
                velocity = float.Parse(splitText[4], CultureInfo.InvariantCulture.NumberFormat);
                timestamp = splitText[5];
                
                
            }
            catch (Exception err)
            {
                Debug.Log(err.ToString());
            }
        }
    }

    public void Write(string strMessage) 
    {
        byte[] arr = System.Text.Encoding.UTF8.GetBytes(strMessage);
        server.Send(arr, arr.Length, serverTarget);
    }
}
