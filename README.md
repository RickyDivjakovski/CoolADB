# CoolADB
## Client info
A C# Android Debug Bridge client for .net.

## Method and function examples
-Create the client  
ADBClient myClient = new ADBClient();

-Set the path of adb.exe here or leave null if PATH is defined or adb.exe is in the same directory as the host application  
myClient.AdbPath = @"C:\path\to\adb.exe";

-Connect via ip  
myClient.Connect("192.192.192.192");

-Disconnect  
myClient.Disconnect("192.192.192.192");

-Kill server  
myClient.KillServer();

-Start server  
myClient.StartServer();

-Get output of adb(string)  
string adbOutput = myClient.Output;

-List devices(array)  
myClient.Devices();

-Exececute command on device(2nd boot is if root is needed)  
myClient.Command("rm -rf /system/app/myApp.apk", true);

-Remount system as r/w(bypasses cannot be run in production builds)  
myClient.Remount();

-Reboot to any state(arg is a bootstate enum)  
myClient.Reboot(ADBClient.BootState.Recovery);

-Push file(forward or backslashes dont matter)  
myClient.Push(@"C:\path\to\my\file", "/sdcard");

-Pull file(forward or backslashes dont matter)  
myClient.Push("/sdcard/myFile", @"C:\myFile");

-Install application(forward or backslashes dont matter)  
myClient.Install(@"C:\path\to\apk");

-Uninstall application  
myClient.Uninstall("com.my.package.name");

-Backup(2nd arg is backup args)  
myClient.Backup(@"C:\backup.ab", "-noapk")

-Restore  
myClient.Restore(@"C:\backup.ab");

-Logcat(2nd arg is overwrite file if exists)  
myClient.Logcat(@"C:\logcat.txt", true);

## Contributors
Myself - Ricky Divjakovski

## License
GNU GPL v3
