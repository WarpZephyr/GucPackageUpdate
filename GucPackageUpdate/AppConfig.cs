using GucPackageUpdate.Configuration;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace GucPackageUpdate
{
    [JsonSourceGenerationOptions(WriteIndented = true,
        GenerationMode = JsonSourceGenerationMode.Metadata,
        IncludeFields = true,
        UseStringEnumConverter = true)]
    [JsonSerializable(typeof(AppConfig))]
    internal partial class AppConfigSerializerContext : JsonSerializerContext
    {
    }

    internal class AppConfig : IConfig
    {
        [JsonIgnore]
        private const string FileName = "appconfig.json";

        [JsonIgnore]
        internal static readonly AppConfig Current;

        public string PackageListPath;

        static AppConfig()
        {
            Current = Load();
        }

        public AppConfig()
        {
            PackageListPath = string.Empty;
        }

        public static AppConfig Load()
            => ConfigLoader.Load(FileName, AppConfigSerializerContext.Default.AppConfig);

        public void Save()
            => ConfigLoader.Save(this, FileName, AppConfigSerializerContext.Default.AppConfig);
    }
}
