namespace Enclave.Phosphor;

/// <summary>
/// A renderable UI component that projects its current state into a <see cref="LayerWriter"/>.
/// </summary>
/// <remarks>
/// Components are pure renderers: they read state and write cells. They must not perform
/// async I/O, Task.Delay, or navigation — those belong to the ViewModel.
/// </remarks>
public interface IComponent
{
    /// <summary>Component bounds relative to its parent (or absolute for root components).</summary>
    Rectangle Bounds { get; }

    /// <summary>Renders the current state of this component into <paramref name="writer"/>.</summary>
    void Render(LayerWriter writer);
}
