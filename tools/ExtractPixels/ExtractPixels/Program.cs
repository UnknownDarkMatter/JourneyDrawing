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
using ExtractPixels;
using System.Drawing;

string inputPath = Path.Combine(Environment.CurrentDirectory, "image.png");
string outputMapPath = Path.Combine(Environment.CurrentDirectory, "map.png");
string debugDumpPath = Path.Combine(Environment.CurrentDirectory, "dump.csv");

var mapPreprocessingGenerator = new MapPreprocessingGenerator();

int width;
int height;
using (var image = new Bitmap(Image.FromFile(inputPath)))
{
    width = image.Width;
    height = image.Height;
    var outputMap = image.Clone() as Bitmap;

    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            outputMap.SetPixel(x, y, Color.White);
        }
    }


    var bordersTracing = new BordersTracing(new List<IPixelHandler>() { mapPreprocessingGenerator });
    int i = 1;//continent number
    int s = 1;//variable indicating progress when walking along the continents

    var prod = true;
    if (prod)
    {
        //bordersTracing.DoTraceBorders(1847, 75, outputMap, i++, ref s);//continent eurasien
        //bordersTracing.DoTraceBorders(973, 211, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(972, 220, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(991, 236, outputMap, i++, ref s);//
        bordersTracing.DoTraceBorders(904, 136, outputMap, i++, ref s);//angleterre
        //bordersTracing.DoTraceBorders(875, 153, outputMap, i++, ref s);//irlande
        //bordersTracing.DoTraceBorders(896, 248, outputMap, i++, ref s);//contient africain
        bordersTracing.DoTraceBorders(1177, 497, outputMap, i++, ref s);//madagascar
        //bordersTracing.DoTraceBorders(1657, 488, outputMap, i++, ref s);//australie
        //bordersTracing.DoTraceBorders(1656, 157, outputMap, i++, ref s);//japon
        //bordersTracing.DoTraceBorders(1655, 200, outputMap, i++, ref s);//japon
        //bordersTracing.DoTraceBorders(1649, 219, outputMap, i++, ref s);//japon
        //bordersTracing.DoTraceBorders(1608, 257, outputMap, i++, ref s);//japon
        //bordersTracing.DoTraceBorders(1596, 258, outputMap, i++, ref s);//japon
        //bordersTracing.DoTraceBorders(1524, 396, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(1415, 403, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(1470, 462, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(1562, 390, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(1545, 426, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(1579, 426, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(1539, 439, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(1574, 448, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(1528, 474, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(1543, 474, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(1538, 480, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(1568, 476, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(1568, 476, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(1604, 434, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(1693, 460, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(1487, 330, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(1543, 341, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(1335, 386, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(1677, 644, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(1812, 611, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(1789, 671, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(822, 92, outputMap, i++, ref s);//islande
        //bordersTracing.DoTraceBorders(748, 2, outputMap, i++, ref s);//groenland
        //bordersTracing.DoTraceBorders(443, 339, outputMap, i++, ref s);//amerique du nord
        //bordersTracing.DoTraceBorders(526, 392, outputMap, i++, ref s);//amerique du sud
        //bordersTracing.DoTraceBorders(505, 313, outputMap, i++, ref s);//
        //bordersTracing.DoTraceBorders(557, 329, outputMap, i++, ref s);//
                                                           
    }
    else
    {
        bordersTracing.DoTraceBorders(137, 265, outputMap, i++, ref s);//
        bordersTracing.DoTraceBorders(37, 226, outputMap, i++, ref s);//
        bordersTracing.DoTraceBorders(20, 299, outputMap, i++, ref s);//
    }

    outputMap.Save(outputMapPath);
    outputMap.Dispose();
}

ImageToPixels.ExtractPixels(outputMapPath);

mapPreprocessingGenerator.LinkFirstAndLastOfContinent(null);
mapPreprocessingGenerator.Step1GenerateBorderWalkingVariable();
mapPreprocessingGenerator.Dump(debugDumpPath);
