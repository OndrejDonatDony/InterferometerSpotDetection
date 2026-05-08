using OpenCvSharp;
using System.Security.Claims;

namespace AligningOpticalElements;

/// <summary>
/// Třída zajišťující detekci a zpracování optických bodů v obraze.
/// </summary>
public class OpticalElementsAligner
{
    /// <summary>
    /// Seznam detekovaných bodů v obraze.
    /// </summary>
    public List<Spot> spots = new List<Spot>();

    /// <summary>
    /// Načte a detekuje optické body ze vstupního obrazu.
    /// </summary>
    /// <param name="img">Vstupní obraz.</param>
    public void LoadSpots(Mat img)
    {
        List<Spot> sp = new List<Spot>();

        if (img == null || img.Empty())
        {
            Console.WriteLine("LoadSpots: img je null nebo empty");
            return;
        }
        Mat bin = ProcessingImg(img);

        Cv2.FindContours(
            bin,
            out Point[][] contours,
            out HierarchyIndex[] hierarchy,
            RetrievalModes.External,
            ContourApproximationModes.ApproxSimple
        );
        int foundCount = 0;
        int devSpotY = (int)(bin.Height * bin.Width / 100 * 0.0008);
        for (int i = 0; i < contours.Length; i++)
        {
            
            double area = Cv2.ContourArea(contours[i]);

            if (area < 5) continue;

            int minY = int.MaxValue;
            int maxY = int.MinValue;

            for (int a = 0; a < contours[i].Length; a++)
            {
                int y = contours[i][a].Y;
                if (y < minY) minY = y;
                if (y > maxY) maxY = y;
            }

            int minX = contours[i].Min(p => p.X);
            int maxX = contours[i].Max(p => p.X);

            int diameterX = maxX - minX; 
            int diameterY = maxY - minY;

            Moments mom = Cv2.Moments(contours[i]);

            if (Math.Abs(mom.M00) < 1e-9) continue;
            int xImg = (int)Math.Round(mom.M10 / mom.M00);
            int yImg = (int)Math.Round(mom.M01 / mom.M00);
            if (diameterY > diameterX+ devSpotY)
            {
               xImg = xImg-diameterY/diameterX*20;
               diameterY = 30 * diameterY/diameterX;
            }
            int radiusPx = (int)Math.Round(diameterY / 2.0);

            if (radiusPx >= 2)
            {
                foundCount++;
                sp.Add(new Spot(xImg, yImg, radiusPx, 0f));
            }
        }
        bin.Dispose();
        spots = sp;
        foreach (var spot in sp)
        {
            Console.WriteLine("X = " + spot.GetCoordX);
            Console.WriteLine("Y = " + spot.GetCoordY);
            Console.WriteLine("poloměr = " + spot.GetRadius);
        }     
    }

    /// <summary>
    /// Provede kompletní předzpracování obrazu pro detekci bodů.
    /// </summary>
    /// <param name="img">Vstupní obraz.</param>
    /// <returns>Vyčištěný binární obraz připravený pro detekci kontur.</returns>
    private Mat ProcessingImg(Mat img)
    {
        bool show = true;

        Mat grayImg = ToGray(img);
        Mat enhancedImg = EnhanceImage(grayImg);
        Mat thresholdImg = Threshold(enhancedImg);
        Mat morphImg = MorphClosing(thresholdImg);
        Mat cleanImg = removeDots(morphImg);

        if (show)
        {
            //Console.WriteLine("gray");
            //ShowImage(grayImg);
            //Console.WriteLine("contrast");
            ////ShowImage(enhancedImg);
            //Console.WriteLine("threshold");
            ////ShowImage(thresholdImg);
            //Console.WriteLine("closing");
            ////ShowImage(morphImg);
            //Console.WriteLine("clean");
            ////ShowImage(cleanImg);
        }

        grayImg.Dispose();
        enhancedImg.Dispose();
        thresholdImg.Dispose();
        morphImg.Dispose();

        return cleanImg;
    }

    /// <summary>
    /// Převede vstupní obraz do odstínů šedi.
    /// </summary>
    /// <param name="img">Vstupní obraz.</param>
    /// <returns>Jednokanálový šedotónový obraz.</returns>
    private Mat ToGray(Mat img)
    {
        if (img.Channels() == 1)
        {
            return img.Clone();
        }
        Mat g = new();
        Cv2.CvtColor(img, g, ColorConversionCodes.BGR2GRAY);
        return g;
    }

