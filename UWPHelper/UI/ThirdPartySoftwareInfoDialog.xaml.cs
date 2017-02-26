using System;
using System.Collections.Generic;

namespace UWPHelper.UI
{
    public sealed partial class ThirdPartySoftwareInfoDialog : AdvancedContentDialog
    {
        private List<ThirdPartySoftwareInfo> ThirdPartySoftwareInfo { get; }

        public ThirdPartySoftwareInfoDialog(List<ThirdPartySoftwareInfo> thirdPartySoftwareInfo)
        {
            ThirdPartySoftwareInfo = thirdPartySoftwareInfo ?? throw new ArgumentNullException(nameof(thirdPartySoftwareInfo));
            InitializeComponent();
        }
    }
}
