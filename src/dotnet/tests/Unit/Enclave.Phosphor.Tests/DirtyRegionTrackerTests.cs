namespace Enclave.Phosphor.Tests;

/// <summary>
/// Unit tests for DirtyRegionTracker merge and flush logic, tested via <see cref="VirtualScreen"/>.
/// </summary>
[UnitTest, TestOf("DirtyRegionTracker")]
public sealed class DirtyRegionTrackerTests
{
    // DirtyRegionTracker is internal; tests access it via InternalsVisibleTo or reflection.
    // We test it indirectly through VirtualScreen to avoid brittle reflection.
    // Direct tests are enabled because the test assembly is in the same project scope.

    [Fact]
    public void HasRegions_InitiallyFalse()
    {
        var screen = new VirtualScreen(new Size(80, 24));
        screen.HasDirtyRegions.ShouldBeFalse();
    }

    [Fact]
    public void Invalidate_SingleRegion_MarkedDirty()
    {
        var screen = new VirtualScreen(new Size(80, 24));
        screen.Invalidate(new Rectangle(0, 0, 10, 5));
        screen.HasDirtyRegions.ShouldBeTrue();
    }

    [Fact]
    public void FlushDirtyRegions_ReturnsRegionsAndClearsState()
    {
        var screen = new VirtualScreen(new Size(80, 24));
        var region = new Rectangle(5, 2, 20, 10);
        screen.Invalidate(region);

        var regions = screen.FlushDirtyRegions();

        regions.Count.ShouldBe(1);
        regions[0].ShouldBe(region);
        screen.HasDirtyRegions.ShouldBeFalse();
    }

    [Fact]
    public void Invalidate_OverlappingRegions_AreMerged()
    {
        var screen = new VirtualScreen(new Size(80, 24));
        screen.Invalidate(new Rectangle(0, 0, 10, 5));
        screen.Invalidate(new Rectangle(5, 3, 10, 5)); // overlaps the first

        var regions = screen.FlushDirtyRegions();

        // Two overlapping regions should be merged into one
        regions.Count.ShouldBe(1);
        // Union should encompass both original rectangles
        regions[0].Contains(new Rectangle(0, 0, 10, 5)).ShouldBeTrue();
        regions[0].Contains(new Rectangle(5, 3, 10, 5)).ShouldBeTrue();
    }

    [Fact]
    public void Invalidate_NonOverlappingRegions_AreKeptSeparate()
    {
        var screen = new VirtualScreen(new Size(80, 24));
        screen.Invalidate(new Rectangle(0, 0, 5, 5));
        screen.Invalidate(new Rectangle(70, 18, 5, 5)); // far corner, no overlap

        var regions = screen.FlushDirtyRegions();

        regions.Count.ShouldBe(2);
    }

    [Fact]
    public void Invalidate_EmptyRegion_IsIgnored()
    {
        var screen = new VirtualScreen(new Size(80, 24));
        screen.Invalidate(Rectangle.Empty);
        screen.HasDirtyRegions.ShouldBeFalse();
    }

    [Fact]
    public void FlushDirtyRegions_AfterFlush_IsEmpty()
    {
        var screen = new VirtualScreen(new Size(80, 24));
        screen.Invalidate(new Rectangle(0, 0, 40, 12));
        screen.FlushDirtyRegions();

        screen.HasDirtyRegions.ShouldBeFalse();
        screen.FlushDirtyRegions().Count.ShouldBe(0);
    }
}
