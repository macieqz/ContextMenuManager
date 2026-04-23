using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using ContextMenuManager.Methods;

namespace BluePointLilac.Methods
{
    public static class ResourceString
    {
        private static readonly Regex HanRegex = new Regex("[\\u4e00-\\u9fff]+", RegexOptions.Compiled);
        private static readonly Dictionary<string, string> PhraseMap = new Dictionary<string, string>
        {
            { "右键菜单", "Context Menu" },
            { "目录背景", "Directory Background" },
            { "桌面背景", "Desktop Background" },
            { "任务栏", "Taskbar" },
            { "开始屏幕", "Start Menu" },
            { "工具栏", "Toolbar" },
            { "固定到任务栏", "Pin to Taskbar" },
            { "显示图标", "Show Icons" },
            { "层叠右键菜单", "Cascading Context Menu" },
            { "显示桌面右键菜单", "Show Desktop Context Menu" },
            { "显示桌面上下文菜单", "Show Desktop Context Menu" },
            { "显示/隐藏", "Show/Hide" },
            { "深色应用", "Dark Apps" },
            { "浅色应用", "Light Apps" },
            { "颜色模式", "Color Mode" },
            { "系统信息", "System Information" },
            { "注册", "Register" },
            { "注销", "Unregister" },
            { "共享文件夹同步", "Shared Folder Sync" },
            { "文档加密/解密", "Document Encrypt/Decrypt" },
            { "文件", "File" },
            { "目录", "Directory" },
            { "桌面", "Desktop" },
            { "磁盘", "Disk" },
            { "回收站", "Recycle Bin" },
            { "此电脑", "This PC" }
        };

        //MSDN文档: https://docs.microsoft.com/windows/win32/api/shlwapi/nf-shlwapi-shloadindirectstring
        //提取.pri文件资源: https://docs.microsoft.com/windows/uwp/app-resources/makepri-exe-command-options
        //.pri转储.xml资源列表: MakePri.exe dump /if [priPath] /of [xmlPath]

        [DllImport("shlwapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode,
            ExactSpelling = true, SetLastError = false, ThrowOnUnmappableChar = true)]
        private static extern int SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, uint cchOutBuf, IntPtr ppvReserved);

        /// <summary>获取格式为"@[filename],-[strID]"或"@{[packageName]?ms-resource://[resPath]}"的直接字符串</summary>
        /// <param name="resStr">要转换的字符串</param>
        /// <returns>resStr为Null时返回值为string.Empty; resStr首字符为@但解析失败时返回string.Empty</returns>
        /// <remarks>[fileName]:文件路径; [strID]:字符串资源索引; [packageName]:UWP带版本号包名; [resPath]:pri资源路径</remarks>
        public static string GetDirectString(string resStr)
        {
            if(string.IsNullOrWhiteSpace(resStr)) return string.Empty;

            string outStr;
            if(!resStr.StartsWith("@"))
            {
                outStr = resStr;
            }
            else
            {
            StringBuilder outBuff = new StringBuilder(1024);
            SHLoadIndirectString(resStr, outBuff, 1024, IntPtr.Zero);
                outStr = outBuff.ToString();
            }

            string lang = AppConfig.Language;
            bool englishUi = string.IsNullOrWhiteSpace(lang) || lang.StartsWith("en", StringComparison.OrdinalIgnoreCase);
            if(englishUi) outStr = TranslateChineseFallback(outStr);
            return outStr;
        }

        private static string TranslateChineseFallback(string text)
        {
            if(string.IsNullOrWhiteSpace(text) || !HanRegex.IsMatch(text)) return text;
            foreach(var kv in PhraseMap)
            {
                text = text.Replace(kv.Key, kv.Value);
            }
            text = HanRegex.Replace(text, string.Empty).Trim();
            return string.IsNullOrWhiteSpace(text) ? "Item" : text;
        }

        public static readonly string OK = GetDirectString("@shell32.dll,-9752");
        public static readonly string Cancel = GetDirectString("@shell32.dll,-9751");
    }
}