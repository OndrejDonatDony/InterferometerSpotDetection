
namespace AligningOpticalElements;

/// <summary>
/// Třída reprezentující detekovaný optický bod v obraze.
/// </summary>
public class Spot
{
    private int coordX;
    private int coordY;
    private float coordZ;
    private int radius;

    /// <summary>
    /// Inicializuje nový objekt optického bodu.
    /// </summary>
    /// <param name="coordX">Souřadnice bodu v ose X.</param>
    /// <param name="coordY">Souřadnice bodu v ose Y.</param>
    /// <param name="radius">Poloměr detekovaného bodu.</param>
    /// <param name="coordZ">Souřadnice bodu v ose Z.</param>
    public Spot(int coordX, int coordY, int radius, float coordZ)
    {
        this.coordX = coordX;
        this.coordY = coordY;
        this.radius = radius;
        this.coordZ = coordZ;
    }

    /// <summary>
    /// Vrací souřadnici bodu v ose X.
    /// </summary>
    public int GetCoordX { get { return coordX; } }

    /// <summary>
    /// Vrací souřadnici bodu v ose Y.
    /// </summary>
    public int GetCoordY { get { return coordY; } }

    /// <summary>
    /// Vrací souřadnici bodu v ose Z.
    /// </summary>
    public float GetCoordZ { get { return coordZ; } }

    /// <summary>
    /// Vrací poloměr detekovaného bodu.
    /// </summary>
    public int GetRadius { get { return radius; } }

    /// <summary>
    /// Vytvoří kopii objektu bodu.
    /// </summary>
    /// <returns>Kopie objektu Spot.</returns>
    public Spot Clone()
    {
        return new Spot(GetCoordX, GetCoordY, GetRadius, GetCoordZ);
    }

}