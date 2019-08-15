# Craiel Unity Audio

Audio System using Craiel GameData with support for Localized and Attached Emitters, Audio Source Pooling, UI Audio Events, Fading and Multiple Clips in sequence or random.

## Getting Started

Add the package and dependencies to your Project Manifest.json:
```
{
    "dependencies": {
    ...
    "com.craiel.unity.essentials": "https://github.com/Craiel/UnityEssentials.git",
    "com.craiel.unity.gamedata": "https://github.com/Craiel/UnityGameData.git",
    "com.craiel.unity.audio": "https://github.com/Craiel/UnityAudio.git",
    ...
  }
}
```


### Prerequisites
 
- https://github.com/Craiel/UnityEssentials
- https://github.com/Craiel/UnityGameData


### Usage

Register in the IGameDataEditorConfig script:

```
GameDataEditorWindow.AddContent<GameDataAudio>("Audio");
```

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.


## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
