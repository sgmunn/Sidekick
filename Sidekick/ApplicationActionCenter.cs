using System.Collections.Generic;
using AppKit;
using Foundation;

namespace Sidekick
{
    public sealed class ApplicationActionCenter : NSObject
    {
        private readonly NSStatusItem statusItem;
        private readonly List<VisualStudioApp> apps;

        public ApplicationActionCenter(NSStatusItem statusItem)
        {
            this.statusItem = statusItem;
            this.apps = new List<VisualStudioApp>();
        }

        public VisualStudioApp SelectedApp { get; private set; }

        // TODO: pass in the app instances
        public void Refresh()
        {
            NSMenu menu = this.statusItem.Menu;
            if (menu == null)
            {
                menu = new NSMenu();
                this.statusItem.Menu = menu;
            }

            menu.RemoveAllItems();
            menu.AddItem(new NSMenuItem("VSMac Instances"));

            // TODO: maintain selection
            this.apps.Clear();
            this.SelectedApp = null;
            this.apps.AddRange(VisualStudioApp.FindApplications());

            for (int i = 0; i < this.apps.Count; i++)
            {
                // use selectors to avoid GC issues
                var item = new NSMenuItem(this.apps[i].AppPath)
                {
                    Tag = i,
                    Action = new ObjCRuntime.Selector("selectApplicationInstance:"),
                    Target = this
                };

                menu.AddItem(item);
            }
        }

        [Export("selectApplicationInstance:")]
        void SelectApplicationInstance(NSMenuItem menuItem)
        {
            if (menuItem != null)
            {
                this.SelectedApp = this.apps[(int)menuItem.Tag];
                foreach (var x in this.statusItem.Menu.Items)
                {
                    if (x != menuItem && x.Action != null && x.Action.Name == "selectApplicationInstance:")
                    {
                        x.State = NSCellStateValue.Off;
                    }
                }

                menuItem.State = NSCellStateValue.On;
            }
        }
    }
}
