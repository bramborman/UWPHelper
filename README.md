# UWPHelper
UWPHelper is a set of useful UWP tools such as converters, triggers, custom controls and helpers. Currently the [xCalculator][1], [Mytronome][2] and some other apps that are coming soon are using UWPHelper.

[1]: https://www.microsoft.com/store/apps/9nblggh5zbj6
[2]: https://www.microsoft.com/store/apps/9nblggh4r69s

> It's just a Shared Project for good usability when changing its code but I could make it a NuGet package if someone will be interested.

## How to make it work

When you open the solution for the first time, the UWPHelper Sample App will probably not work because of some dependencies in the project...

So to make it work, just follow these steps:
1. Open the solution
2. Right click on the solution in the Solution Explorer window and select "Restore NuGet packages"
3. Wait until the restoring completes
4. Unload the UWPHelper Sample App project by clicking on the "Unload project" option in its context menu
5. Reload it using the "Reload project" option in the UWPHelper Sample App project's context menu
6. You're done :-) Everything should work as expected now

<sub>To remove all warnings, you'll have to generate new temponary signing key... To do that, go to Package.appxmanifest > Packaging and click the "Choose Certificate..." button. In the dialog that will then open, create new certificate using the "Create test certificate..." option in the "Configure certificate..." combobox</sub>