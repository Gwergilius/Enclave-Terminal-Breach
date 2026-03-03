namespace Enclave.Phosphor.Tests;

/// <summary>
/// Unit tests for <see cref="VirtualScreen"/> — layer management and region queries.
/// Dirty-region behaviour is covered separately in <see cref="DirtyRegionTrackerTests"/>.
/// </summary>
[UnitTest, TestOf(nameof(VirtualScreen))]
public sealed class VirtualScreenTests
{
    // --- Size ---------------------------------------------------------------

    [Fact]
    public void Size_ReturnsValuePassedToConstructor()
    {
        var screen = new VirtualScreen(new Size(80, 24));
        screen.Size.ShouldBe(new Size(80, 24));
    }

    // --- AddLayer -----------------------------------------------------------

    [Fact]
    public void AddLayer_ReturnsLayerWithCorrectBoundsAndZOrder()
    {
        var screen = new VirtualScreen(new Size(80, 24));
        var bounds = new Rectangle(5, 2, 20, 10);

        var layer = screen.AddLayer(bounds, zOrder: 42);

        layer.Bounds.ShouldBe(bounds);
        layer.ZOrder.ShouldBe(42);
    }

    [Fact]
    public void AddLayer_LayerAppearsInGetLayersInRegion()
    {
        var screen = new VirtualScreen(new Size(80, 24));
        var layer  = screen.AddLayer(new Rectangle(0, 0, 10, 5), zOrder: 0);

        var result = screen.GetLayersInRegion(new Rectangle(0, 0, 10, 5));

        result.ShouldContain(layer);
    }

    // --- RemoveLayer --------------------------------------------------------

    [Fact]
    public void RemoveLayer_LayerNoLongerReturnedByGetLayersInRegion()
    {
        var screen = new VirtualScreen(new Size(80, 24));
        var layer  = screen.AddLayer(new Rectangle(0, 0, 10, 5), zOrder: 0);

        screen.RemoveLayer(layer);

        screen.GetLayersInRegion(new Rectangle(0, 0, 10, 5)).ShouldBeEmpty();
    }

    [Fact]
    public void RemoveLayer_InvalidatesVacatedBounds()
    {
        var screen  = new VirtualScreen(new Size(80, 24));
        var bounds  = new Rectangle(5, 2, 20, 10);
        var layer   = screen.AddLayer(bounds, zOrder: 0);
        screen.FlushDirtyRegions(); // clear any regions from AddLayer (none expected, but defensive)

        screen.RemoveLayer(layer);

        screen.HasDirtyRegions.ShouldBeTrue();
        var dirty = screen.FlushDirtyRegions();
        dirty.ShouldContain(r => r.Contains(bounds));
    }

    // --- GetLayersInRegion --------------------------------------------------

    [Fact]
    public void GetLayersInRegion_NoLayers_ReturnsEmpty()
    {
        var screen = new VirtualScreen(new Size(80, 24));

        screen.GetLayersInRegion(new Rectangle(0, 0, 80, 24)).ShouldBeEmpty();
    }

    [Fact]
    public void GetLayersInRegion_NonIntersectingLayer_Excluded()
    {
        var screen = new VirtualScreen(new Size(80, 24));
        screen.AddLayer(new Rectangle(60, 18, 10, 5), zOrder: 0); // far corner

        var result = screen.GetLayersInRegion(new Rectangle(0, 0, 10, 5));

        result.ShouldBeEmpty();
    }

    [Fact]
    public void GetLayersInRegion_MultipleIntersecting_ReturnedOrderedByZOrder()
    {
        var screen  = new VirtualScreen(new Size(80, 24));
        var region  = new Rectangle(0, 0, 10, 5);
        var layerLo = screen.AddLayer(region, zOrder: 1);
        var layerHi = screen.AddLayer(region, zOrder: 9);
        var layerMd = screen.AddLayer(region, zOrder: 5);

        var result = screen.GetLayersInRegion(region).ToList();

        result.ShouldBe([layerLo, layerMd, layerHi]);
    }

    [Fact]
    public void GetLayersInRegion_OnlyIntersectingLayersReturned()
    {
        var screen     = new VirtualScreen(new Size(80, 24));
        var inside     = screen.AddLayer(new Rectangle(0,  0, 10,  5), zOrder: 0);
        var outside    = screen.AddLayer(new Rectangle(70, 18, 5,  3), zOrder: 1);
        var overlapping = screen.AddLayer(new Rectangle(8,  3,  5,  5), zOrder: 2);

        var result = screen.GetLayersInRegion(new Rectangle(0, 0, 10, 5));

        result.ShouldContain(inside);
        result.ShouldContain(overlapping);
        result.ShouldNotContain(outside);
    }
}
