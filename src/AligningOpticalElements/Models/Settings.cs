using OpenCvSharp;

/// <summary>
/// Třída obsahující konfigurační parametry pro zpracování obrazu
/// a morfologické operace.
/// </summary>
public class Settings
{
    /// <summary>
    /// Maximální povolená odchylka pozice bodu v ose Y.
    /// </summary>
    public float DevSpotY { get; set; }

    /// <summary>
    /// Hodnota clip limitu používaná při CLAHE kontrastní úpravě obrazu.
    /// </summary>
    public int ClipLimit { get; set; }

    /// <summary>
    /// Velikost dlaždic používaných při CLAHE filtraci.
    /// </summary>
    public int TileGridSize { get; set; }

    /// <summary>
    /// Velikost kernelu mediánového filtru.
    /// </summary>
    public int MedianKernel { get; set; }

    /// <summary>
    /// Minimální procentuální počet bílých pixelů po prahování.
    /// </summary>
    public float MinimumPixels { get; set; }

    /// <summary>
    /// Maximální povolený rozdíl počtu pixelů při dodatečné úpravě prahu.
    /// </summary>
    public float MaxPixelsDiff { get; set; }

    /// <summary>
    /// Velikost kernelu pro morfologické uzavření.
    /// </summary>
    public int CloseMorphKernel { get; set; }

    /// <summary>
    /// Velikost kernelu pro morfologické otevření.
    /// </summary>
    public int OpenMorphKernel { get; set; }

    /// <summary>
    /// Typ masky používané při morfologickém uzavření.
    /// </summary>
    public MorphShapes MorphCloseShape { get; set; }

    /// <summary>
    /// Typ masky používané při morfologickém otevření.
    /// </summary>
    public MorphShapes MorphOpenShape { get; set; }

    /// <summary>
    /// Inicializuje novou instanci třídy <see cref="Settings"/>.
    /// </summary>
    /// <param name="devSpotY">Maximální povolená odchylka pozice bodu v ose Y.</param>
    /// <param name="clipLimit">Hodnota clip limitu pro CLAHE.</param>
    /// <param name="tileGridSize">Velikost dlaždic CLAHE filtru.</param>
    /// <param name="medianKernel">Velikost kernelu mediánového filtru.</param>
    /// <param name="minimumPixels">Minimální procentuální počet bílých pixelů.</param>
    /// <param name="maxPixelsDiff">Maximální rozdíl počtu pixelů při úpravě prahu.</param>
    /// <param name="closeMorphKernel">Velikost kernelu pro morfologické uzavření.</param>
    /// <param name="openMorphKernel">Velikost kernelu pro morfologické otevření.</param>
    /// <param name="morphCloseShape">Typ masky pro morfologické uzavření.</param>
    /// <param name="morphOpenShape">Typ masky pro morfologické otevření.</param>
    public Settings(
        float devSpotY,
        int clipLimit,
        int tileGridSize,
        int medianKernel,
        float minimumPixels,
        float maxPixelsDiff,
        int closeMorphKernel,
        int openMorphKernel,
        MorphShapes morphCloseShape,
        MorphShapes morphOpenShape)
    {
        DevSpotY = devSpotY;
        ClipLimit = clipLimit;
        TileGridSize = tileGridSize;
        MedianKernel = medianKernel;
        MinimumPixels = minimumPixels;
        MaxPixelsDiff = maxPixelsDiff;
        CloseMorphKernel = closeMorphKernel;
        OpenMorphKernel = openMorphKernel;
        MorphCloseShape = morphCloseShape;
        MorphOpenShape = morphOpenShape;
    }
}