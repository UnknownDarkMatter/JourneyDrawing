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
string workMapPath = Path.Combine(Environment.CurrentDirectory, "map_step1.png");
string workMapPath2 = Path.Combine(Environment.CurrentDirectory, "map_step2.png");

var borderWalkingPointsCollection = new BorderPointCollection();
var borderWalkingPointExtractor = new BorderWalkingPointExtractor(new List<IMapPointHandler>() { borderWalkingPointsCollection });

var prod = false;
int i = 1;//continent number
int s = 1;//variable indicating progress when walking along the continents

using (var imageSource = new Bitmap(Image.FromFile(imageFilePath)))
{
    int width = imageSource.Width;
    int height = imageSource.Height;

    var imageWork = imageSource.Clone() as Bitmap;

    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            imageWork.SetPixel(x, y, Color.White);
        }
    }


    if (prod)
    {

    }
    else
    {
        //borderWalkingPointExtractor.ExtractBorderWalkingPoints(26, 222, i++, ref s, imageSource, imageWork, workMapPath);
        //borderWalkingPointExtractor.ExtractBorderWalkingPoints(11, 243, i++, ref s, imageSource, imageWork, workMapPath);
    }
    //borderWalkingPointsCollection.LinkFirstAndLastOfContinent(null);


    //File.Copy(workMapPath, workMapPath2, true);

    //Console.WriteLine("Veuillez ouvrir avec paint l'image suivante et remplir les continents avec une couleur");
    //Console.WriteLine($"Fichier : {workMapPath2}");
    //Console.ReadLine();

    //MapUtils.DrawLine(new MapPoint(26, 219), new MapPoint(189, 304), width, height, imageFilePath);//pente negative, inversion y en biais
    //MapUtils.DrawLine(new MapPoint(77, 94), new MapPoint(80, 333), width, height, imageFilePath);//pente negative vertical
    //MapUtils.DrawLine(new MapPoint(124, 285), new MapPoint(212, 281), width, height, imageFilePath);//pente negative horizontal
    //MapUtils.DrawLine(new MapPoint(43, 330), new MapPoint(118, 232), width, height, imageFilePath);//pente positive, inversion y en biais
    //MapUtils.DrawLine(new MapPoint(98, 340), new MapPoint(103, 264), width, height, imageFilePath);//pente positive vertical
    //MapUtils.DrawLine(new MapPoint(254, 459), new MapPoint(350, 450), width, height, imageFilePath);//pente positive horizontal
    //MapUtils.DrawLine(new MapPoint(141, 778), new MapPoint(290, 710), width, height, imageFilePath);//pente positive diagonale
    //MapUtils.DrawLine(new MapPoint(151, 70), new MapPoint(300, 137), width, height, imageFilePath);//pente negative diagonale
    MapUtils.DrawLine(new MapPoint(151, 70), new MapPoint(300, 120), width, height, imageFilePath);//pente negative diagonale
}

Console.WriteLine("ended");