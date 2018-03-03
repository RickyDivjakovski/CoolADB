using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CoolADB
{
    public partial class ADBClient : Component
    {
        // ----------------------------------------- Adb.exe path, leave blank if in same directory as app or included in PATH
        private string adbPath = "adb";
        public string AdbPath
        {
            get { return adbPath; }
            set
            {
                if (File.Exists(value)) adbPath = value;
                else adbPath = "\"" + adbPath + "\"";
            }
        }

        // ----------------------------------------- Adb command timeout - usable in push and pull to avoid hanging while executing
        private int adbTimeout;
        public int AdbTimeout
        {
            get { return adbTimeout > 0 ? adbTimeout : 5000; }
            set { adbTimeout = value; }
        }

        // ----------------------------------------- Create our emulated shell here and assign events

        // Create a background thread an assign work event to our emulated shell method
        BackgroundWorker CMD = new BackgroundWorker();
        private Process Shell;

        public ADBClient()
        {
            CMD.DoWork += new DoWorkEventHandler(CMD_Send);
        }

        // Needed data types for our emulated shell
        string Command = "";
        bool Complete = false;

        // Create an emulated shell for executing commands
        private void CMD_Send(object sender, DoWorkEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = "cmd.exe",
                Arguments = "/C \"" + Command + "\""
            };

            using (Process process = Process.Start(startInfo))
            {
                if (Command.StartsWith("\"" + adbPath + "\" logcat"))
                {
                    Complete = true;
                    process.WaitForExit();
                    return;
                }

                if (!process.WaitForExit(AdbTimeout))
                    process.Kill();

                Output = process.StandardOutput.ReadToEnd();
                Complete = true;
            };
        }

        // Send a command to emulated shell
        private void SendCommand(string command)
        {
            CMD.WorkerSupportsCancellation = true;
            Command = command;
            CMD.RunWorkerAsync();
            while (!Complete) Sleep(500);
            Complete = false;
        }

        // Sleep until output
        public void Sleep(int milliseconds)
        {
            DateTime delayTime = DateTime.Now.AddMilliseconds(milliseconds);
            while (DateTime.Now < delayTime)
            {
                Application.DoEvents();
            }
        }

        // Bootstate for rebooting
        public enum BootState
        {
            System, Bootloader, Recovery
        }

        // ----------------------------------------- Allow public modifiers to get output

        public string Output { get; private set; }

        // ----------------------------------------- Functions

        public void Connect(string ip)
        {
            SendCommand("\"" + adbPath + "\" connect " + ip);
        }

        public void Disconnect(decimal ip)
        {
            SendCommand("\"" + adbPath + "\" disconnect " + ip);
        }

        public void StartServer()
        {
            SendCommand("\"" + adbPath + "\" start-server");
        }

        public void KillServer()
        {
            SendCommand("\"" + adbPath + "\" kill-server");
        }

        public List<string> Devices()
        {
            SendCommand("\"" + adbPath + "\" devices");

            string[] outLines = Output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return outLines.Skip(1).ToList();
        }

        public void Execute(string command, bool asroot)
        {
            if (asroot) SendCommand("\"" + adbPath + "\" shell su -c \"" + command + "\"");
            else SendCommand("\"" + adbPath + "\" shell " + command);
        }

        public void Remount()
        {
            SendCommand("\"" + adbPath + "\" shell su -c \"mount -o rw,remount /system\"");
        }

        public void Reboot(BootState boot)
        {
            if (boot == BootState.System) SendCommand("\"" + adbPath + "\" shell su -c \"reboot\"");
            if (boot == BootState.Bootloader) SendCommand("\"" + adbPath + "\" shell su -c \"reboot bootloader\"");
            if (boot == BootState.Recovery) SendCommand("\"" + adbPath + "\" shell su -c \"reboot recovery\"");
        }

        public void Push(string input, string output)
        {
            try { SendCommand("\"" + adbPath + "\" push \"" + input + "\" \"" + output + "\""); } catch { try { SendCommand("\"" + adbPath + "\" push \"" + input.Replace("/", "\\") + "\" \"" + output + "\""); } catch { } }
        }

        public void Pull(string input, string output)
        {
            if (output != null && !string.IsNullOrWhiteSpace(output)) try { SendCommand("\"" + adbPath + "\" pull \"" + input + "\" \"" + output + "\""); } catch { try { SendCommand("\"" + adbPath + "\" pull \"" + input + "\" \"" + output.Replace("/", "\\") + "\""); } catch { } }
            else try { SendCommand("\"" + adbPath + "\" pull \"" + input + "\""); } catch { }
        }

        public void Install(string application)
        {
            try { SendCommand("\"" + adbPath + "\" install \"" + application + "\""); } catch { try { SendCommand("\"" + adbPath + "\" install \"" + application.Replace("/", "\\") + "\""); } catch { } }
        }

        public void Uninstall(string packageName)
        {
            SendCommand("\"" + adbPath + "\" uninstall \"" + packageName + "\"");
        }

        public void Backup(string backupPath, string backupArgs)
        {
            if (backupArgs != null && !string.IsNullOrWhiteSpace(backupArgs)) SendCommand("\"" + adbPath + "\" backup \"" + backupPath + "\" " + "\"" + backupArgs + "\"");
            else SendCommand("\"" + adbPath + "\" backup \"" + backupPath + "\"");
        }

        public void Restore(string backupPath)
        {
            try { SendCommand("\"" + adbPath + "\" restore \"" + backupPath + "\""); } catch { try { SendCommand("\"" + adbPath + "\" restore \"" + backupPath.Replace("/", "\\") + "\""); } catch { } }
        }

        public void Logcat(string logPath, bool overWrite)
        {
            if (overWrite == true) try { SendCommand("\"" + adbPath + "\" logcat > \"" + logPath + "\""); } catch { try { SendCommand("\"" + adbPath + "\" logcat > \"" + logPath.Replace("/", "\\") + "\""); } catch { } }
            else try { SendCommand("\"" + adbPath + "\" logcat >> \"" + logPath + "\""); } catch { try { SendCommand("\"" + adbPath + "\" logcat >> \"" + logPath.Replace("/", "\\") + "\""); } catch { } }
        }
    }
}
