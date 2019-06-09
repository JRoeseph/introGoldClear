using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace ICantGetThis
{
    class Program
    {
        const int PROCESS_ALL_ACCESS = 0x1F0FFF;

        public static int levelData1stOffset = (int)0xE8F4B8;
        public static int levelData2ndOffset = (int)0x810;
		public static int levelData3rdOffset = (int)0x80C168;

        public static Process nppProcess;
        public static IntPtr nppProcessHandle;
        public static ProcessModule nppProcessModule;
        public static ProcessModuleCollection nppProcessModuleCollection;
        public static IntPtr nppdllBaseAddress;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        static void Main(string[] args)
        {
            int bytesRead = 0;
            byte[] levelDataBuffer = new byte[8];
            nppProcess = Process.GetProcessesByName("N++")[0];
            nppProcessHandle = OpenProcess(PROCESS_ALL_ACCESS, false, nppProcess.Id);

            string nppdllFilePath = "npp.dll";
            nppdllBaseAddress = (IntPtr)0;
            nppProcessModuleCollection = nppProcess.Modules;


            for (int i = 0; i < nppProcessModuleCollection.Count; i++)
            {
                nppProcessModule = nppProcessModuleCollection[i];
                if (nppProcessModule.FileName.Contains(nppdllFilePath))
                {
                    nppdllBaseAddress = nppProcessModule.BaseAddress;
                }
            }
			
            ReadProcessMemory((int)nppProcessHandle, (int)(nppdllBaseAddress + levelData1stOffset), levelDataBuffer, levelDataBuffer.Length, ref bytesRead);
            Int32 levelData1stAddress = BitConverter.ToInt32(levelDataBuffer, 0);
			
            ReadProcessMemory((int)nppProcessHandle, (int)(levelData1stAddress + levelData2ndOffset), levelDataBuffer, levelDataBuffer.Length, ref bytesRead);
            Int32 levelData2ndAddress = BitConverter.ToInt32(levelDataBuffer, 0);
			
            int bytesWritten = 0;
			for (int i = 0; i < 125; i++) 
			{
				bytesWritten = 0;
				WriteProcessMemory((int)nppProcessHandle, (int)(levelData2ndAddress + levelData3rdOffset + (i * (int)0x30)), new byte[]{ 0, 0, 0, 0, 0, 0, 0, 0}, 8, ref bytesWritten);
			}
        }
    }
}
