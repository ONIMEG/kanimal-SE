using IWshRuntimeLibrary;
using kanimal;
using NLog;
using System.IO;

namespace kanimal_lazy {
    
    public class Program {

        public const string SOURCE = "S.lnk";
        public const string DESTINATION = "D.lnk";
        public static bool flag = false; // 表示模式，false 是 S，true 是 D

        private static void Main() {
            KLog.Banner();
            var dir = GetDir();
            if (flag) {
                KanimalToScml(dir);
            } else {
                ScmlToKaimal(dir);
            }
            Console.ResetColor();
            Console.WriteLine("\n执行结束，按任意键退出……");
            Console.ReadKey();

        }
        
        private static string GetDir() {
            // 首先，判断执行的模式，
            // 如果是 S source 就是 scml 转 kanimal，
            // 如果是 D destination 就是 kanimal 转 scml。
            var lnk = "";
            if (System.IO.File.Exists(SOURCE)) {
                lnk = SOURCE;
            } else if (System.IO.File.Exists(DESTINATION)) {
                lnk = DESTINATION;
                flag = true;
            }
            if (lnk != "") {
                IWshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(lnk);
                // 读取快捷方式的目标值
                string targetPath = shortcut.TargetPath;
                if (Directory.Exists(targetPath)) {
                    var model = flag ? "kaimal 转 scml" : "scml 转 kanimal";
                    KLog.Info($"读取到目标目录 {targetPath} ，模式：{model}。", true);
                    return targetPath;
                } else {
                    KLog.Error($"快捷方式：{lnk} 的目标 {targetPath} 不是目录。", true);
                    return KLog.ErrorLine(1);
                }
            } else {
                KLog.Error($"当前目录没有指定快捷方式", true);
                return KLog.ErrorLine(2);
            }
        }

        private static void ScmlToKaimal(string dir) {
            string[] files = Directory.GetFiles(".");
            List<string> fileList = files.ToList();
            Reader? reader = null;
            var scml = fileList.Find(path => path.EndsWith(".scml"));
            if (scml == null) {
                KLog.Error("当前目录并不存在 scml 项目，请确认程序在正确的目录。",true);
                return;
            } else {
                var scmlReader = new ScmlReader(scml) {
                    AllowInFramePivots = true,
                    AllowMissingSprites = true,
                    InterpolateMissingFrames = true,
                    Debone = true
                };
                scmlReader.Read();
                KLog.Info("读取文件成功，当前只支持宽松模式，默认去骨骼。", true);
                KLog.Info("开始转换……", true);
                reader = scmlReader;
                var kanimalWriter = new KanimWriter(scmlReader);
                KLog.Info($"转换成功，将把 kanimal 项目存在 {dir} 目录下。", true);
                try {
                    kanimalWriter.SaveToDir(dir);
                    KLog.Success();
                } catch (Exception e) {
                    KLog.Error($"{e.Message}", true);
                }
            }
        }

        private static void KanimalToScml(string dir) {
            string[] files = Directory.GetFiles(dir);
            var png = "";
            var anim = "";
            var build = "";
            Reader? reader = null;
            if (files.Length == 0) {
                KLog.Error("目录：{0} 下不存在文件，请提供正确的目录。", true);
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
                    if (convertList == null)
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
            } else {
                reader = new KanimReader(
                    new FileStream(build!, FileMode.Open),
                    new FileStream(anim!, FileMode.Open),
                    new FileStream(png!, FileMode.Open));
                reader.Read();
                KLog.Info("读取文件成功，只支持宽松模式。开始转换……", true);
                KLog.Info("开始转换……", true);
                var scmlWriter = new ScmlWriter(reader) {
                    FillMissingSprites = true,
                    AllowDuplicateSprites = true
                };
                KLog.Info("转换成功，将把 scml 项目保存到当前目录。", true);
                try {
                    scmlWriter.SaveToDir(".");
                    KLog.Success();
                } catch (Exception e) {
                    KLog.Error($"{e.Message}", true);
                }
            }
        }
        
    }
}