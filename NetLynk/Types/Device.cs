using System.ComponentModel;

namespace NetLynk.Types
{

    public class Device : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BoardType { get; set; }
        public string Token { get; set; }
        public string ConnectionType { get; set; }
        public string Status { get; set; }
        public long DisconnectTime { get; set; }
        public string LastLoggedIP { get; set; }
        public HardwareInfo HardwareInfo { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
