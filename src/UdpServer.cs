using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

using DuGu720DegreeView.Communications;
using DuGu720DegreeView.Models;
using DuGu720DegreeView.ShareMemory;
using Utils;

public class UdpServer : MonoBehaviour
{

    AngleDef angle;
    AngleDef lastAngle;

    AngleDef otherAngle;

    AngleWithLocation angleWithLocation;
    AngleWithLocation otherAngleWithLocation;

    private float nowTime;
    private float oldTime;
    private float startTime;
    private float lerp = 0.05f;
    private int timeIndex = 1;
    public bool IsSend = false;
    public bool IsSendFromTeachingPlatform = true;
    private float s = 1;
    private float t;
    private float speed;
    private float time;
    int recievecount;

    private Thread SendThreading;
    private Thread RecieveThread;
    private Thread RecieveFromTeachThread;
    IPEndPoint ip;

    private string memName = "sm_7dplayer_0000";
    private ZyShareMem MemDB = new ZyShareMem();

    object locked = new object();

    float myAngleTime;

    TeachingCommand command;

    //string TeachIp = "192.168.0.132";
    IPEndPoint TeachIp;
    UdpClient clientTeach;


    AngleDef DefaultAngle
    {
        get
        {
            return new AngleDef()
            {
                MsgType = 1001,
                Length = 24,
                Yaw = 0.0f,
                Time = 0
            };
        }
    }

    void Start()
    {
        TeachIp = new IPEndPoint(IPAddress.Parse(File.ReadAllText("ip.txt")), 16000);
        this.clientTeach = new UdpClient(12000);
        ip = new IPEndPoint(IPAddress.Any, 0);
        angle = DefaultAngle;
        lastAngle = DefaultAngle;
        otherAngle = DefaultAngle;
        command = default(TeachingCommand);
        angleWithLocation = default(AngleWithLocation);
        otherAngleWithLocation = default(AngleWithLocation);

        SendThreading = new Thread(new ThreadStart(SendRotation));
        SendThreading.Start();
        SendThreading.Name = "SendEvent" + base.transform.name;

        RecieveThread = new Thread(new ThreadStart(RecieveData));
        RecieveThread.Start();
        RecieveThread.Name = "RecieveEvent";

        RecieveFromTeachThread = new Thread(new ThreadStart(RecieveDataFromTeach));
        RecieveFromTeachThread.Start();
        RecieveFromTeachThread.Name = "RecieveFromTeachEvent";
        //UdpPlatform.Instance.SendString("Udp Start");

        Task.Run(() =>
        {
            while (true)
            {
                //var ipRe = new IPEndPoint(IPAddress.Parse("192.168.0.135"), 16000);
                //var bytes = this.clientTeach.Receive(ref ipRe);
                var bytes = this.clientTeach.Receive(ref TeachIp);
                var length = bytes.Length;
                if (length == StructHelper.GetStructSize<TeachingCommand>())
                {
                    command = StructHelper.BytesToStruct<TeachingCommand>(bytes);
                    this.IsSendFromTeachingPlatform = command.IsStart == 1;
                }
            }
        });

        Task.Run(() =>
        {
            while (true)
            {
                UdpPlatform.Instance.SendData(this.angleWithLocation, new IPEndPoint(IPAddress.Broadcast, 16000));
                Thread.Sleep(20);
            }
        });

        if (this.MemDB.Init(this.memName) == 0 || this.MemDB.Init("GAME_SHARED_MEM_0000") == 0)
        {
            Debug.Log("Run");
        }
        else
        {
            Debug.Log("no Run");
            Time.timeScale = 0f;
        }

    }

    void Awake()
    {
        RecieveFromTeachThread = new Thread(new ThreadStart(RecieveDataFromTeach));
        RecieveFromTeachThread.Start();
        RecieveFromTeachThread.Name = "RecieveFromTeachEvent";
    }

    private void OnEnable()
    {
        
    }

