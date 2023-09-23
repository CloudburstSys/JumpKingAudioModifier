namespace JumpKingAudioModifier.Settings;

/// <summary>
/// A definition file for presets.
/// Defines the preset name and the file names.
/// </summary>
public class SFXPresetDefinition
{
    public SFXPresetDefinition()
    {
        Name = "";
        Directory = ""; // This is actually set automatically and just serves as a way to "remember" where this SFX stores it's assets.
        Files = new SFXPresetFiles();
    }

    public SFXPresetDefinition(string name, string directory)
    {
        Name = name;
        Directory = directory;
    }
    
    public string Name { get; set; }
    public string Directory { get; set; }
    public SFXPresetFiles Files { get; set; }
}

public class SFXPresetFiles
{
    public SFXPresetFiles()
    {
        KingBump = new SFXPresetBumpFiles();
        KingJump = new SFXPresetJumpFiles();
        KingLand = new SFXPresetLandFiles();
        KingSplat = new SFXPresetSplatFiles();
    }

    public SFXPresetBumpFiles KingBump { get; set; }
    public SFXPresetJumpFiles KingJump { get; set; }
    public SFXPresetLandFiles KingLand { get; set; }
    public SFXPresetSplatFiles KingSplat { get; set; }
}

public class SFXPresetBumpFiles
{
    public string? Normal { get; set; } = null;
    public string? Water { get; set; } = null;
}

public class SFXPresetJumpFiles
{
    public string? Normal { get; set; } = null;
    public string? Ice { get; set; } = null;
    public string? Snow { get; set; } = null;
    public string? Water { get; set; } = null;
}

public class SFXPresetLandFiles
{
    public string? Normal { get; set; } = null;
    public string? Ice { get; set; } = null;
    public string? Sand { get; set; } = null;
    public string? ShoesIron { get; set; } = null;
    public string? Snow { get; set; } = null;
    public string? Water { get; set; } = null;
}

public class SFXPresetSplatFiles
{
    public string? Normal { get; set; } = null;
    public string? ShoesIron { get; set; } = null;
    public string? Snow { get; set; } = null;
    public string? Water { get; set; } = null;
}