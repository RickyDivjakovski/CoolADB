## CoolADB
# Client info
A C# Android Debug Bridge client for .net.

# Method and function examples
-Create the client
ADBClient MyClient = new ADBClient();

-Set the path of adb.exe here or leave null if PATH is defined or adb.exe is in the same directory as the host application
MyClient.AdbPath = @"C:\path\to\adb.exe";

-Connect via ip
MyClient.Connect("192.192.192.192");

-Disconnect
MyClient.Disconnect();

-Kill server
MyClient.KillServer();

-Start server
MyClient.StartServer();

-Get output of adb(string)
string adbOutput = MyClient.Output;

-List device(array)
MyClient.Devices()[0];

-Exececute command on device(2nd boot is if root is needed)
MyClient.Command("rm -rf /system/app/myApp.apk", true);

-Remount system as r/w(bypasses cannot be run in production builds)
MyClient.Remount();

-Reboot to any state(arg is a bootstate enum)
MyClient.Reboot(ADBClient.BootState.Recovery);

-Push file(forward or backslashes dont matter)
MyClient.Push(@"C:\path\to\my\file", "/sdcard");

-Pull file(forward or backslashes dont matter)
MyClient.Push("/sdcard/myFile", @"C:\myFile");

-Install application(forward or backslashes dont matter)
MyClient.Install(@"C:\path\to\apk");

-Uninstall application
MyClient.Uninstall(com.my.package.name);

-Backup(2nd arg is backup args)
MyClient.Backup(@"C:\backup.ab", "-noapk")

-Restore
MyClient.Restore(@"C:\backup.ab");

-Logcat(2nd arg is overwrite file if exists)
MyClient.Logcat(@"C:\logcat.txt", true);

# Contributors
Myself - Ricky Divjakovski

# License
GNU GPL v3