    private void RecieveData()
    {
        while (true)
        {
            ip = new IPEndPoint(IPAddress.Any, 0);
            var bytes = UdpPlatform.Instance.RecieveData(ref ip);
            var length = bytes.Length;
            if (length == StructHelper.GetStructSize<AngleWithLocation>())
            {
                var angleAndLocation = StructHelper.BytesToStruct<AngleWithLocation>(bytes);
                var pitchRad = StructHelper.Deg2Rad((float)angleAndLocation.Pitch);
                var rollRad = StructHelper.Deg2Rad((float)angleAndLocation.Roll);
                var yawRad = StructHelper.Deg2Rad((float)angleAndLocation.Yaw);
                var ipstr = ip.ToString();
                if (ipstr.StartsWith("192.168.0.131") == false && ipstr.StartsWith("192.168.0.136") == false)
                {
                    this.angleWithLocation = angleAndLocation;
                    this.angle.Pitch = -pitchRad;
                    this.angle.Roll = -rollRad;
                    this.angle.Yaw = yawRad;
                    if (this.IsSend == true)
                        UdpPlatform.Instance.SendData(this.angleWithLocation, TeachIp);
                }
                else
                {
                    this.otherAngleWithLocation = angleAndLocation;
                    this.otherAngle.Pitch = pitchRad;
                    this.otherAngle.Roll = rollRad;
                    this.otherAngle.Yaw = yawRad;
                }
            }
            if (length == StructHelper.GetStructSize<TeachingCommand>())
            {
                command = StructHelper.BytesToStruct<TeachingCommand>(bytes);
                this.IsSendFromTeachingPlatform = command.IsStart == 1;
            }
         
        }      
    }

    private void RecieveDataFromTeach()
    {
        while (true)
        {
            var ipRe = new IPEndPoint(IPAddress.Parse("192.168.0.135"), 16000);
            var bytes = this.clientTeach.Receive(ref ipRe);
            //var bytes = this.clientTeach.Receive(ref TeachIp);
            var length = bytes.Length;
            if (length == StructHelper.GetStructSize<TeachingCommand>())
            {
                command = StructHelper.BytesToStruct<TeachingCommand>(bytes);
                this.IsSendFromTeachingPlatform = command.IsStart == 1;
            }
        }
    }

    private void SendRotation()
    {
        while (true)
        {
            myAngleTime += 0.0015f;
            if (this.IsSend && this.IsSendFromTeachingPlatform == true)
            {
                this.angle.Time = myAngleTime;

                this.angle.Speed = CalculateSpeed(angle, lastAngle);

                UdpPlatform.Instance.SendData(this.angle);
            }
            lastAngle = angle;
            Thread.Sleep(1);
        }    
    }

    private void Update()
    {       
        this.time = Time.realtimeSinceStartup;
        Debug.Log("roll: " + this.angle.Roll + " pitch:" + this.angle.Pitch + " speed:" + this.angle.Speed + " time:" + this.angle.Time);
        Debug.Log("roll: " + this.otherAngle.Roll + " pitch:" + this.otherAngle.Pitch + " speed:" + this.otherAngle.Speed + " time:" + this.angle.Time);
        Debug.Log("location:" + angleWithLocation.X + " " + angleWithLocation.Y + " " + angleWithLocation.Z + " " + angleWithLocation.Roll);
        Debug.Log("otherLocation: " + otherAngleWithLocation.X + " " + otherAngleWithLocation.Y);
        Debug.Log("IsSend:" + this.IsSend + ";IsSendFromTeachingPlatform:" + this.IsSendFromTeachingPlatform);
        Debug.Log("UnityTime:" + this.time);
    }
    /*
    private void OnDisable()
    {
        this.IsSend = false;
        if (this.SendThreading != null)
        {
            this.SendThreading.Abort();
            this.SendThreading = null;
        }
        Debug.Log("App Disable");
    }
    */
    private void OnApplicationQuit()
    {
        this.IsSend = false;
        if (this.SendThreading != null)
        {
            this.SendThreading.Abort();
            this.SendThreading = null;
        }
        if (this.RecieveThread != null)
        {
            this.RecieveThread.Abort();
            this.RecieveThread = null;
        }
        if (this.RecieveFromTeachThread != null)
        {
            this.RecieveFromTeachThread.Abort();
            this.RecieveFromTeachThread = null;
        }
        Debug.Log("App Quit");
    }

    private float CalculateSpeed(AngleDef angle1, AngleDef angle2)
    {

        try
        {

            var deltaTime = Mathf.Abs(angle1.Time - angle2.Time);

            return (Mathf.Abs((angle1.Roll - angle2.Roll) / deltaTime) +

                Mathf.Abs((angle1.Pitch - angle2.Pitch) / deltaTime)) * 0.02f;

        }

        catch
        {

            return 0;

        }

    }

}

