using System;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Random = System.Random;

namespace Model.Generators
{
	[Serializable]
    [CreateAssetMenu(fileName = "DiamondSquareGenerator", menuName = "ScriptableObjects/DiamondSquareGenerator", order = 1)]
    public class DiamondSquareGenerator : BaseGenerator
    {
	    [SerializeField] 
	    private float cornerSeed = 0;
	    [SerializeField] 
	    private int randomSeed = 0;
	    [SerializeField] 
	    private float roughness = 1024;

	    private void OnEnable()
	    {
		    if (cornerSeed == 0)
		    {
			    cornerSeed = DateTime.Now.Ticks % 2000 - 1000;
		    }
		    
		    if (randomSeed == 0)
		    {
			    randomSeed = (int)(DateTime.Now.Ticks % int.MaxValue);
		    }
	    }
	    
        public override float[,] GenerateElevation(int sizeX, int sizeY)
        {
	        int length = LimitMapSizes(sizeX, sizeY).sizeX;
	        // ReSharper disable once LocalVariableHidesMember
	        float roughness = this.roughness;
	        
            // Initialisiert den Wert der 4 Ecken des Quadrats
			float[,] values = new float[length, length]; // Initialisiert das zweidimensionale Array für die Werte des quadratischen Rasters
			values[0, 0] = cornerSeed; // Wert der Ecke links oben
			values[0, length - 1] = cornerSeed; // Wert der Ecke links unten
			values[length - 1, 0] = cornerSeed; // Wert der Ecke rechts oben
			values[length - 1, length - 1] = cornerSeed; // Wert der Ecke rechts unten
			Random random = new Random(randomSeed); // Initialisiert den Zufallsgenerator
			// Diese for-Schleife definiert die Iterationsschritte des Algorithmus
			// In jedem Iterationsschritt wird die Seitenlänge der Teilquadrate und der Bereich für die zufälligen Werte halbiert, bis die Seitenlänge kleiner als 2 ist
			for (int sideLength = length - 1; sideLength >= 2; sideLength /= 2, roughness /= 2.0f)
			{
				int halfLength = sideLength / 2;
				// In diesen zwei for-Schleifen werden die Werte für den Karoschritt berechnet
				for (int x = 0; x < length - 1; x += sideLength)
				{
					for (int y = 0; y < length - 1; y += sideLength)
					{
						// Berechnet den Mittelwert der 4 Ecken des Teilquadrats
						float average = (values[x, y] // Wert der Ecke links oben
						                  + values[x, y + sideLength] // Wert der Ecke links unten
						                  + values[x + sideLength, y] // Wert der Ecke rechts oben
						                  + values[x + sideLength, y + sideLength]) / 4.0f; // Wert der Ecke rechts unten
						average += (2 * roughness * (float)random.NextDouble()) - roughness; // Addiert einen zufälligen Wert im Bereich von -roughness bis +roughness zum Mittelwert
						values[x + halfLength, y + halfLength] = average; // Setzt den Wert für den Mittelpunkt des Teilquadrats
					}
				}
				// In diesen zwei for-Schleifen werden die Werte für den Quadratschritt berechnet
				for (int x = 0; x < length - 1; x += halfLength)
				{
					for (int y = (x + halfLength) % sideLength; y < length - 1; y += sideLength)
					{
						// Berechnet den Mittelwert der 4 Ecken des Karos
						float average = (values[(x - halfLength + length) % length, y] // Wert der linken Ecke
						                 + values[(x + halfLength) % length, y] // Wert der rechten Ecke
						                 + values[x, (y - halfLength + length) % length] // Wert der oberen Ecke
						                 + values[x, (y + halfLength) % length]) / 4.0f; // Wert der unteren Ecke
						average += (2f * roughness * (float)random.NextDouble()) - roughness; // Addiert einen zufälligen Wert im Bereich von -roughness bis +roughness zum Mittelwert
						values[x, y] = average; // Setzt den Wert für den Mittelpunkt des Karos
						if (x == 0) // Sonderfall für die linke Kante des Quadrats
						{
							values[length - 1, y] = average; // Setzt den entsprechenden Punkt auf der rechte Kante auf denselben Wert
						}
						if (y == 0) // Sonderfall für die obere Kante des Quadrats
						{
							values[x, length - 1] = average; // Setzt den entsprechenden Punkt auf der unteren Kante auf denselben Wert
						}
					}
				}
			}
			return NormalizeElevation(values, minHeight, maxHeight);
        }

        public override (int sizeX, int sizeY) LimitMapSizes(int sizeX, int sizeY)
        {
	        int bigger = Math.Max(sizeX, sizeY);

	        if (MathUtil.IsPowerOfTwo(bigger - 1))
	        {
		        return (bigger, bigger);
	        }
            int biggerLog = ((int)Math.Log(bigger, 2.0)) + 1;
            int newSize = ((int)Math.Pow(2, biggerLog)) + 1;

            return (newSize, newSize);
        }
    }
}