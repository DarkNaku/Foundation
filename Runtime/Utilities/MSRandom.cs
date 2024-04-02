using System;
using UnityEngine;

public class MSRandom {
    public long Seed { get; set; }

    public MSRandom() {
        DateTime now = DateTime.Now.ToLocalTime();
        DateTime StartOfEpoch = new DateTime(1970, 1, 1).ToLocalTime();
        Seed = (long)((now - StartOfEpoch).TotalSeconds);
    }

    public MSRandom(long seed) {
        Seed = seed;
    }

    public int Next() {
        return (int)RandomTable() % Int32.MaxValue;
    }

    public int Next(int max) {
        return (int)RandomTable() % max;
    }

    public int Next(int min, int max) {
        Debug.Assert(min < max, "[MSRandom] Next : Max value must be larget than the Min value.");
        return ((int)RandomTable() % (max - min)) + min;
    }

    private long RandomTable() {
        Seed = Seed * 0x343fd + 0x269EC3; // a=214013, b=2531011
        return (Seed >> 0x10) & 0x7FFF;
    }
}