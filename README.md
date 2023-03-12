# EveIntelChecker
A compact desktop tool that filters relevant messages across Intel chat channels.

**The screenshots on this page need to be updated and are therefore no longer contractual.**

## Working on

- Windows 10 / 11
- MacOS Intel / ARM (*Manual building required*)

## Installation

Download the latest release [here](https://github.com/SebastienDuruz/Eve-Intel-Checker/releases)

### Build for MacOS
On **MacOS** the application use [Electron.NET](https://github.com/ElectronNET/Electron.NET) as replacement for WPF. Make sure you have **all the dependencies** required for this library.

Install Electron.NET Dotnet tool :
```
dotnet tool install --global h5.ElectronNET.CLI
```

Add Dotnet tools to your PATH (this command may vary depending on your system) :
```
export PATH="$PATH:/Users/YOUR_USERNAME/.dotnet/tools"
or
export PATH="$HOME/.dotnet/tools:$PATH"
```

Download the source code and move into the correct folder :
```
git clone https://github.com/SebastienDuruz/Eve-Intel-Checker.git
cd Eve-Intel-Checker/EveIntelChecker/EveIntelCheckerElectron
```

Build the project for the targeted architecture :

*MacOS Intel*
```
electronize-h5 build /target osx
```
*MacOS ARM*
```
electronize-h5 build /target osx-arm
```

Finally, access the built application :
```
cd bin/Desktop/TARGETED_ARCHITECTURE
```

## Usage

### Eve Online Options

Make sure **Log Chat to File** is activated. If it's not the case, activate the option and restart the Eve client.

<img align="center" height="250" src=".\Screenshots\ChatLogOption.png">

### Select the system you're currently in


Use the autocomplete form to select the system. Press **Enter** to validate selection.

<img align="center" height="300" src=".\Screenshots\SelectSystem.png"><img align="left" height="300" src=".\Screenshots\SelectedSystem.png">

### Open chat log file

Click the red **OpenFile** icon and select the desired text file. 

Chatlogs are stored in ***Document/EVE/logs/Chatlogs***

<img align="left" height="300" src=".\Screenshots\OpenLogFile.png"><img align="left" height="120" src=".\Screenshots\SelectLogFile.png"><img align="center" height="300" src=".\Screenshots\SelectedLogFile.png">

Once selected, **OpenFile** icon turn green.

The App is now ready to listen for new intel messages.

The application automatically checks for a new version of the **same chatlog** file. So Once the chat log file is configured, you should no longer have to do anything.

### App running

Each time a message contains system near your position is sent :

- Last message label is updated
- An **audible signal** is triggered
- The system is now **red** until new relevant message is trigger
- Triggered column is incremented

<img align="center" height="300" src=".\Screenshots\Detected.png">

Each time irrelevant message is sent :

- Last message label is updated

<img align="center" height="300" src=".\Screenshots\Undetected.png">

### Other functionalities

#### Settings Menu
You can open/close application settings by clicking on the **settings icon**.

<img align="left" height="300" src=".\Screenshots\OpenSettings.png"><img align="center" height="375" src=".\Screenshots\OpenedSettings.png">
##### Jumps from root
Max jumps to check from the root system (your current position)
##### Danger notifications
Max jumps before switching from **Danger** to **Classic** sound alert
##### Mute notifications
Max jumps before muting sound alerts (only visual modifications will continue to trigger)
##### Volume
Volume of the sound alerts
##### Compact mode
Default mode is compact. An alternative mode is available. This mode display a network map instead of a list
##### Always on top
By default the application stay **Top most** you can disable this behaviour. An application restart is **required** to apply this settings
##### Keyboard Shortcuts
**Activate/Desactivate** the keyboard shortcuts, by default this settings is activated

#### Compact Mode
You can **left click** on a system to access more functionalities :

- Access **ZKillboard** system page
- Access **Dotlan** system map
- Set as Root (Change actual position for the selected one)

You can reset the **triggers counters** by pressing **Reset icon**.

<img align="left" height="300" src=".\Screenshots\Reset.png"><img align="center" height="300" src=".\Screenshots\Reseted.png">

#### Starmap Mode
- Zoom In and Out with **mouse wheel**
- Move the map around with **left click**
- Reorganise nodes on the map with **left click**
- You can manually center the map by clicking the **Align icon**
<img align="center" height="400" src=".\Screenshots\StarMap.png">

#### Secondary Window
It's possible to have a secondary window that works as an independant Intel.

To *open/close* the secondary window press **CTRL+T** (if keyboards shortcuts are activated on the settings) or simply click the **open icon** on the top bar.

### Custom alert sounds
Both alert sounds can be found in the root of the installation folder, next to the executable.

You can replace *danger_[1 or 2].wav* and *notif_[1 or 2].wav* by any other ***.wav*** sound file, but file names have to still **be identical**.

## License

**EveIntelChecker** is open-sourced software licensed under [GNU General Public License v3.0](LICENSE)

