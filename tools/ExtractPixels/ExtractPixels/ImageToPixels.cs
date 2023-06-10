using System.Drawing;
using System.Globalization;
using System.Text;

public static class ImageToPixels
{
    public static void ExtractPixels()
    {

        string inputPath = Path.Combine(Environment.CurrentDirectory, "image.png");
        string outputCsv = Path.Combine(Environment.CurrentDirectory, "pixels.csv");
        var cultureInfo = new CultureInfo("fr-FR");


        var sb = new StringBuilder();
        sb.AppendLine("X;Y");

        using (var image = new Bitmap(System.Drawing.Image.FromFile(inputPath)))
        {
            int width = image.Width;
            int height = image.Height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //if(x == 118 && y == 68)
                    //{
                    //    string a = "a";
                    //    a = "e";
                    //}
                    Color pixelColor = image.GetPixel(x, y);
                    if (pixelColor.Name != "0")
                    {
                        sb.AppendLine($"{x.ToString(cultureInfo)};{y.ToString(cultureInfo)}");
                    }
                }
            }
        }

        File.WriteAllText(outputCsv, sb.ToString());

    }
}