    /// <summary>
    /// Zvýší kontrast obrazu a redukuje šum.
    /// </summary>
    /// <param name="img">Vstupní obraz.</param>
    /// <returns>Upravený obraz po aplikaci CLAHE a mediánového filtru.</returns>
    private Mat EnhanceImage(Mat img)
    {
        Mat claheImg = new Mat();
        var clahe = Cv2.CreateCLAHE(
            clipLimit: 20.0,
            tileGridSize: new Size(16, 16)
        );

        clahe.Apply(img, claheImg);
        clahe.Dispose();
        Mat medianImg = new Mat();
        Cv2.MedianBlur(claheImg, medianImg, 3);
        claheImg.Dispose();

        return medianImg;
    }

    /// <summary>
    /// Provede binární prahování obrazu.
    /// </summary>
    /// <param name="img">Vstupní obraz.</param>
    /// <returns>Binární obraz.</returns>
    private Mat Threshold(Mat img)
    {
        Mat bw = new Mat();
        int minimumPixels = (int)(img.Height * img.Width / 100 * 0.05);

        int whiteCount = 0;
        int k = 2;
        int th = 127;

        for (int i = 0; i < 8; i++)
        {
            Cv2.Threshold(img, bw, th, 255, ThresholdTypes.Binary);

            double thCompute = 127.5 / k;
            whiteCount = Cv2.CountNonZero(bw);

            if (whiteCount < minimumPixels)
            {
                th = th - (int)thCompute;
            }
            else
            {
                th = th + (int)thCompute;
            }
            k = k * 2;
        }

        int whiteCountPost = 0;
        int maxPixelsDiff = (int)(img.Height * img.Width / 100 * 0.01); 
        for (int i = 0; i < 50; i++)
        {
            th -= 1;
            Cv2.Threshold(img, bw, th, 255, ThresholdTypes.Binary);
            whiteCountPost = Cv2.CountNonZero(bw);

            if (whiteCountPost - whiteCount > maxPixelsDiff)
            {
                th += 1;
                Cv2.Threshold(img, bw, th, 255, ThresholdTypes.Binary);
                break;
            }
            else
            {
                whiteCount = whiteCountPost;
            }
        }
        return bw;
    }

    /// <summary>
    /// Provede morfologickou operaci uzavření.
    /// </summary>
    /// <param name="img">Binární obraz.</param>
    /// <returns>Obraz po morfologickém uzavření.</returns>
    private Mat MorphClosing(Mat img)
    {
        Mat imgClosing = new Mat();
        Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(5, 5));
        Cv2.MorphologyEx(img, imgClosing, MorphTypes.Close, kernel);
        kernel.Dispose();
        return imgClosing;
    }

    /// <summary>
    /// Odstraní malé objekty a šum z obrazu.
    /// </summary>
    /// <param name="img">Binární obraz.</param>
    /// <returns>Vyčištěný obraz bez malých artefaktů.</returns>
    private Mat removeDots(Mat img)
    {
        Mat labels = new Mat();
        Mat stats = new Mat();
        Mat centroids = new Mat();

        int n = Cv2.ConnectedComponentsWithStats(
            img, labels, stats, centroids
        );
        Mat clean = Mat.Zeros(img.Size(), MatType.CV_8U);

        for (int i = 1; i < n; i++)
        {
            int area = stats.At<int>(i, (int)ConnectedComponentsTypes.Area);

            if (area >= 20)
            {
                Mat mask = new Mat();
                Cv2.Compare(labels, i, mask, 0);
                clean.SetTo(255, mask);
                mask.Dispose();
            }
        }
        labels.Dispose();
        stats.Dispose();
        centroids.Dispose();
        return clean;
    }

    /// <summary>
    /// Zobrazí obraz v zmenšeném měřítku.
    /// </summary>
    /// <param name="img">Obraz určený k zobrazení.</param>
    private void ShowImage(Mat img)
    {
        if (img == null || img.IsDisposed || img.Empty())
        {
            Console.WriteLine("ShowImage: img je null / disposed / empty");
            return;
        }

        using var small = new Mat();
        Cv2.Resize(img, small, new Size(), 0.5, 0.5);

        Cv2.ImShow("Reference", small);
        Cv2.WaitKey();
    }

}