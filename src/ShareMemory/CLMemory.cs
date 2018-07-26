using System;
using System.Runtime.InteropServices;

using UnityEngine;

using DuGu720DegreeView.Models;

namespace DuGu720DegreeView.ShareMemory
{
    public class CLMemory : MonoBehaviour
    {
        private const int FILE_MAP_READ = 4;

        private static CLMemory m_Instance;

        private string memName = "GAME_SHARED_MEM_0000";

        private ZyShareMem MemDB = new ZyShareMem();

        private IntPtr hMappingHandle = IntPtr.Zero;

        private IntPtr hVoid = IntPtr.Zero;

        private bool inited;

        private VectorAngExp m_vectorAngExp;

        private GameSharedDef m_gameSharedDef;

        private Vector3 angele = Vector3.zero;

        public static CLMemory Instance
        {
            get
            {
                return CLMemory.m_Instance;
            }         
        }

        private void Awake()
        {
            CLMemory.m_Instance = this;
            this.m_vectorAngExp = default(VectorAngExp);
            this.m_vectorAngExp.X = 0f;
            this.m_vectorAngExp.Y = 0f;
            this.m_vectorAngExp.Z = 0f;
            this.m_gameSharedDef = default(GameSharedDef);
            this.m_gameSharedDef.Angle = this.m_vectorAngExp;
            this.m_gameSharedDef.GameStatus = 0;
        }

        private void Update()
        {
            if (!this.inited)
            {
                this.hMappingHandle = ZyShareMem.OpenFileMapping(4, false, this.memName);
                if (this.hMappingHandle == IntPtr.Zero)
                {
                    Debug.Log("打开共享内存失败:");
                    return;
                }
                this.hVoid = ZyShareMem.MapViewOfFile(this.hMappingHandle, 4u, 0u, 0u, (uint)Marshal.SizeOf(this.m_gameSharedDef));
                if (this.hVoid == IntPtr.Zero)
                {
                    Debug.Log("文件映射失败——读共享内存");
                    return;
                }
                //Debug.Log("共享内存Run");
                this.inited = true;
            }
            if (this.inited)
            {
                this.m_gameSharedDef = (GameSharedDef)this.ReadFromMemory(this.m_gameSharedDef.GetType());
                this.m_vectorAngExp = this.m_gameSharedDef.Angle;
                //Debug.Log("游戏状态：" + m_gameSharedDef.GameStatus);
                //Debug.Log("读取角度 X：" + m_vectorAngExp.X + " Y:" + m_vectorAngExp.Y + " Z:" + m_vectorAngExp.Z);
            }
        }

        public Vector3 GetVectorAngExp()
        {
            this.angele.x = this.m_vectorAngExp.X * 57.29578f;
            this.angele.y = -this.m_vectorAngExp.Z * 57.29578f;
            this.angele.z = this.m_vectorAngExp.Y * 57.29578f;
            return this.angele;
        }

        public int GetGameStatus()
        {
            return this.m_gameSharedDef.GameStatus;
        }

        public object ReadFromMemory(Type type)
        {
            return Marshal.PtrToStructure(this.hVoid, type);
        }

        private void Close()
        {
            if (this.hVoid != IntPtr.Zero)
            {
                ZyShareMem.UnmapViewOfFile(this.hVoid);
                this.hVoid = IntPtr.Zero;
            }
            if (this.hMappingHandle != IntPtr.Zero)
            {
                ZyShareMem.CloseHandle(this.hMappingHandle);
                this.hMappingHandle = IntPtr.Zero;
            }
        }

        private void OnApplicationQuit()
        {
            this.Close();
        }
    }

}
