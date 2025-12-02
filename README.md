# GucPackageUpdate
A tool to update the encrypted packagelist files for DLC in the Gundam Unicorn game for PS3.  
Without updating these files, the game will reject any changes to DLC content immediately.  

# Requirements
Make sure the .NET 8.0 Runtime is installed or the program cannot run:  
https://dotnet.microsoft.com/en-us/download/dotnet/8.0  

Select ".NET Runtime"  
Most users will need the x64 installer.  
This program has only been tested on Windows x64.  

Users will need to copy the ".rap" files used to decrypt the DLC content to the "/Assets/rap/" folder.  
This is necessary to be able to decrypt and encrypt the packagelists.  
These rap files should be found alongside DLC PKGs used to install content in the first place.  
RPCS3, and probably a real PS3, will store the rap files here when installed:  
/dev_hdd0/home/00000001/exdata/  

# Usage
This tool does not have a GUI.  
Instead a user drags and drops the packagelist folder into GucPackageUpdate.exe.  

The user may also add the full path of the packagelist folder to appconfig.json.  
If using backslashes in the path like "\\", make sure they are changed to "\\\\" instead.  
This will allow users to just launch the exe by clicking on it to immediately update the packagelist.  

# FAQ
Q: Can I add new files to or remove files from the DLC?  
A: Not yet, this tool just updates the current lists.  

# Troubleshooting
Q: The tool immediately closes and does nothing?  
A: See Requirements; This is generally caused by a lack of .NET.  

Q: I drag and drop into the window but nothing happens?  
A: See Usage; Users must drag and drop into the program exe file, not the window it opens.  

Q: The tool says an error occurred about not finding rap files?  
A: See Requirements; Rap files are required to decrypt and encrypt packagelists.  

Q: The game is saying the DLC is corrupted?  
A: Make sure you didn't remove any DLC files and that the updater was ran after; If this still fails make an issue report.  

Q: I have an issue but don't see my problem listed, or the tool threw an error?  
A: Make an issue report and I might get to it at some point.  

# Building
Clone or download this project somewhere:  
```
git clone https://github.com/WarpZephyr/GucPackageUpdate.git  
```

This project requires the following libraries to be cloned alongside it.  
Place them in the same top-level folder as this project.  
These dependencies may change at any time.  
```
git clone https://github.com/WarpZephyr/libps3.git  
git clone https://github.com/WarpZephyr/Edoke.git  
```

Then build the project in Visual Studio 2022.  
Other IDEs or build solutions are untested.