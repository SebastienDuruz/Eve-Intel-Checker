# EveIntelChecker
A compact desktop tool used to filter relevant messages on Intel chat channels.

## Requirements

- Windows 10 / 11
- Mac (Work in progress)

## Installation

Download the latest release [here](https://github.com/SebastienDuruz/Eve-Intel-Checker/releases)

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

You can **left click** on a system to access more functionalities :

- Access **ZKillboard** system page
- Access **Dotlan** system map
- Set as Root (Change actual position for the selected one)

You can reset the **triggers counters** by pressing **Reset icon**.

<img align="left" height="300" src=".\Screenshots\Reset.png"><img align="center" height="300" src=".\Screenshots\Reseted.png">

## License

**EveIntelChecker** is open-sourced software licensed under [GNU General Public License v3.0](LICENSE)
