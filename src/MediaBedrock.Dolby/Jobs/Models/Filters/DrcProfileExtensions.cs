namespace MediaBedrock.Dolby.Jobs.Models.Filters;

public static class DrcProfileExtensions
{
    public static string ToDtoString(this DrcProfile profile)
    {
        return profile switch
        {
            DrcProfile.None => "none",
            DrcProfile.FilmStandard => "film_standard",
            DrcProfile.FilmLight => "film_light",
            DrcProfile.MusicStandard => "music_standard",
            DrcProfile.MusicLight => "music_light",
            DrcProfile.Speech => "speech",
            _ => throw new ArgumentOutOfRangeException(nameof(profile), profile, null)
        };
    }
}