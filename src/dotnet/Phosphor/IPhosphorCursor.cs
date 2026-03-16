namespace Enclave.Phosphor;

/// <summary>
/// Moves the physical terminal cursor to an absolute position.
/// </summary>
public interface IPhosphorCursor
{
    /// <summary>
    /// Moves the cursor to column <paramref name="col"/>, row <paramref name="row"/> (both 0-based).
    /// </summary>
    void MoveTo(int col, int row);
}
