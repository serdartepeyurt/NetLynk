namespace NetLynk.Types
{
    using Utils;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public class Project
    {
        public Project()
        {
            this.UpdatedWidgets = new List<Widget>();
        }

        public int Id { get; set; }
        public int ParentId { get; set; }
        public bool IsPreview { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public long CreatedAt { get; set; }
        [JsonIgnore]
        public long UpdatedAt { get; set; }

        public TrueObservableCollection<Widget> Widgets { get; set; }
        public TrueObservableCollection<Device> Devices { get; set; }
        public string Theme { get; set; }
        public bool KeepScreenOn { get; set; }
        public bool IsAppConnectedOn { get; set; }
        public bool IsShared { get; set; }
        public bool IsActive { get; set; }

        [JsonIgnore]
        public List<Widget> UpdatedWidgets { get; set; }

        public void Update(Project project)
        {
            this.Id = project.Id;
            this.ParentId = project.ParentId;
            this.IsPreview = project.IsPreview;
            this.Name = project.Name;
            this.CreatedAt = project.CreatedAt;
            this.UpdatedAt = project.UpdatedAt;
            this.Theme = project.Theme;
            this.KeepScreenOn = project.KeepScreenOn;
            this.IsAppConnectedOn = project.IsAppConnectedOn;
            this.IsShared = project.IsShared;
            this.IsActive = project.IsActive;

            var removedWidgets = this.Widgets.Except(project.Widgets, new WidgetIdComparer());
            foreach(var removedWidget in removedWidgets)
            {
                this.Widgets.Remove(removedWidget);
            }

            var addedWidgets = project.Widgets.Except(this.Widgets, new WidgetIdComparer());
            foreach (var addedWidget in addedWidgets)
            {
                this.Widgets.Add(addedWidget);
            }

            var changed = false;
            foreach (var widget in project.Widgets)
            {
                var wdgt = this.Widgets.FirstOrDefault(w => w.Id == widget.Id);
                if(wdgt != null)
                {
                    if (this.UpdatedWidgets?.Any(w => w.Id == widget.Id) ?? false)
                    {
                        continue;
                    }

                    if(wdgt.Label != widget.Label)
                    {
                        wdgt.Label = widget.Label;
                        changed = true;
                    }
                    
                    if(wdgt.Value != widget.Value)
                    {
                        wdgt.Value = widget.Value;
                        changed = true;
                    }
                }
            }

            if (changed)
            {
                this.Widgets.FireCollectionChanged();
            }
            this.UpdatedWidgets?.Clear();

            this.Devices?.Clear();
            foreach (var device in project.Devices)
            {
                this.Devices.Add(device);
            }
        }
    }
}