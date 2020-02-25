using System;
using System.Runtime.InteropServices;

namespace StartScene
{

    /**
     * 引用：https://blog.csdn.net/SimulationPD/article/details/83027801
     */
//脚本FileManager
    public static class FileManager
    {
        private static OpenFileName CreateFileNameObj(string str, string filter)
        {
            OpenFileName openFileName = new OpenFileName();
            openFileName.structSize = Marshal.SizeOf(openFileName);
            //文件类型 config配置文件,"Excel文件(*.xlsx)\0*.xlsx" ,"Txt文件(*.txt)\0*.txt"...
            openFileName.filter = filter;
            openFileName.file = new string(new char[256]);//new一个256字符的string
            openFileName.maxFile = openFileName.file.Length;//获取256字符的string的长度作为最大
            openFileName.fileTitle = new string(new char[64]);//64字符的string
            openFileName.maxFileTitle = openFileName.fileTitle.Length;//文件标题的最大长度
            openFileName.initialDir = UnityEngine.Application.dataPath;//默认路径
            openFileName.title = str;//文件标题
            openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
            return openFileName;
        }
 
        public static OpenFileName OpenFile(string str, string filter)
        {
            OpenFileName openFileName = CreateFileNameObj(str, filter);
            if (LocalDialog.GetOpenFileName(openFileName))
            {
                return openFileName;
            }
            else
            {
                throw new Exception("打开文件失败！请检查文件是否存在。");
            }
        }
 
        public static OpenFileName SaveFile(string str, string filter)
        {
            OpenFileName openFileName = CreateFileNameObj(str, filter);
            if (LocalDialog.GetSaveFileName(openFileName))
            {
                return openFileName;
            }
            else
            {
                throw new Exception("保存文件失败！请检查路径是否合法存在。");
            }
        }
    }
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OpenFileName
    {
        public int structSize = 0;
        public IntPtr dlgOwner = IntPtr.Zero;
        public IntPtr instance = IntPtr.Zero;
        public String filter = null;
        public String customFilter = null;
        public int maxCustFilter = 0;
        public int filterIndex = 0;
        public String file = null;
        public int maxFile = 0;
        public String fileTitle = null;
        public int maxFileTitle = 0;
        public String initialDir = null;
        public String title = null;
        public int flags = 0;
        public short fileOffset = 0;
        public short fileExtension = 0;
        public String defExt = null;
        public IntPtr custData = IntPtr.Zero;
        public IntPtr hook = IntPtr.Zero;
        public String templateName = null;
        public IntPtr reservedPtr = IntPtr.Zero;
        public int reservedInt = 0;
        public int flagsEx = 0;
    }
 
    public class LocalDialog
    {
 
        //链接指定系统函数       打开文件对话框
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

        //链接指定系统函数        另存为对话框
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);
    }
}