using System.Collections.Generic;
using UWPHelper.Utilities;

namespace UWPHelper.UI
{
    public sealed partial class ThirdPartySoftwareInfoDialog : AdvancedContentDialog
    {
        private List<ThirdPartySoftwareInfo> ThirdPartySoftwareInfo { get; }

        public ThirdPartySoftwareInfoDialog(List<ThirdPartySoftwareInfo> thirdPartySoftwareInfo)
        {
            ExceptionHelper.ValidateObjectNotNull(thirdPartySoftwareInfo, nameof(thirdPartySoftwareInfo));

            ThirdPartySoftwareInfo = thirdPartySoftwareInfo;
            InitializeComponent();
        }
    }
}
