namespace NetLynk.Types
{
    using Newtonsoft.Json;
    using System.ComponentModel;
    using System.Threading.Tasks;

    public class Widget : INotifyPropertyChanged
    {
        private string value;

        public string Type { get; set; }
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Color { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int TabId { get; set; }
        public string Label { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsDefaultColor { get; set; }
        public int DeviceId { get; set; }
        public PinType PinType { get; set; }
        public int Pin { get; set; }
        public bool PwmMode { get; set; }
        public bool RangeMappingOn { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                Task.Run(async () => await Lynk.SetDigitalPinValue(this.Pin, value == "1" ? true : false));
                this.Project?.UpdatedWidgets?.Add(this);
                this.value = value;
            }
        }
        public bool? PushMode { get; set; }
        public string OnLabel { get; set; }
        public string OffLabel { get; set; }

        [JsonIgnore]
        public Project Project { get; set; }

        [JsonIgnore]
        public NetLynk Lynk { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}