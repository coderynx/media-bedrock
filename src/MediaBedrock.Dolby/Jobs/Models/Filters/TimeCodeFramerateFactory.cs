namespace MediaBedrock.Dolby.Jobs.Models.Filters;

public static class TimeCodeFramerateFactory
{
    public static string ToDtoString(this TimeCodeFrameRate timeCodeFrameRate)
    {
        return timeCodeFrameRate switch
        {
            TimeCodeFrameRate.NotIndicated => "not_indicated",
            TimeCodeFrameRate.TwentyThreeNineSevenSix => "23.976",
            TimeCodeFrameRate.TwentyFour => "24",
            TimeCodeFrameRate.TwentyFive => "25",
            TimeCodeFrameRate.TwentyNineNineSeven => "29.97",
            TimeCodeFrameRate.Thirty => "30",
            TimeCodeFrameRate.FortyEight => "48",
            TimeCodeFrameRate.Fifty => "50",
            TimeCodeFrameRate.FiftyNineNineFour => "59.94",
            TimeCodeFrameRate.Sixty => "60",
            _ => throw new ArgumentOutOfRangeException(nameof(timeCodeFrameRate), timeCodeFrameRate, null)
        };
    }
}