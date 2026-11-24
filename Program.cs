using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;

class Program {
    [STAThread]
    static void Main() {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string audioDir = Path.Combine(baseDir, "Audio");
        if (!Directory.Exists(audioDir)) Directory.CreateDirectory(audioDir);

        string fonixPath = FindFonixData(baseDir);
        string profileUrl = "https://www.nexusmods.com/profile/lodmac/mods";
        string title = "FO4 LIP Generator by Lodmac";

        if (string.IsNullOrEmpty(fonixPath)) {
            MessageBox.Show(
                "FonixData.cdf not found automatically.\nPlease copy 'FonixData.cdf' from your Creation Kit folder (Data\\Sound\\Voice\\Processing\\) next to this EXE.\n\nCreated by Lodmac (Special thanks to Nukem9 for FaceFXWrapper)\n" + profileUrl,
                title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string[] wavFiles = Directory.GetFiles(audioDir, "*.wav");
        if (wavFiles.Length == 0) {
            DialogResult res = MessageBox.Show(
                "The 'Audio' folder has been created next to this EXE!\nPut your WAV files (16-bit PCM, Mono, 44100 Hz) into 'Audio' and run this tool again.\n\nCreated by Lodmac (Special thanks to Nukem9 for FaceFXWrapper)\nOpen my Nexus Mods profile to check out other mods?\n" + profileUrl,
                title, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (res == DialogResult.Yes) Process.Start(new ProcessStartInfo(profileUrl) { UseShellExecute = true });
            return;
        }

        string tempDir = Path.Combine(Path.GetTempPath(), "FO4_LIP_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        string wrapperPath = Path.Combine(tempDir, "fxwrap.exe");

        try {
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("fxwrap.exe"))
            using (FileStream fs = new FileStream(wrapperPath, FileMode.Create)) {
                s.CopyTo(fs);
            }

            int count = 0;
            foreach (string wav in wavFiles) {
                string baseName = Path.GetFileNameWithoutExtension(wav);
                string outLip = Path.Combine(audioDir, baseName + ".lip");
                string tempWav = Path.Combine(tempDir, "temp.wav");

                ProcessStartInfo psi = new ProcessStartInfo {
                    FileName = wrapperPath,
                    Arguments = string.Format("Fallout4 USEnglish \"{0}\" \"{1}\" \"{2}\" \"{3}\" \"Hello there.\"", fonixPath, wav, tempWav, outLip),
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                using (Process p = Process.Start(psi)) {
                    p.WaitForExit();
                }
                if (File.Exists(outLip)) count++;
            }

            DialogResult res = MessageBox.Show(
                string.Format("Success! Generated {0} .lip file(s) inside the 'Audio' folder.\n\nCreated by Lodmac (Special thanks to Nukem9 for FaceFXWrapper)\nOpen my Nexus Mods profile to check out other mods?\n{1}", count, profileUrl),
                title, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (res == DialogResult.Yes) Process.Start(new ProcessStartInfo(profileUrl) { UseShellExecute = true });
        }
        finally {
            try { Directory.Delete(tempDir, true); } catch { }
        }
    }

    static string FindFonixData(string baseDir) {
        string local = Path.Combine(baseDir, "FonixData.cdf");
        if (File.Exists(local)) return local;

        string[] regPaths = {
            @"SOFTWARE\WOW6432Node\Bethesda Softworks\Fallout4",
            @"SOFTWARE\Bethesda Softworks\Fallout4",
            @"SOFTWARE\WOW6432Node\Bethesda Softworks\Skyrim Special Edition",
            @"SOFTWARE\Bethesda Softworks\Skyrim Special Edition",
            @"SOFTWARE\WOW6432Node\Bethesda Softworks\Skyrim"
        };
        foreach (string rp in regPaths) {
            try {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(rp)) {
                    if (key != null) {
                        object val = key.GetValue("Installed Path");
                        if (val != null) {
                            string cand = Path.Combine(val.ToString(), @"Data\Sound\Voice\Processing\FonixData.cdf");
                            if (File.Exists(cand)) return cand;
                        }
                    }
                }
            } catch { }
        }

        string[] drives = { "C", "D", "E", "F" };
        string[] games = { "Fallout 4", "Skyrim Special Edition", "Skyrim" };
        foreach (string d in drives) {
            foreach (string g in games) {
                string c1 = d + @":\Program Files (x86)\Steam\steamapps\common\" + g + @"\Data\Sound\Voice\Processing\FonixData.cdf";
                if (File.Exists(c1)) return c1;
                string c2 = d + @":\SteamLibrary\steamapps\common\" + g + @"\Data\Sound\Voice\Processing\FonixData.cdf";
                if (File.Exists(c2)) return c2;
            }
        }
        return null;
    }
}
