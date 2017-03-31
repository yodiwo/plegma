using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Yodiwo
{
    public interface IYConfigEntry
    {
    }

    public abstract class YConfigBase
    {
        public string ActiveID;

        [JsonIgnore]
        public string Filename;

        public YConfigBase(string fname) { this.Filename = FindFile(fname); }
        public YConfigBase() : this(null) { }

        public abstract bool Retrieve(Func<string, string> PreProccessContent = null);
        public abstract bool Save();

        public static int GetActiveID(string confTxt)
        {
            var json = confTxt.FromJSON() as Newtonsoft.Json.Linq.JObject;
            return (int)json[nameof(YConfigBase.ActiveID)];
        }

        public static string ChangeActiveID(string confTxt, string newID)
        {
            var json = confTxt.FromJSON() as Newtonsoft.Json.Linq.JObject;
            json[nameof(YConfigBase.ActiveID)] = newID;
            return json.ToString();
        }

        static string FindFile(string fname)
        {
            //default filename
            if (string.IsNullOrWhiteSpace(fname))
                return null;

            //check if exists
            if (File.Exists(fname))
                return fname;

            try
            {
                //find current assembly dir
                string asmdir = null;
#if !UNIVERSAL
                try { asmdir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); } catch { }
#endif
                //check if conf file exists under the current assembly dir
                if (asmdir != null && File.Exists(Path.Combine(asmdir, fname)))
                    return Path.Combine(asmdir, fname);
#if !UNIVERSAL
                else if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fname)))
                    return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fname);
                else if (File.Exists(Path.Combine(Environment.CurrentDirectory, fname)))
                    return Path.Combine(Environment.CurrentDirectory, fname);
#endif
                else
                    return fname;
            }
            catch { return fname; }
        }
    }

    public class YConfig<T> : YConfigBase where T : IYConfigEntry
    {
        public Dictionary<string, T> Configs;

        [JsonIgnore]
        public bool IsValid { get { return Configs != null && Configs.ContainsKey(ActiveID); } }

        public YConfig() : base() { }
        public YConfig(string fname) : base(fname) { }

        public override bool Retrieve(Func<string, string> PreProccessContent = null)
        {
            if (string.IsNullOrWhiteSpace(this.Filename))
                return false;

            //check paths
            var yconfig = Retrieve(this.Filename, PreProccessContent: PreProccessContent);
            if (yconfig != null && yconfig.IsValid)
            {
                ActiveID = yconfig.ActiveID;
                Configs = yconfig.Configs;
                return true;
            }
            return false;
        }

        private static YConfig<T> Retrieve(string filename, Func<string, string> PreProccessContent = null)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return null;

            try
            {
                //Read in configuration array from file pointed to by input param (or falling back to default file)
#if NETFX
                var content = File.ReadAllText(filename);
#elif UNIVERSAL
                var ss = new Tools.StorageService(false);
                var content = ss.LoadFile(filename).GetResults();
#endif
                if (PreProccessContent != null)
                    content = PreProccessContent(content);
                //deserialize into List of Configurations and pick the active one
                return content.FromJSON<YConfig<T>>();
            }
            catch (Exception ex)
            {
                DebugEx.TraceLog("Failed to read config file : " + ex.Message);
                return null;
            }
        }

        public override bool Save()
        {
            if (string.IsNullOrWhiteSpace(this.Filename))
                return false;

            try
            {
                String cfg = JsonConvert.SerializeObject(this, Formatting.Indented);
#if NETFX
                File.WriteAllText(Filename, cfg);
                return true;
#elif UNIVERSAL
                var ss = new Tools.StorageService(false);
                return ss.SaveFile(Filename, cfg).GetResults();
#endif
            }
            catch (Exception ex)
            {
                DebugEx.TraceLog("Error: Failed to write configFile {0}", ex.Message);
                return false;
            }
        }

        public T GetActiveConf(string confid = null)
        {
            return (T)this.Configs[(confid != null) ? confid : this.ActiveID];
        }

        public void AddActiveConf(string Name, T cfg)
        {
            if (this.Configs == null)
                this.Configs = new Dictionary<string, T>();

            this.Configs.Add(Name, cfg);
            this.ActiveID = Name;
        }
    }
}
