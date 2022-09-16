using System;
using Rottytooth.Entropy;

namespace Rottytooth
{
public class NinetyNineBottles
{
static void Main()
{
	Rottytooth.Entropy.Real.MutationRate = 2F;
Rottytooth.Entropy.Real CONST0 = 99;
Rottytooth.Entropy.Real CONST1 = 0;
Rottytooth.Entropy.Real CONST2 = 99;
Rottytooth.Entropy.String CONST3 = " bottles of beer on the wall.\n";
Rottytooth.Entropy.String CONST4 = " bottles of beer on the wall, ";
Rottytooth.Entropy.String CONST5 = " bottles of beer.\nTake one down, pass it around, ";
Rottytooth.Entropy.Real CONST6 = 1;
Rottytooth.Entropy.String CONST7 = " no more bottles of beer on the wall.";

Rottytooth.Entropy.Real count;
count = CONST0;
while ( count  > CONST1)
{
if ( count  < CONST2)
{
Console.Write( count );
Console.Write(CONST3);
}
Console.Write( count );
Console.Write(CONST4);
Console.Write( count );
Console.Write(CONST5);
count =  count  - CONST6;
}
Console.Write(CONST7);
}
}
}
