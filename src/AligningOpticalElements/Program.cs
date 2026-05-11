using AligningOpticalElements;
using OpenCvSharp;

/// <summary>
/// Hlavní třída programu zajišťující načtení obrazů a vizualizaci detekovaných bodů.
/// </summary>
class Program
{

    /// <summary>
    /// Hlavní vstupní bod aplikace.
    /// </summary>
    static void Main()
    {
        OpticalElementsAligner aligner = new OpticalElementsAligner();
        //R585 v1 + R52 x+2 y+2
        //Zygo flat 4_ + flat
        string rootPath = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            @"..\..\..\data\Align camera data"));

        foreach (string file in Directory.GetFiles(rootPath, "*.bmp", SearchOption.AllDirectories))
        {
            Console.WriteLine(file);
            Console.WriteLine(Path.GetFileName(file));

            using Mat image = Cv2.ImRead(file);

            aligner.LoadSpots(image);
            ShowSpotsOnImage(image, aligner.spots);
        }
    }

    /// <summary>
    /// Zobrazí detekované body vykreslené nad vstupním obrazem.
    /// </summary>
    /// <param name="image">Vstupní obraz.</param>
    /// <param name="spots">Seznam detekovaných bodů.</param>
    public static void ShowSpotsOnImage(Mat image, List<Spot> spots)
    {
        if (image == null || image.Empty())
        {
            Console.WriteLine("ShowImage: img je null nebo empty");
            return;
        }
        using var small = new Mat();
        Cv2.Resize(image, small, new Size(), 0.5, 0.5);

        foreach (var spot in spots)
        {
            int x = (int)(spot.GetCoordX * 0.5);
            int y = (int)(spot.GetCoordY * 0.5);

            int radius = (int)(spot.GetRadius * 0.5);
            int thickness = 1;
            Cv2.Circle(
                small,
                new Point(x, y),
                radius,
                Scalar.Red,   
                thickness             
            );
        }
        Cv2.ImShow("Reference", small);
        Cv2.WaitKey();
    }
}
