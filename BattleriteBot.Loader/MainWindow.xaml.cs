using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BattleriteBot.Loader.Core;

namespace BattleriteBot.Loader
{
    public class Status
    {
        private static String _text;
        public static String Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnTextChanged(EventArgs.Empty);
            }
        }
        public static event EventHandler TextChanged;
        protected static void OnTextChanged(EventArgs e)
        {
            TextChanged?.Invoke(null, e);
        }
        static Status()
        {
            TextChanged += (sender, e) => { return; };
        }
    }
    public partial class TempMainWindow
    {
        public TempMainWindow()
        {
            InitializeComponent();
            Task.Run(() => { Init(); });
        }
        public void Init()
        {
            var gameName = "Battlerite";
            Status.Text = "Waiting for {gameName} to open";
            var gameProcesses = System.Diagnostics.Process.GetProcessesByName(gameName);
            while (gameProcesses.Length == 0)
            {
                Thread.Sleep(5000);
                gameProcesses = System.Diagnostics.Process.GetProcessesByName(gameName);
            }
            Status.Text = "Game Open - Injecting";
            //try
            {
                String randString = "aa" + Guid.NewGuid().ToString().Substring(0, 8);
                var gameDir = System.IO.Path.GetDirectoryName(gameProcesses[0].MainModule.FileName);
                var gameNamePath = System.IO.Path.GetFileName(gameDir);
                var unityDllPath = gameDir + @"\" + gameNamePath + @"_Data\Managed\";
                Compiler.UnityDllPath = unityDllPath;
                Status.Text = "Injecting - Game @ " + unityDllPath;
                Compiler.UpdateSources();
                var dll = Compiler.CompileDll(randString);
                Injector.Inject(gameName, dll, randString, "Loader", "Load");
                System.IO.File.Delete(randString + ".dll");
                if (System.IO.File.Exists(randString + ".pdb"))
                    System.IO.File.Delete(randString + ".pdb");
                Status.Text = "Injected, closing app shortly";
                //Thread.Sleep(10000);
                Environment.Exit(0);
            }
            //catch (Exception e)
            {
                //Status.Text = e.Message;
            }
        }
    }
}
