![GitHub all releases](https://img.shields.io/github/downloads/dekirai/xeropresence/total)  
![GitHub release (latest by date)](https://img.shields.io/github/v/release/dekirai/xeropresence)
# Xero Presence
Discord Rich Presence for Xero  
Every 5 seconds it updates your current game state

## Information
- Never ever share your "Secret Access Key"
- If you think someone has access to it, deactivate or reset the key immediately to prevent unauthorized use
- A documentation to customize your presence with a config.json file can be found [here](https://github.com/Dekirai/XeroPresence/blob/main/DOCUMENTATION.md)

## Requirements
- Windows 10/11 64 Bit (Recommended)
- [.NET 6 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- Discord on Windows
- "Display current activity as a status message" enabled in Discord found under "Activity Privacy"

## Tutorial
1. Head over to [xero.gg - API](https://xero.gg/settings/api)
2. Create a new Access Key and name it anything you want
3. Enter the "Access Key ID" and "Secret Access Key" in the textboxes
4. Click "Start Presence"

## Adding more profiles
1. Create a file named "profiles.ini"
2. Open it with a text editor of your choice
3. Copy and paste the template below into it
4. Replace it with your data

```ini
[Account 1]
AccessKeyID=12349a02da2d
SecretAccessKey=awdkapodaw012390
[Account 2]
AccessKeyID=12349a02da2d
SecretAccessKey=awdkapodaw012390
[Account 3]
AccessKeyID=12349a02da2d
SecretAccessKey=awdkapodaw012390```


![image](https://i.imgur.com/iOZdBX3.png)![image](https://i.imgur.com/ttVW6Zh.png)
