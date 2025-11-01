using System.Runtime.CompilerServices;

namespace KbinXml.Net.Internal;

internal ref struct DataPositionTracker
{
    public int Pos32;
    public int Pos16;
    public int Pos8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Align32()
    {
        AlignTo4Bytes(ref Pos32);
        Realign16_8();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Realign16_8()
    {
        if ((Pos8 & 3) == 0) Pos8 = Pos32;
        if ((Pos16 & 3) == 0) Pos16 = Pos32;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AlignTo4Bytes(ref int pointer)
    {
        pointer = (pointer + 3) & ~3;
    }
}