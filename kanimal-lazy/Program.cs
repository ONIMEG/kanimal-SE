using IWshRuntimeLibrary;
using kanimal;
using NLog;
using System.IO;

namespace kanimal_lazy {
    
    public class Program {

        public const string SOURCE = "S.lnk";
        public const string DESTINATION = "D.lnk";
        private static int flag = -1;

        private static void Main(string[] args) {
            KLog.Banner();
            try {
                if (args.Length == 0) {
                    Mode1();
                } else {
                    Mode2(args);
                }
            } catch(Exception ex) {
                KLog.Error(ex.Message, true);
            }
            Console.ResetColor();
            Console.WriteLine("\n执行结束，按任意键退出...");
            Console.ReadKey();
        }

        private static void Mode2(string[] args) {
            if (System.IO.File.Exists(args[0]) && args.Length == 3) {
                KLog.Wink("文件已接收", true);
                KanimalToScml(".", args);
            } else if(System.IO.File.Exists(args[0]) && args.Length == 1) {
                if (args[0].EndsWith(".scml")) {
                    ScmlToKaimal("./anim/assets/", args[0]);
                }
            } else {
                KLog.Wink("暂不支持这样的转换", true);
            }
        }
        
        private static void Mode1() {
            var dir = GetDir();
            if (flag == 1) {
                KanimalToScml(dir, null);
            } else if (flag == 0) {
                ScmlToKaimal(dir);
            } else {
                ScmlToKaimal("./anim/assets/");
            }
        }

        private static string GetDir() {
            var lnk = "";
            if (System.IO.File.Exists(SOURCE)) {
                lnk = SOURCE;
                flag = 1;
            } else if (System.IO.File.Exists(DESTINATION)) {
                lnk = DESTINATION;
                flag = 0;
            }
            if (lnk != "") {
                IWshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(lnk);
                // 读取快捷方式的目标值
                string targetPath = shortcut.TargetPath;
                if (Directory.Exists(targetPath)) {
                    var model = (flag == 1) ? "kaimal 转 scml" : "scml 转 kanimal";
                    KLog.Info($"读取到目标目录 {targetPath} 。", true);
                    KLog.Info($"模式：{model}。", true);
                    return targetPath;
                } else {
                    KLog.Error($"快捷方式：{lnk} 的目标 {targetPath} 不是目录。", true);
                    return KLog.ErrorLine(1);
                }
            } else {
                KLog.Warning($"当前目录没有指定快捷方式", true);
                return KLog.ErrorLine(2);
            }
        }


        private static void ScmlToKaimal(string dir, string path) {
            var scml = path;
            Reader? reader;
            dir = Path.GetFullPath(dir);
            var fileName = Path.GetFileNameWithoutExtension(scml);
            if (scml == null) {
                KLog.Error("当前目录并不存在 scml 项目，请确认程序在正确的目录中。", true);
                return;
            } else {
                KLog.Wink($"检测到项目 {fileName}，读取文件...", true);
                var scmlReader = new ScmlReader(scml) {
                    AllowInFramePivots = true,
                    AllowMissingSprites = true,
                    InterpolateMissingFrames = true,
                    Debone = true
                };
                scmlReader.Read();
                KLog.Info("读取文件成功，当前只支持宽松模式，默认去骨骼。", true);
                KLog.Info("开始转换...", true);
                reader = scmlReader;
                var kanimalWriter = new KanimWriter(scmlReader);
                dir = Path.Combine(dir, fileName!);
                KLog.Info($"转换成功，结果会保存在 {dir} 目录下。", true);
                try {
                    AllDelete(dir);
                    kanimalWriter.SaveToDir(dir);
                    KLog.Success();
                }
                catch (Exception e) {
                    KLog.Error($"{e.Message}", true);
                }
            }
        }


        private static void ScmlToKaimal(string dir) {
            var scml = "";
            string[] files = Directory.GetFiles(".");
            List<string> fileList = files.ToList();
            Reader? reader = null;
            dir = Path.GetFullPath(dir);
            scml = fileList.Find(path => path.EndsWith(".scml"));
            var fileName = Path.GetFileNameWithoutExtension(scml);
            if (scml == null) {
                KLog.Error("当前目录并不存在 scml 项目，请确认程序在正确的目录中。",true);
                return;
            } else {
                KLog.Wink($"检测到项目 {fileName}，读取文件...",true);
                var scmlReader = new ScmlReader(scml) {
                    AllowInFramePivots = true,
                    AllowMissingSprites = true,
                    InterpolateMissingFrames = true,
                    Debone = true
                };
                scmlReader.Read();
                KLog.Info("读取文件成功，当前只支持宽松模式，默认去骨骼。", true);
                KLog.Info("开始转换...", true);
                reader = scmlReader;
                var kanimalWriter = new KanimWriter(scmlReader);
                dir = Path.Combine(dir,fileName!);
                KLog.Info($"转换成功，结果会保存在 {dir} 目录下。", true);
                try {
                    AllDelete(dir);
                    kanimalWriter.SaveToDir(dir);
                    KLog.Success();
                } catch (Exception e) {
                    KLog.Error($"{e.Message}", true);
                }
            }
        }

        private static void KanimalToScml(string dir, string[]? args) {
            string[] files = Array.Empty<string>();
            dir = Path.GetFullPath(dir);
            if (args != null && args.Length == 3) {
                files = args;
            } else {
                files = Directory.GetFiles(dir);
            }
            var png = "";
            var anim = "";
            var build = "";
            Reader? reader = null;
            if (files.Length == 0) {
                KLog.Error($"目录：{dir} 下不存在文件，请提供正确的目录。", true);
                return;
            } else {
                List<string> list = files.ToList();
                png = list.Find(path => path.EndsWith(".png"));
                build = list.Find(path => path.Contains("_build."));
                anim = list.Find(path => path.Contains("_anim."));
            }
            var convertList = new[] {png, build, anim};
            var nullCount = convertList.Count(o => o == null);
            if (nullCount > 0) {
                KLog.Error("以下类型文件没有指定：", true);
                for (var i = 0; i < 3; ++i) {
                    if (convertList[i] == null)
                        switch (i) {
                            case 0:
                                KLog.Error("*.png", true);
                                break;
                            case 1:
                                KLog.Error("*_build.*", true);
                                break;
                            case 2:
                                KLog.Error("*_anim.*", true);
                                break;
                        }
                }
                return;
            }
            reader = new KanimReader(
                new FileStream(build!, FileMode.Open),
                new FileStream(anim!, FileMode.Open),
                new FileStream(png!, FileMode.Open));
            reader.Read();
            var fileName = Path.GetFileNameWithoutExtension(png!);
            dir = Path.Combine(dir, fileName);
            KLog.Info($"读取项目 {fileName} 成功。", true);
            KLog.Info("开始转换...", true);
            var scmlWriter = new ScmlWriter(reader) {
                FillMissingSprites = true,
                AllowDuplicateSprites = true
            };
            KLog.Info($"转换成功，结果会保存到 {dir}。", true);
            try {
                scmlWriter.SaveToDir(dir);
                KLog.Success();
            } catch (Exception e) {
                KLog.Error($"{e.Message}", true);
            }
            
        }
        
        private static void AllDelete(string dir) {
            try {
                KLog.Info("清除多余文件...", true);
                foreach (string file in Directory.GetFiles(dir)){
                    System.IO.File.Delete(file);
                }
                KLog.Info("成功", true);
            }
            catch (Exception ex) {
                KLog.Warning($"清空目标目录失败{ex.Message}\n如有多余文件请自行删除。", true);
            }
        }
    }
}