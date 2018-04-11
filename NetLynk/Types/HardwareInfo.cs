namespace NetLynk.Types
{

    public class HardwareInfo
    {
        public string Version { get; set; }
        public string BoardType { get; set; }
        public string CpuType { get; set; }
        public string ConnectionType { get; set; }
        public string Build { get; set; }
        public int HeartbeatInterval { get; set; }
    }
}
