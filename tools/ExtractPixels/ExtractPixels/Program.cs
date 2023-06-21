﻿/*
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
using ExtractPixels.MapProcessing;
using ExtractPixels.MapProcessing.Model;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

string imageFilePath = Path.Combine(Environment.CurrentDirectory, "image.png");
string outputMapPath = Path.Combine(Environment.CurrentDirectory, "map.png");
string workMapPath = Path.Combine(Environment.CurrentDirectory, "map_step1.png");
string workMapPath2 = Path.Combine(Environment.CurrentDirectory, "map_step2.png");
string workMapPath3 = Path.Combine(Environment.CurrentDirectory, "map_step3.png");

var borderPointsCollection = new BorderPointCollection();
var borderExtractor = new BorderWalkingPointExtractor(new List<IMapPointHandler>() { borderPointsCollection });

//if (Constants.IsDebug)
//{
//    long maxCount = 9999 * 8888;
//    long count = 0;
//    for (int a = 1; a <= 9999; a++)
//    {
//        for (int b = 1; b <= 8888; b++)
//        {
//            count++;
//            var rest = (int) (count % (maxCount * 0.1M));
//            if (rest == 0 || rest  == (maxCount * 0.1M)) {
//                Console.WriteLine($"DONE {(int)(10 * (((decimal)count / (decimal)maxCount)))}%");
//            }
//        }
//    }

//    var tripData = new SeaTripsData();
//    tripData.SeaTrips.First().Value.First().Value.DrawTrip(new Bitmap(Image.FromFile(imageFilePath)), Path.Combine(Environment.CurrentDirectory, "map_step2_hardcoded.png"));
//}

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

    var prod = !Constants.IsDebug; ;
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
    Console.ReadLine();

    var tripGenerator = new TripGenerator();
    var csharpGenerator = new CSharpGenerator();

    if (Constants.IsDebug)
    {
        var s1 = borderPointsCollection.GetPoints().FirstOrDefault(m => m.X == 31 && m.Y == 208);
        var s2 = borderPointsCollection.GetPoints().FirstOrDefault(m => m.X == 189 && m.Y == 304);
        tripGenerator.CalculateAllTrips(borderPointsCollection, width, height, workMapPath2,
            new Bitmap(Image.FromFile(workMapPath2)), s1.S, s2.S);
        var trip = tripGenerator.SeaTrips[s1.S][s2.S];
        trip.DrawTrip(imageWork, workMapPath3);
    }
    else
    {
        tripGenerator.CalculateAllTrips(borderPointsCollection, width, height, workMapPath2, new Bitmap(Image.FromFile(workMapPath2)), null, null);
    }

    csharpGenerator.GenerateSCharp(tripGenerator.SeaTrips, Path.Combine(Environment.CurrentDirectory, "SeaTripsData.cs"));

}

Console.WriteLine("ended");