using Flare.Components.Scheduler;

namespace Flare.Components.Scheduler.Tests;

public sealed class SchedulerSlotsTests
{
    [Theory]
    [InlineData(2026, 3, 8, 46)]
    [InlineData(2026, 11, 1, 50)]
    public void SlotsFollowRealDaylightSavingIntervals(int year, int month, int day, int count)
    {
        var zone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
        var slots = SchedulerSlots.ForDate(new(year, month, day), zone, TimeSpan.FromMinutes(30));
        Assert.Equal(count, slots.Count);
        for (var i = 1; i < slots.Count; i++) Assert.Equal(slots[i - 1].End, slots[i].Start);
        Assert.All(slots, s => Assert.False(zone.IsInvalidTime(s.Start.DateTime)));
        if (month == 11)
        {
            var repeated = slots.Where(s => s.Start.Hour == 1 && s.Start.Minute == 0).ToArray();
            Assert.Equal(2, repeated.Length);
            Assert.NotEqual(repeated[0].Start.Offset, repeated[1].Start.Offset);
        }
    }
}
