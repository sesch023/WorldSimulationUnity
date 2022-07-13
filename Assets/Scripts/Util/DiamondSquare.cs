using System;

namespace Util
{
    // https://gist.github.com/awilki01/83b65ad852a0ab30192af07cda3d7c0b
    public static class DiamondSquare
	{
		// Diese Methode berechnet die Werte mit dem Diamond-square Algorithmus
		// Gegen ist die Seitenlänge des Quadrats, der Bereich für die zufälligen Werte und der Wert für die 4 Ecken des Quadrats
		public static float[,] CalculateValues(int length, float roughness, float seed)
		{
			// Initialisiert den Wert der 4 Ecken des Quadrats
			float[,] values = new float[length, length]; // Initialisiert das zweidimensionale Array für die Werte des quadratischen Rasters
			values[0, 0] = seed; // Wert der Ecke links oben
			values[0, length - 1] = seed; // Wert der Ecke links unten
			values[length - 1, 0] = seed; // Wert der Ecke rechts oben
			values[length - 1, length - 1] = seed; // Wert der Ecke rechts unten
			Random random = new Random(); // Initialisiert den Zufallsgenerator
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
			return values;
		}
	}
}