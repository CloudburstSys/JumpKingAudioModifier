using Logging.API;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using JumpKingAudioModifier.Helpers;
using JumpKingAudioModifier.Logging;
using JumpKingAudioModifier.Settings;
using Prism.Commands;

namespace JumpKingAudioModifier
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// The voices available for use
        /// </summary>
        public ObservableCollection<string> PresetNames
        {
            get
            {
                return presetNames;
            }
            set
            {
                if (presetNames != value)
                {
                    presetNames = value;
                    RaisePropertyChanged(nameof(PresetNames));
                }
            }
        }
        private ObservableCollection<string> presetNames;

        /// <summary>
        /// The selected voice name
        /// </summary>
        public string SelectedPresetName
        {
            get
            {
                return selectedPresetName;
            }
            set
            {
                if (selectedPresetName != value)
                {
                    selectedPresetName = value;
                    if (presetNameLookup.ContainsKey(selectedPresetName))
                    {
                        presetToUse = presetNameLookup[selectedPresetName];
                    }
                    RaisePropertyChanged(nameof(SelectedPresetName));
                }
            }
        }
        private string selectedPresetName;

        /// <summary>
        /// Whether the preset should affect the bump sound effect
        /// </summary>
        public bool IsAffectingBumpSFX
        {
            get
            {
                return isAffectingBumpSFX;
            }
            set
            {
                if (isAffectingBumpSFX != value)
                {
                    isAffectingBumpSFX = value;
                    RaisePropertyChanged(nameof(IsAffectingBumpSFX));
                }
            }
        }
        private bool isAffectingBumpSFX;
        
        /// <summary>
        /// Whether the preset should affect the jump sound effect
        /// </summary>
        public bool IsAffectingJumpSFX
        {
            get
            {
                return isAffectingJumpSFX;
            }
            set
            {
                if (isAffectingJumpSFX != value)
                {
                    isAffectingJumpSFX = value;
                    RaisePropertyChanged(nameof(IsAffectingJumpSFX));
                }
            }
        }
        private bool isAffectingJumpSFX;
        
        /// <summary>
        /// Whether the preset should affect the land sound effect
        /// </summary>
        public bool IsAffectingLandSFX
        {
            get
            {
                return isAffectingLandSFX;
            }
            set
            {
                if (isAffectingLandSFX != value)
                {
                    isAffectingLandSFX = value;
                    RaisePropertyChanged(nameof(IsAffectingLandSFX));
                }
            }
        }
        private bool isAffectingLandSFX;
        
        /// <summary>
        /// Whether the preset should affect the land sound effect
        /// </summary>
        public bool IsAffectingSplatSFX
        {
            get
            {
                return isAffectingSplatSFX;
            }
            set
            {
                if (isAffectingSplatSFX != value)
                {
                    isAffectingSplatSFX = value;
                    RaisePropertyChanged(nameof(IsAffectingSplatSFX));
                }
            }
        }
        private bool isAffectingSplatSFX;
        
        /// <summary>
        /// Called to randomise the preset.
        /// </summary>
        public ICommand RandomPreset { get; private set; }

        /// <summary>
        /// Called to Start or Stop the TTS system
        /// </summary>
        public ICommand InjectChanges { get; private set; }

        /// <summary>
        /// Called to Edit the Settings
        /// </summary>
        public ICommand EditSettingsCommand { get; private set; }

        private SFXPresetDefinition presetToUse;
        private readonly Dictionary<string, SFXPresetDefinition> presetNameLookup;
        private readonly Settings.Settings Settings;
        private readonly ILogger logger;

        public MainWindowViewModel()
        {
            logger = new FileLogger();
            logger.Information($"Starting Logging at {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}");
            
            // Get settings
            Settings = FileHelper.LoadSettings(logger);
            
            // Get all presets
            List<SFXPresetDefinition> presets = FileHelper.LoadExternalPresets(logger);
            
            // Populate the Voice Names
            PresetNames = new ObservableCollection<string>();
            presetNameLookup = new Dictionary<string, SFXPresetDefinition>();
            for (int i = 0; i < presets.Count; i++)
            {
                PresetNames.Add(presets[i].Name);
                presetNameLookup.Add(presets[i].Name, presets[i]);
            }

            if (PresetNames.Count > 0)
            {
                SelectedPresetName = PresetNames[0];
                presetToUse = presetNameLookup[PresetNames[0]];
            }

            SetupCommands();
        }

        /// <summary>
        /// Sets up all the <see cref="ICommand"/> instances
        /// </summary>
        private void SetupCommands()
        {
            RandomPreset = new DelegateCommand(RandomisePreset);
            
            InjectChanges = new DelegateCommand(Inject);

            EditSettingsCommand = new DelegateCommand(OpenEditSettingsWindow);
        }

        /// <summary>
        /// Opens the window to Edit Settings
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void OpenEditSettingsWindow()
        {
            throw new NotImplementedException();
        }

        private void RandomisePreset()
        {
            var r = new Random();

            var mainPresetNames = presetNames.Where(preset => preset != "Randomized Sounds").ToList();
            
            int index = r.Next(0, mainPresetNames.Count);

            SelectedPresetName = mainPresetNames[index];
            presetToUse = presetNameLookup[mainPresetNames[index]];
        }

        /// <summary>
        /// Toggles the TTS Speaker
        /// </summary>
        private void Inject()
        {
            var sourceDir = presetToUse.Directory;
            var destinationDir = $@"{Settings.JumpKingFolder}\Content\audio\jump_king";
            
            // TODO: make more efficient.
            try
            {
                if (presetToUse.Name == "Randomized Sounds")
                {
                    // Unique behaviour: just go through every allowed SFX and give it a random sound from sourceDir.
                    if (IsAffectingBumpSFX)
                    {
                        FileHelper.DuplicateFile($@"{FileHelper.GetRandomFilePath(sourceDir)}",
                            $@"{destinationDir}\king_bump.xnb");
                        FileHelper.DuplicateFile($@"{FileHelper.GetRandomFilePath(sourceDir)}",
                            $@"{destinationDir}\water\water_bump.xnb");
                    }

                    if (IsAffectingJumpSFX)
                    {
                        FileHelper.DuplicateFile($@"{FileHelper.GetRandomFilePath(sourceDir)}",
                            $@"{destinationDir}\king_jump.xnb");
                        FileHelper.DuplicateFile($@"{FileHelper.GetRandomFilePath(sourceDir)}",
                            $@"{destinationDir}\ice\king_jump.xnb");
                        FileHelper.DuplicateFile($@"{FileHelper.GetRandomFilePath(sourceDir)}",
                            $@"{destinationDir}\snow\king_jump.xnb");
                        FileHelper.DuplicateFile($@"{FileHelper.GetRandomFilePath(sourceDir)}",
                            $@"{destinationDir}\water\water_jump.xnb");
                    }

                    if (IsAffectingLandSFX)
                    {
                        FileHelper.DuplicateFile($@"{FileHelper.GetRandomFilePath(sourceDir)}",
                            $@"{destinationDir}\king_land.xnb");
                        FileHelper.DuplicateFile($@"{FileHelper.GetRandomFilePath(sourceDir)}",
                            $@"{destinationDir}\ice\king_land.xnb");
                        FileHelper.DuplicateFile($@"{FileHelper.GetRandomFilePath(sourceDir)}",
                            $@"{destinationDir}\sand\sand_land.xnb");
                        FileHelper.DuplicateFile($@"{FileHelper.GetRandomFilePath(sourceDir)}",
                            $@"{destinationDir}\shoes_iron\iron_land.xnb");
                        FileHelper.DuplicateFile($@"{FileHelper.GetRandomFilePath(sourceDir)}",
                            $@"{destinationDir}\snow\king_land.xnb");
                        FileHelper.DuplicateFile($@"{FileHelper.GetRandomFilePath(sourceDir)}",
                            $@"{destinationDir}\water\water_land.xnb");
                    }

                    if (IsAffectingSplatSFX)
                    {
                        FileHelper.DuplicateFile($@"{FileHelper.GetRandomFilePath(sourceDir)}",
                            $@"{destinationDir}\king_splat.xnb");
                        FileHelper.DuplicateFile($@"{FileHelper.GetRandomFilePath(sourceDir)}",
                            $@"{destinationDir}\shoes_iron\iron_splat.xnb");
                        FileHelper.DuplicateFile($@"{FileHelper.GetRandomFilePath(sourceDir)}",
                            $@"{destinationDir}\snow\king_splat.xnb");
                        FileHelper.DuplicateFile($@"{FileHelper.GetRandomFilePath(sourceDir)}",
                            $@"{destinationDir}\water\water_splat.xnb");
                    }
                    
                    MessageBox.Show(
                        $"Successfully randomised audio files. Restart Jump King for it to take effect!");
                }
                else
                {

                    if (presetToUse.Files.KingBump.Normal != null && IsAffectingBumpSFX)
                        FileHelper.DuplicateFile($@"{sourceDir}\{presetToUse.Files.KingBump.Normal}.xnb",
                            $@"{destinationDir}\king_bump.xnb");
                    if (presetToUse.Files.KingBump.Water != null && IsAffectingBumpSFX)
                        FileHelper.DuplicateFile($@"{sourceDir}\{presetToUse.Files.KingBump.Water}.xnb",
                            $@"{destinationDir}\water\water_bump.xnb");
                    if (presetToUse.Files.KingJump.Normal != null && IsAffectingJumpSFX)
                        FileHelper.DuplicateFile($@"{sourceDir}\{presetToUse.Files.KingJump.Normal}.xnb",
                            $@"{destinationDir}\king_jump.xnb");
                    if (presetToUse.Files.KingJump.Ice != null && IsAffectingJumpSFX)
                        FileHelper.DuplicateFile($@"{sourceDir}\{presetToUse.Files.KingJump.Ice}.xnb",
                            $@"{destinationDir}\ice\king_jump.xnb");
                    if (presetToUse.Files.KingJump.Snow != null && IsAffectingJumpSFX)
                        FileHelper.DuplicateFile($@"{sourceDir}\{presetToUse.Files.KingJump.Snow}.xnb",
                            $@"{destinationDir}\snow\king_jump.xnb");
                    if (presetToUse.Files.KingJump.Water != null && IsAffectingJumpSFX)
                        FileHelper.DuplicateFile($@"{sourceDir}\{presetToUse.Files.KingJump.Water}.xnb",
                            $@"{destinationDir}\water\water_jump.xnb");
                    if (presetToUse.Files.KingLand.Normal != null && IsAffectingLandSFX)
                        FileHelper.DuplicateFile($@"{sourceDir}\{presetToUse.Files.KingLand.Normal}.xnb",
                            $@"{destinationDir}\king_land.xnb");
                    if (presetToUse.Files.KingLand.Ice != null && IsAffectingLandSFX)
                        FileHelper.DuplicateFile($@"{sourceDir}\{presetToUse.Files.KingLand.Ice}.xnb",
                            $@"{destinationDir}\ice\king_land.xnb");
                    if (presetToUse.Files.KingLand.Sand != null && IsAffectingLandSFX)
                        FileHelper.DuplicateFile($@"{sourceDir}\{presetToUse.Files.KingLand.Sand}.xnb",
                            $@"{destinationDir}\sand\sand_land.xnb");
                    if (presetToUse.Files.KingLand.ShoesIron != null && IsAffectingLandSFX)
                        FileHelper.DuplicateFile($@"{sourceDir}\{presetToUse.Files.KingLand.ShoesIron}.xnb",
                            $@"{destinationDir}\shoes_iron\iron_land.xnb");
                    if (presetToUse.Files.KingLand.Snow != null && IsAffectingLandSFX)
                        FileHelper.DuplicateFile($@"{sourceDir}\{presetToUse.Files.KingLand.Snow}.xnb",
                            $@"{destinationDir}\snow\king_land.xnb");
                    if (presetToUse.Files.KingLand.Water != null && IsAffectingLandSFX)
                        FileHelper.DuplicateFile($@"{sourceDir}\{presetToUse.Files.KingLand.Water}.xnb",
                            $@"{destinationDir}\water\water_land.xnb");
                    if (presetToUse.Files.KingSplat.Normal != null && IsAffectingSplatSFX)
                        FileHelper.DuplicateFile($@"{sourceDir}\{presetToUse.Files.KingSplat.Normal}.xnb",
                            $@"{destinationDir}\king_splat.xnb");
                    if (presetToUse.Files.KingSplat.ShoesIron != null && IsAffectingSplatSFX)
                        FileHelper.DuplicateFile($@"{sourceDir}\{presetToUse.Files.KingSplat.ShoesIron}.xnb",
                            $@"{destinationDir}\shoes_iron\iron_splat.xnb");
                    if (presetToUse.Files.KingSplat.Snow != null && IsAffectingSplatSFX)
                        FileHelper.DuplicateFile($@"{sourceDir}\{presetToUse.Files.KingSplat.Snow}.xnb",
                            $@"{destinationDir}\snow\king_splat.xnb");
                    if (presetToUse.Files.KingSplat.Water != null && IsAffectingSplatSFX)
                        FileHelper.DuplicateFile($@"{sourceDir}\{presetToUse.Files.KingSplat.Water}.xnb",
                            $@"{destinationDir}\water\water_splat.xnb");

                    MessageBox.Show(
                        $"Successfully loaded the {presetToUse.Name} preset. Restart Jump King for it to take effect!");
                }
            }
            catch (FileNotFoundException _)
            {
                MessageBox.Show($"Failed to load the {presetToUse.Name} preset. Do the files requested exist?",
                    "Failure!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Invokes the <see cref="PropertyChanged"/> event
        /// </summary>
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
