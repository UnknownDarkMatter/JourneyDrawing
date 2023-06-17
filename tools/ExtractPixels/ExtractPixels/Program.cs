/*
    - listing des positions des bordures des continents
    - aouter une dimension s, trier par s pour qu'il puisse être une clé
        a partir de la clé s, on a les contours avec s +/- 1

    - pour chaque s du contour, tableau selon l'angle 360 qui donne un tuble <distance, autre s projetté à la distance>
    11714 pixels pour les contours * 360 = 4 217 040 entrées dans le tableau

    ALGO : 
        partant d'un s, get autre s projeté à la distance
        aller à la distance entre la cible et le point de départ
        calculer ax + b de la droite vers la cible et la fonction qui sur la droite vaut zero 
            ainsi que bornes pour rester sur le segment (xmin, xmax)
        boucle sur tous les s en + ou en - en parallele : 
            prendre l'ensemble des s qui permet de retrouver un des points du segment cible et ayant le delta de s minimum
    
    s est un vecteur a 3 composantes afin de gerer les boucles (s1+1= s2-1 en tout s): 
        un identifiant unique
        une valeur de l'identifiant de s pour croitre de 1
        une valeur del'identifiant de s pour decroitre de 1

*/
using ExtractPixels.MapProcessing;
using ExtractPixels.MapProcessing.Model;
using System.Drawing;

string imageFilePath = Path.Combine(Environment.CurrentDirectory, "image.png");
string outputMapPath = Path.Combine(Environment.CurrentDirectory, "map.png");
string workMapPath = Path.Combine(Environment.CurrentDirectory, "map_step1.png");
string workMapPath2 = Path.Combine(Environment.CurrentDirectory, "map_step2.png");

var borderPointsCollection = new BorderPointCollection();
var borderExtractor = new BorderWalkingPointExtractor(new List<IMapPointHandler>() { borderPointsCollection });


int i = 1;//continent number
int s = 1;//variable indicating progress when walking along the continents

using (var imageSource = new Bitmap(Image.FromFile(imageFilePath)))
{
    int width = imageSource.Width;
    int height = imageSource.Height;

    var imageWork = imageSource.Clone() as Bitmap;
    var imageOutput = imageSource.Clone() as Bitmap;

    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            imageWork.SetPixel(x, y, Color.White);
            imageOutput.SetPixel(x, y, Color.White);
        }
    }

    var prod = true;
    if (prod)
    {
        borderExtractor.ExtractBorder(1847, 75, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//continent eurasien
        borderExtractor.ExtractBorder(973, 211, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(972, 220, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(991, 236, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(904, 136, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//angleterre
        borderExtractor.ExtractBorder(875, 153, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//irlande
        borderExtractor.ExtractBorder(896, 248, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//contient africain
        borderExtractor.ExtractBorder(1177, 497, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//madagascar
        borderExtractor.ExtractBorder(1657, 488, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//australie
        borderExtractor.ExtractBorder(1656, 157, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//japon
        borderExtractor.ExtractBorder(1655, 200, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//japon
        borderExtractor.ExtractBorder(1649, 219, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//japon
        borderExtractor.ExtractBorder(1608, 257, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//japon
        borderExtractor.ExtractBorder(1596, 258, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//japon
        borderExtractor.ExtractBorder(1524, 396, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(1415, 403, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(1470, 462, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(1562, 390, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(1545, 426, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(1579, 426, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(1539, 439, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(1574, 448, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(1528, 474, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(1543, 474, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(1538, 480, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(1568, 476, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(1568, 476, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(1604, 434, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(1693, 460, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(1487, 330, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(1543, 341, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(1335, 386, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(1677, 644, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(1812, 611, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(1789, 671, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(822, 92, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//islande
        borderExtractor.ExtractBorder(748, 2, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//groenland
        borderExtractor.ExtractBorder(443, 339, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//amerique du nord
        borderExtractor.ExtractBorder(526, 392, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//amerique du sud
        borderExtractor.ExtractBorder(505, 313, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
        borderExtractor.ExtractBorder(557, 329, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);//
    }
    else
    {
        borderExtractor.ExtractBorder(26, 222, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);
        borderExtractor.ExtractBorder(11, 243, i++, ref s, imageSource, imageWork, workMapPath, imageOutput, outputMapPath);
    }
    borderPointsCollection.LinkFirstAndLastOfContinent(null);


    File.Copy(workMapPath, workMapPath2, true);

    Console.WriteLine("Veuillez ouvrir avec paint l'image suivante et remplir les continents avec une couleur");
    Console.WriteLine($"Fichier : {workMapPath2}");
    Console.WriteLine("(  La couleur des bordures est RGB(119, 255, 118)  )");
    Console.ReadLine();

    //MapUtils.DrawLine(new MapPoint(26, 219), new MapPoint(189, 304), width, height, imageFilePath);//pente negative, inversion y en biais
    //MapUtils.DrawLine(new MapPoint(77, 94), new MapPoint(80, 333), width, height, imageFilePath);//pente negative vertical
    //MapUtils.DrawLine(new MapPoint(124, 285), new MapPoint(212, 281), width, height, imageFilePath);//pente negative horizontal
    //MapUtils.DrawLine(new MapPoint(43, 330), new MapPoint(118, 232), width, height, imageFilePath);//pente positive, inversion y en biais
    //MapUtils.DrawLine(new MapPoint(98, 340), new MapPoint(103, 264), width, height, imageFilePath);//pente positive vertical
    //MapUtils.DrawLine(new MapPoint(254, 459), new MapPoint(350, 450), width, height, imageFilePath);//pente positive horizontal
    //MapUtils.DrawLine(new MapPoint(141, 778), new MapPoint(290, 710), width, height, imageFilePath);//pente positive diagonale
    //MapUtils.DrawLine(new MapPoint(151, 70), new MapPoint(300, 137), width, height, imageFilePath);//pente negative diagonale
    //MapUtils.DrawLine(new MapPoint(151, 70), new MapPoint(300, 120), width, height, imageFilePath);//pente negative diagonale
}

Console.WriteLine("ended");