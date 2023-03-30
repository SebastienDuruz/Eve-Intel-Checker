# EveIntelChecker
A compact desktop tool for filtering relevant messages across Intel chat channels.

**Please note: The screenshots on this page are outdated and should be updated. As a result, they are no longer considered contractual.**

## Supported Platforms
- Windows 10 / 11
- MacOS Intel / ARM (Manual building required)

## Installation

Download the latest release [here](https://github.com/SebastienDuruz/Eve-Intel-Checker/releases)

### Build for MacOS
Ensure that you have **all the required** dependencies for this library

1) Install Electron.NET Dotnet tool :
```
dotnet tool install --global h5.ElectronNET.CLI
```

2) Add Dotnet tools to your PATH (this command may vary depending on your system) :
```
export PATH="$PATH:/Users/YOUR_USERNAME/.dotnet/tools"
or
export PATH="$HOME/.dotnet/tools:$PATH"
```

3) Download the source code and move into the correct folder :
```
git clone https://github.com/SebastienDuruz/Eve-Intel-Checker.git
cd Eve-Intel-Checker/EveIntelChecker/EveIntelCheckerElectron
```

4) Build the project for the targeted architecture :

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

Ensure that **the Log Chat to File** option is activated. If it is not, activate the option and restart the Eve client.

<img align="center" height="250" src=".\Screenshots\ChatLogOption.png">

### Select your current system

Use the autocomplete form to select the system you're currently in. Press Enter to validate your selection.

<img align="center" height="300" src=".\Screenshots\SelectSystem.png"><img align="left" height="300" src=".\Screenshots\SelectedSystem.png">

### Open chat log file

Click the red **OpenFile** icon and select the desired text file. 

Chatlogs are stored in ***Document/EVE/logs/Chatlogs***

<img align="left" height="300" src=".\Screenshots\OpenLogFile.png"><img align="left" height="120" src=".\Screenshots\SelectLogFile.png"><img align="center" height="300" src=".\Screenshots\SelectedLogFile.png">

Once selected, **OpenFile** icon turns green.

The App is now ready to listen for new intel messages.

The application automatically checks for a new version of the **same chatlog** file. Once the chat log file is configured, you should no longer have to make any adjustments.

### App running

Each time a message contains system near your position is sent :

- The **Last message** label is updated
- An **audible signal** is triggered
- The system turns **red** until new relevant message is triggered
- the Triggered column is incremented

<img align="center" height="300" src=".\Screenshots\Detected.png">

Each time irrelevant message is sent :

- The **Last message** label is updated

<img align="center" height="300" src=".\Screenshots\Undetected.png">

### Other functionalities

#### Settings Menu
You can open/close the application settings by clicking on the **settings icon**.

<img align="left" height="300" src=".\Screenshots\OpenSettings.png"><img align="center" height="375" src=".\Screenshots\OpenedSettings.png">
##### Jumps from root
Max jumps to check from the root system (your current position)
##### Danger notifications
Max jumps before switching from **Danger** to **Classic** sound alert
##### Mute notifications
Max jumps before muting sound alerts, visual modifications will continue to trigger
##### Volume
Volume of the sound alerts
##### Compact mode
Default mode is **compact**. An alternative mode is available. This mode display a **network map** instead of a list
##### Always on top
By default the application stay **Top most** you can disable this behaviour. An application restart is **required** to apply this settings
##### Keyboard Shortcuts
**Activate/Desactivate** the keyboard shortcuts, by default this settings is activated

#### Compact Mode
You can **left click** on a system to access more functionalities :

- Access **ZKillboard** system page
- Access **Dotlan** system map
- Set as Root (Change the current position for the selected one)

You can reset the **triggers counters** by pressing the **Reset icon**.

<img align="left" height="300" src=".\Screenshots\Reset.png"><img align="center" height="300" src=".\Screenshots\Reseted.png">

#### Starmap Mode
- Zoom In and Out with the **mouse wheel**
- Move the map around with the **left click**
- Reorganise nodes on the map with the **left click**
- Manually center the map by clicking the **Align icon**
<img align="center" height="400" src=".\Screenshots\StarMap.png">

#### Secondary Window
It's possible to have a secondary window that works as an independant Intel.

To *open/close* the secondary window press **CTRL+T** (if keyboards shortcuts are activated on the settings) or simply click the **open icon** on the top bar.

### Custom alert sounds
Both alert sounds can be found in the root of the installation folder, next to the executable.

You can replace *danger_[1 or 2].wav* and *notif_[1 or 2].wav* by any other ***.wav*** sound file, but the file names must **remain identical**.

## License

**EveIntelChecker** is open-sourced software licensed under [GNU General Public License v3.0](LICENSE)

