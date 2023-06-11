/*
    - listing des positions des bordures des continents
    - aouter une dimension s, trier par s pour qu'il puisse être une clé
        a partir de la clé s, on a les contours avec s +/- 1

    - pour chaque s du contour, tableau selon l'angle 360 qui donne un tuble <distance, autre s projetté à la distance>
    24166 pixels pour les contours * 360 = 8 699 760 entrées dans le tableau

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
using System.Drawing;

string inputPath = Path.Combine(Environment.CurrentDirectory, "image.png");
string outputMapPath = Path.Combine(Environment.CurrentDirectory, "map.png");

int width;
int height;
using (var image = new Bitmap(System.Drawing.Image.FromFile(inputPath)))
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
    BordersTracing.DoTraceBorders(1847, 75, outputMap);//continent eurasien
    BordersTracing.DoTraceBorders(973, 211, outputMap);//
    BordersTracing.DoTraceBorders(972, 220, outputMap);//
    BordersTracing.DoTraceBorders(991, 236, outputMap);//
    BordersTracing.DoTraceBorders(904, 136, outputMap);//angleterre
    BordersTracing.DoTraceBorders(875, 153, outputMap);//irlande
    BordersTracing.DoTraceBorders(896, 248, outputMap);//contient africain
    BordersTracing.DoTraceBorders(1177, 497, outputMap);//madagascar
    BordersTracing.DoTraceBorders(1657, 488, outputMap);//australie
    BordersTracing.DoTraceBorders(1656, 157, outputMap);//japon
    BordersTracing.DoTraceBorders(1655, 200, outputMap);//japon
    BordersTracing.DoTraceBorders(1649, 219, outputMap);//japon
    BordersTracing.DoTraceBorders(1608, 257, outputMap);//japon
    BordersTracing.DoTraceBorders(1596, 258, outputMap);//japon
    BordersTracing.DoTraceBorders(1524, 396, outputMap);//
    BordersTracing.DoTraceBorders(1415, 403, outputMap);//
    BordersTracing.DoTraceBorders(1470, 462, outputMap);//
    BordersTracing.DoTraceBorders(1562, 390, outputMap);//
    BordersTracing.DoTraceBorders(1545, 426, outputMap);//
    BordersTracing.DoTraceBorders(1579, 426, outputMap);//
    BordersTracing.DoTraceBorders(1539, 439, outputMap);//
    BordersTracing.DoTraceBorders(1574, 448, outputMap);//
    BordersTracing.DoTraceBorders(1528, 474, outputMap);//
    BordersTracing.DoTraceBorders(1543, 474, outputMap);//
    BordersTracing.DoTraceBorders(1538, 480, outputMap);//
    BordersTracing.DoTraceBorders(1568, 476, outputMap);//
    BordersTracing.DoTraceBorders(1568, 476, outputMap);//
    BordersTracing.DoTraceBorders(1604, 434, outputMap);//
    BordersTracing.DoTraceBorders(1693, 460, outputMap);//
    BordersTracing.DoTraceBorders(1487, 330, outputMap);//
    BordersTracing.DoTraceBorders(1543, 341, outputMap);//
    BordersTracing.DoTraceBorders(1335, 386, outputMap);//
    BordersTracing.DoTraceBorders(1677, 644, outputMap);//
    BordersTracing.DoTraceBorders(1812, 611, outputMap);//
    BordersTracing.DoTraceBorders(1789, 671, outputMap);//
    BordersTracing.DoTraceBorders(822, 92, outputMap);//islande
    BordersTracing.DoTraceBorders(748, 2, outputMap);//groenland
    BordersTracing.DoTraceBorders(443, 339, outputMap);//amerique du nord
    BordersTracing.DoTraceBorders(526, 392, outputMap);//amerique du sud
    BordersTracing.DoTraceBorders(505, 313, outputMap);//
    BordersTracing.DoTraceBorders(557, 329, outputMap);//


    outputMap.Save(outputMapPath);
    outputMap.Dispose();
}





ImageToPixels.ExtractPixels(outputMapPath);



