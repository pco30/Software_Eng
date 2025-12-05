using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace TestFramework
{
    internal class Program
    {
        private const String TestKeyPath = @"HKEY_CURRENT_USER\Software\Test";
        //private static String testFolderPath = @"C:\Users\J\Desktop\fall_2025\cs673_a1\TestTarget";
        private static String testFolderPath = null;

        static void Main(String[] args)
        {
            // HKCU\Software\Test

            // RegistryAdd
            // RegistryModified
            // RegistryDeleted
            // RegistryAdd -Sub
            // RegistryModified -Sub
            // RegistryDeleted -Sub

            // C:\Users\J\Desktop\fall_2025\cs673_a1\TestTarget

            // FileAdd
            // FileModified
            // FileDeleted
            // FileAdd -Sub
            // FileModified -Sub
            // FileDeleted -Sub

            // ServiceAdd
            // ServiceModified (ExampleServiceToMod)
            // ServiceDeleted (ExampleServiceToDel)

            // -PreInstall

            // HKCU\Software\Test\"DelValue_BINARY" [REG_BINARY]
            // HKCU\Software\Test\"DelValue_DWORD" [REG_DWORD]
            // HKCU\Software\Test\"DelValue_MULTI_SZ" [REG_MULTI]
            // HKCU\Software\Test\"DelValue_QWORD" [REG_QWORD]
            // HKCU\Software\Test\"DelValue_SZ" [REG_SZ]

            // HKCU\Software\Test\"ModValue_BINARY" [REG_BINARY]
            // HKCU\Software\Test\"ModValue_DWORD" [REG_DWORD]
            // HKCU\Software\Test\"ModValue_MULTI_SZ" [REG_MULTI]
            // HKCU\Software\Test\"ModValue_QWORD" [REG_QWORD]
            // HKCU\Software\Test\"ModValue_SZ" [REG_SZ]

            // HKCU\Software\Test\DelSubKey

            // HKCU\Software\Test\DelSubKey\"DelValue_BINARY" [REG_BINARY]
            // HKCU\Software\Test\DelSubKey\"DelValue_DWORD" [REG_DWORD]
            // HKCU\Software\Test\DelSubKey\"DelValue_MULTI_SZ" [REG_MULTI]
            // HKCU\Software\Test\DelSubKey\"DelValue_QWORD" [REG_QWORD]
            // HKCU\Software\Test\DelSubKey\"DelValue_SZ" [REG_SZ]

            // HKCU\Software\Test\ModSubKey

            // HKCU\Software\Test\ModSubKey\"ModValue_BINARY" [REG_BINARY]
            // HKCU\Software\Test\ModSubKey\"ModValue_DWORD" [REG_DWORD]
            // HKCU\Software\Test\ModSubKey\"ModValue_MULTI_SZ" [REG_MULTI]
            // HKCU\Software\Test\ModSubKey\"ModValue_QWORD" [REG_QWORD]
            // HKCU\Software\Test\ModSubKey\"ModValue_SZ" [REG_SZ]

            // C:\Users\J\Desktop\fall_2025\cs673_a1\TestTarget\DelFile.txt
            // C:\Users\J\Desktop\fall_2025\cs673_a1\TestTarget\ModFile.txt

            // C:\Users\J\Desktop\fall_2025\cs673_a1\TestTarget\DelSubFolder

            // C:\Users\J\Desktop\fall_2025\cs673_a1\TestTarget\DelSubFolder\DelSubFile.txt

            // C:\Users\J\Desktop\fall_2025\cs673_a1\TestTarget\ModSubFolder

            // C:\Users\J\Desktop\fall_2025\cs673_a1\TestTarget\ModSubFolder\ModSubFile.txt

            if (args.Length > 0)
            {
                for (Int32 i = 0; i < args.Length - 1; ++i)
                {
                    if (args[i].Equals("-folder", StringComparison.OrdinalIgnoreCase))
                    {
                        testFolderPath = args[i + 1];
                    }
                    else if (args[i].Equals("-key", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("ERROR: -key parameter is not supported");
                        return;
                    }
                }

                if (testFolderPath == null)
                {
                    Console.WriteLine("ERROR: -folder parameter not found");
                    return;
                }

                if (args[args.Length - 1].Equals("-pre", StringComparison.OrdinalIgnoreCase))
                {
                    RegistryKey testKey = OpenRegistryKey(TestKeyPath, writable: true);

                    if (testKey == null)
                    {
                        String[] split = TestKeyPath.Split('\\');

                        RegistryKey baseKey = OpenRegistryKey(split[0]);

                        testKey = baseKey.CreateSubKey(TestKeyPath.Substring(split[0].Length + 1)); // add 1 to skip the \
                    }

                    testKey.SetValue("DelValue_BINARY", new Byte[] { 0x9F, 0x3E, 0x07, 0x80, 0x12, 0x00, 0x00, 0x00 }, RegistryValueKind.Binary);
                    testKey.SetValue("DelValue_DWORD", 16, RegistryValueKind.DWord);
                    testKey.SetValue("DelValue_MULTI_SZ", new String[] { "string1", "string2" }, RegistryValueKind.MultiString);
                    testKey.SetValue("DelValue_QWORD", 240, RegistryValueKind.QWord);
                    testKey.SetValue("DelValue_SZ", "string", RegistryValueKind.String);

                    testKey.SetValue("ModValue_BINARY", new Byte[] { 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01 }, RegistryValueKind.Binary);
                    testKey.SetValue("ModValue_DWORD", 16, RegistryValueKind.DWord);
                    testKey.SetValue("ModValue_MULTI_SZ", new String[] { "original1", "original2" }, RegistryValueKind.MultiString);
                    testKey.SetValue("ModValue_QWORD", 99, RegistryValueKind.QWord);
                    testKey.SetValue("ModValue_SZ", "original", RegistryValueKind.String);

                    testKey.SetValue("TypeChangeValue", 16, RegistryValueKind.DWord);

                    testKey.SetValue("BothChangeValue", 100, RegistryValueKind.DWord);

                    testKey.DeleteValue("AddValue_BINARY", throwOnMissingValue: false);
                    testKey.DeleteValue("AddValue_DWORD", throwOnMissingValue: false);
                    testKey.DeleteValue("AddValue_MULTI_SZ", throwOnMissingValue: false);
                    testKey.DeleteValue("AddValue_QWORD", throwOnMissingValue: false);
                    testKey.DeleteValue("AddValue_SZ", throwOnMissingValue: false);

                    RegistryKey delSubKey = testKey.CreateSubKey("DelSubKey", writable: true);
                    delSubKey.SetValue("DelValue_BINARY", new Byte[] { 0x9F, 0x3E, 0x07, 0x80, 0x12, 0x00, 0x00, 0x00 }, RegistryValueKind.Binary);
                    delSubKey.SetValue("DelValue_DWORD", 16, RegistryValueKind.DWord);
                    delSubKey.SetValue("DelValue_MULTI_SZ", new String[] { "string1", "string2" }, RegistryValueKind.MultiString);
                    delSubKey.SetValue("DelValue_QWORD", 240, RegistryValueKind.QWord);
                    delSubKey.SetValue("DelValue_SZ", "string", RegistryValueKind.String);

                    RegistryKey modSubKey = testKey.CreateSubKey("ModSubKey", writable: true);
                    modSubKey.SetValue("ModValue_BINARY", new Byte[] { 0x9F, 0x3E, 0x07, 0x80, 0x12, 0x00, 0x00, 0x00 }, RegistryValueKind.Binary);
                    modSubKey.SetValue("ModValue_DWORD", 16, RegistryValueKind.DWord);
                    modSubKey.SetValue("ModValue_MULTI_SZ", new String[] { "string1", "string2" }, RegistryValueKind.MultiString);
                    modSubKey.SetValue("ModValue_QWORD", 240, RegistryValueKind.QWord);
                    modSubKey.SetValue("ModValue_SZ", "string", RegistryValueKind.String);

                    modSubKey.SetValue("ModTypeChangeValue", 0x16, RegistryValueKind.DWord);

                    modSubKey.SetValue("ModBothChangeValue", 0x673, RegistryValueKind.DWord);

                    testKey.DeleteSubKey("AddSubKey", throwOnMissingSubKey: false);

                    DirectoryInfo testFolder = new DirectoryInfo(testFolderPath);

                    if (testFolder.Exists)
                    {
                        testFolder.Delete(recursive: true); // delete directory (and all its contents)
                    }

                    testFolder.Create();

                    File.WriteAllText(Path.Combine(testFolderPath, "DelFile"), "");
                    File.WriteAllText(Path.Combine(testFolderPath, "ModFile"), "");
                    File.WriteAllText(Path.Combine(testFolderPath, "FileToFolder"), "");
                    Directory.CreateDirectory(Path.Combine(testFolderPath, "FolderToFile"));

                    Directory.CreateDirectory(Path.Combine(testFolderPath, "DelFolder"));
                    Directory.CreateDirectory(Path.Combine(testFolderPath, "ModFolder"));
                    File.WriteAllText(Path.Combine(testFolderPath, "ModFolder", "ModFolderDelFile"), "");
                    File.WriteAllText(Path.Combine(testFolderPath, "ModFolder", "ModFolderModFile"), "");

                    File.WriteAllText(Path.Combine(testFolderPath, "ModFolder", "ModFolderFileToFolder"), "");
                    Directory.CreateDirectory(Path.Combine(testFolderPath, "ModFolder", "ModFolderFolderToFile"));

                    CreateService("DelService");

                    DeleteService("ModServiceSeT"); // delete first to ensure it's created with default properties
                    CreateService("ModServiceSeT", serviceType: "own");

                    DeleteService("ModServiceStT"); // delete first to ensure it's created with default properties
                    CreateService("ModServiceStT");

                    DeleteService("ModServiceDN"); // delete first to ensure it's created with default properties
                    CreateService("ModServiceDN", overrideDisplayName: "ModServiceDN_OldName");

                    DeleteService("ModServiceSDO"); // delete first to ensure it's created with default properties
                    CreateService("ModServiceSDO");

                    DeleteService("ModServiceAll"); // delete first to ensure it's created with default properties
                    CreateService("ModServiceAll");

                    Console.WriteLine("Pre-Install Configuration Set Successfully");
                }
                else if (args[args.Length - 1].Equals("-runInstall", StringComparison.OrdinalIgnoreCase))
                {
                    RegistryKey testKey = OpenRegistryKey(TestKeyPath, writable: true);

                    testKey.DeleteValue("DelValue_BINARY", throwOnMissingValue: false);
                    testKey.DeleteValue("DelValue_DWORD", throwOnMissingValue: false);
                    testKey.DeleteValue("DelValue_MULTI_SZ", throwOnMissingValue: false);
                    testKey.DeleteValue("DelValue_QWORD", throwOnMissingValue: false);
                    testKey.DeleteValue("DelValue_SZ", throwOnMissingValue: false);

                    testKey.SetValue("AddValue_BINARY", new Byte[] { 0x02, 0x04, 0x06, 0x08, 0x0A, 0x0C, 0x0E, 0x10 }, RegistryValueKind.Binary);
                    testKey.SetValue("AddValue_DWORD", 16, RegistryValueKind.DWord);
                    testKey.SetValue("AddValue_MULTI_SZ", new String[] { "addstring1", "addstring2" }, RegistryValueKind.MultiString);
                    testKey.SetValue("AddValue_QWORD", 240, RegistryValueKind.QWord);
                    testKey.SetValue("AddValue_SZ", "addstring", RegistryValueKind.String);

                    testKey.SetValue("ModValue_BINARY", new Byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 }, RegistryValueKind.Binary);
                    testKey.SetValue("ModValue_DWORD", 16, RegistryValueKind.DWord);
                    testKey.SetValue("ModValue_MULTI_SZ", new String[] { "changed1", "changed2" }, RegistryValueKind.MultiString);
                    testKey.SetValue("ModValue_QWORD", 99, RegistryValueKind.QWord);
                    testKey.SetValue("ModValue_SZ", "original", RegistryValueKind.String);

                    testKey.SetValue("TypeChangeValue", 16, RegistryValueKind.QWord);

                    testKey.SetValue("BothChangeValue", 300, RegistryValueKind.QWord);

                    testKey.DeleteSubKey("DelSubKey", throwOnMissingSubKey: false);

                    RegistryKey addSubKey = testKey.CreateSubKey("AddSubKey", writable: true);
                    addSubKey.SetValue("AddValue_BINARY", new Byte[] { 0x9F, 0x3E, 0x07, 0x80, 0x12, 0x00, 0x00, 0x00 }, RegistryValueKind.Binary);
                    addSubKey.SetValue("AddValue_DWORD", 16, RegistryValueKind.DWord);
                    addSubKey.SetValue("AddValue_MULTI_SZ", new String[] { "string1", "string2" }, RegistryValueKind.MultiString);
                    addSubKey.SetValue("AddValue_QWORD", 240, RegistryValueKind.QWord);
                    addSubKey.SetValue("AddValue_SZ", "string", RegistryValueKind.String);

                    RegistryKey modSubKey = testKey.OpenSubKey("ModSubKey", writable: true);
                    modSubKey.SetValue("ModValue_BINARY", new Byte[] { 0x16, 0x15, 0x14, 0x13, 0x12, 0x11, 0x10, 0x0F }, RegistryValueKind.Binary);
                    modSubKey.SetValue("ModValue_DWORD", 32, RegistryValueKind.DWord);
                    modSubKey.SetValue("ModValue_MULTI_SZ", new String[] { "changed1", "changed2" }, RegistryValueKind.MultiString);
                    modSubKey.SetValue("ModValue_QWORD", 120, RegistryValueKind.QWord);
                    modSubKey.SetValue("ModValue_SZ", "changed", RegistryValueKind.String);

                    modSubKey.SetValue("ModTypeChangeValue", 0x20, RegistryValueKind.DWord);

                    modSubKey.SetValue("ModBothChangeValue", 0x573, RegistryValueKind.QWord);

                    DirectoryInfo testFolder = new DirectoryInfo(testFolderPath);

                    File.Delete(Path.Combine(testFolderPath, "DelFile"));

                    File.WriteAllText(Path.Combine(testFolderPath, "ModFile"), "changed");
                    File.WriteAllText(Path.Combine(testFolderPath, "AddFile"), "");

                    File.Delete(Path.Combine(testFolderPath, "FileToFolder"));
                    Directory.CreateDirectory(Path.Combine(testFolderPath, "FileToFolder"));

                    Directory.Delete(Path.Combine(testFolderPath, "FolderToFile"));
                    File.WriteAllText(Path.Combine(testFolderPath, "FolderToFile"), "");

                    Directory.CreateDirectory(Path.Combine(testFolderPath, "AddFolder"));
                    Directory.CreateDirectory(Path.Combine(testFolderPath, "ModFolder", "ModFolderAddFolder"));
                    Directory.Delete(Path.Combine(testFolderPath, "DelFolder"));

                    File.Delete(Path.Combine(testFolderPath, "ModFolder", "ModFolderDelFile"));
                    File.WriteAllText(Path.Combine(testFolderPath, "ModFolder", "ModFolderModFile"), "changed");
                    File.WriteAllText(Path.Combine(testFolderPath, "ModFolder", "ModFolderAddFile"), "");

                    File.Delete(Path.Combine(testFolderPath, "ModFolder", "ModFolderFileToFolder"));
                    Directory.CreateDirectory(Path.Combine(testFolderPath, "ModFolder", "ModFolderFileToFolder"));

                    Directory.Delete(Path.Combine(testFolderPath, "ModFolder", "ModFolderFolderToFile"));
                    File.WriteAllText(Path.Combine(testFolderPath, "ModFolder", "ModFolderFolderToFile"), "");

                    DeleteService("DelService");

                    CreateService("AddService");

                    ChangeService("ModServiceDN", newDisplayName: "ModServiceDN_NewName");
                    
                    CreateService("serviceDependency1");
                    CreateService("serviceDependency2");
                    ChangeService("ModServiceSDO", newDependencies: "serviceDependency1/serviceDependency2");

                    ChangeService("ModServiceSeT", newServiceType: "own");

                    ChangeService("ModServiceStT", newStartType: "disabled");

                    ChangeService("ModServiceAll", newDisplayName: "ModServiceAll_name", newDependencies: "serviceDependency1/serviceDependency2", newServiceType: "share", newStartType: "disabled");


                    Console.WriteLine("Install Configuration Set Successfully");
                }
                else if (args[args.Length - 1].Equals("-cleanup", StringComparison.OrdinalIgnoreCase))
                {
                    RegistryKey key = OpenRegistryKey(TestKeyPath, writable: true);

                    foreach (String subKeyName in key.GetSubKeyNames())
                    {
                        try
                        {
                            key.DeleteSubKey(subKeyName);
                        }
                        catch
                        {
                            Console.WriteLine($@"ERROR: failed to delete key {key}\{subKeyName}");
                        }
                    }

                    foreach (String valueName in key.GetValueNames())
                    {
                        try
                        {
                            key.DeleteValue(valueName);
                        }
                        catch
                        {
                            Console.WriteLine($@"ERROR: failed to delete value {key}\'{valueName}'");
                        }
                    }


                    DirectoryInfo testFolder = new DirectoryInfo(testFolderPath);

                    if (testFolder.Exists)
                    {
                        testFolder.Delete(recursive: true); // delete directory (and all its contents)
                    }

                    DeleteService("ModServiceAll");
                    DeleteService("ModServiceSDO");
                    DeleteService("serviceDependency1");
                    DeleteService("serviceDependency2");
                    DeleteService("AddService");
                    DeleteService("ModServiceDN");
                    DeleteService("ModServiceStT");
                    DeleteService("ModServiceSeT");

                    Console.WriteLine("Cleanup Run Successfully");
                }
            }

            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }

        private const String HKCR = "HKEY_CLASSES_ROOT";
        private const String HKCU = "HKEY_CURRENT_USER";
        private const String HKLM = "HKEY_LOCAL_MACHINE";
        private const String HKU = "HKEY_USERS";
        private const String HKCC = "HKEY_CURRENT_CONFIG";

        private static RegistryKey OpenRegistryKey(String keyPath, Boolean writable = false)
        {
            RegistryKey key = null;

            if (keyPath.StartsWith(HKCR, StringComparison.OrdinalIgnoreCase))
            {
                key = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64);
            }
            else if (keyPath.StartsWith(HKCU, StringComparison.OrdinalIgnoreCase))
            {
                key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
            }
            else if (keyPath.StartsWith(HKLM, StringComparison.OrdinalIgnoreCase))
            {
                key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            }
            else if (keyPath.StartsWith(HKU, StringComparison.OrdinalIgnoreCase))
            {
                key = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64);
            }
            else if (keyPath.StartsWith(HKCC, StringComparison.OrdinalIgnoreCase))
            {
                key = RegistryKey.OpenBaseKey(RegistryHive.CurrentConfig, RegistryView.Registry64);
            }

            if (key == null)
            {
                return null;
            }

            String[] split = keyPath.Split('\\');

            for (Int32 i = 1; i < split.Length; ++i)
            {
                key = key.OpenSubKey(split[i], writable);

                if (key == null)
                {
                    return null;
                }
            }

            return key;
        }

        private static Boolean CreateService(String serviceName, String overrideDisplayName = null, String serviceType = null)
        {
            if (serviceName == null)
            {
                throw new ArgumentNullException("serviceName cannot be null");
            }

            StringBuilder sb = new StringBuilder($"create \"{serviceName}\" binPath= \"{Path.Combine(testFolderPath, "MyService.exe")}\" DisplayName= \"{(overrideDisplayName != null ? overrideDisplayName : serviceName)}\"");

            if (serviceType != null)
            {
                sb.Append($" type= {serviceType}");
            }

            using (Process process = Process.Start(new ProcessStartInfo
            {
                FileName = "sc.exe",
                Arguments = sb.ToString(),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }))
            {
                Console.WriteLine($"{process.StartInfo.FileName} {process.StartInfo.Arguments}");

                String output = process.StandardOutput.ReadToEnd();
                String error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"ERROR - failed to create service \"{serviceName}\":");
                    if (error.Length > 0)
                    {
                        Console.WriteLine(error);
                    }
                    else
                    {
                        Console.WriteLine(output);
                    }

                    return false;
                }
                else
                {
                    Console.WriteLine($"Successfully created service \"{serviceName}\"");
                }
            }

            return true;
        }
        private static Boolean DeleteService(String serviceName)
        {
            using (Process process = Process.Start(new ProcessStartInfo
            {
                FileName = "sc.exe",
                Arguments = $"delete \"{serviceName}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }))
            {
                String output = process.StandardOutput.ReadToEnd();
                String error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"ERROR - failed to delete service \"{serviceName}\":");
                    if (error.Length > 0)
                    {
                        Console.WriteLine(error);
                    }
                    else
                    {
                        Console.WriteLine(output);
                    }

                        return false;
                }

                return true;
            }
        }

        private static Boolean ChangeService(String serviceName, String newDisplayName = null, String newDependencies = null, String newServiceType = null, String newStartType = null)
        {
            StringBuilder sb = new StringBuilder($"config \"{serviceName}\"");

            Boolean valid = false;

            if (newServiceType != null)
            {
                sb.Append($" type= {newServiceType}");
                valid = true;
            }

            if (newStartType != null)
            {
                sb.Append($" start= {newStartType}");
                valid = true;
            }

            if (newDisplayName != null)
            {
                sb.Append($" DisplayName= {newDisplayName}");
                valid = true;
            }

            if (newDependencies != null)
            {
                sb.Append($" depend= {newDependencies}");
                valid = true;
            }

            if (!valid)
            {
                Console.WriteLine("ERROR: ChangeService - at least one of newServiceType/newStartType/newDisplayName/newDependencies");
            }

            using (Process process = Process.Start(new ProcessStartInfo
            {
                FileName = "sc.exe",
                Arguments = sb.ToString(),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }))
            {
                String output = process.StandardOutput.ReadToEnd();
                String error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"Error updating service \"{serviceName}\"");
                    if (newDisplayName != null)
                    {
                        Console.WriteLine($"    newDisplayName = \"{newDisplayName}\"");
                    }
                    if (newDependencies != null)
                    {
                        Console.WriteLine($"    newDependencies = \"{newDependencies}\"");
                    }
                    Console.WriteLine(error);

                    return false;
                }

                return true;
            }
        }
    }
}
